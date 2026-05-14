#!/usr/bin/env python3
"""
Run Linux packaging tests from a host with Docker + dotnet (e.g. Windows with Python).
Replaces run-all-linux-host.sh for host/CI. Uses packaging/ci Python drivers (no host bash).

PowerShell: avoid `2>&1` if debconf/docker stderr lines show as NativeCommandError; they are harmless.

The gRPC firewall E2E rows match the CI matrix in .github/workflows/linux-packaging.yml (without
snap/flatpak). CI runs those rows as separate parallel jobs; locally use -j N to overlap them
(see --help). Pre-publishes VideoDedupGrpcSmoke once when -j > 1 so parallel workers do not race
on packaging/out/<arch>/e2e-smoke/.
"""

from __future__ import annotations

import argparse
import os
import subprocess
import sys
from concurrent.futures import ThreadPoolExecutor, as_completed
from pathlib import Path

from host_script_paths import repo_root_from_script


def run_step(title: str, cmd: list[str], cwd: Path) -> None:
    # flush=True: on Windows, child stdout can appear before this line without a flush.
    print(f"=== {title} ===", flush=True)
    env = os.environ.copy()
    env.setdefault("PYTHONUNBUFFERED", "1")
    r = subprocess.run(cmd, cwd=cwd, env=env)
    if r.returncode != 0:
        raise SystemExit(r.returncode)


def chmod_scripts(root: Path) -> None:
    patterns = [
        root / "packaging" / "tools",
        root / "packaging" / "common",
        root / "packaging" / "tests",
        root / "packaging" / "tests" / "firewall",
        root / "packaging" / "tests" / "install",
        root / "packaging" / "tests" / "e2e",
    ]
    for base in patterns:
        if not base.is_dir():
            continue
        for p in base.glob("*.sh"):
            try:
                p.chmod(p.stat().st_mode | 0o111)
            except OSError:
                pass
    for name in ("generate-metadata.sh", "packaging-python.sh"):
        p = root / "packaging" / "common" / name
        if p.is_file():
            try:
                p.chmod(p.stat().st_mode | 0o111)
            except OSError:
                pass


def rid_for_arch(arch: str) -> str:
    if arch == "amd64":
        return "linux-x64"
    if arch == "arm64":
        return "linux-arm64"
    raise SystemExit(f"Unsupported arch: {arch}")


def ensure_shared_smoke_publish(root: Path, arch: str) -> Path:
    """Publish VideoDedupGrpcSmoke once for parallel E2E workers (avoids dotnet publish races)."""
    e2e_dir = root / "packaging" / "tests" / "e2e"
    if str(e2e_dir) not in sys.path:
        sys.path.insert(0, str(e2e_dir))
    from docker_e2e_common import publish_dotnet_project  # noqa: PLC0415

    smoke_dir = (root / "packaging" / "out" / arch / "e2e-smoke").resolve()
    dll = smoke_dir / "VideoDedupGrpcSmoke.dll"
    if dll.is_file():
        print(f"Using existing smoke build: {dll}", flush=True)
        return smoke_dir
    print(f"Publishing VideoDedupGrpcSmoke once to {smoke_dir} (parallel E2E) ...", flush=True)
    smoke_dir.mkdir(parents=True, exist_ok=True)
    publish_dotnet_project(
        root,
        "VideoDedupGrpcSmoke/VideoDedupGrpcSmoke.csproj",
        smoke_dir,
        rid_for_arch(arch),
    )
    return smoke_dir


def main() -> None:
    ap = argparse.ArgumentParser(
        description="Linux packaging smoke + gRPC firewall E2E (host Docker + Python).",
    )
    ap.add_argument(
        "-j",
        "--parallel",
        type=int,
        default=1,
        metavar="N",
        help=(
            "Run gRPC firewall E2E combinations with up to N concurrent processes "
            "(default 1 = sequential, like a single CI runner). "
            "Matches CI job-level parallelism when N >= number of rows. "
            "Requires enough Docker CPU/RAM; use a smaller N on laptops."
        ),
    )
    ap.add_argument(
        "arch",
        nargs="?",
        default="amd64",
        choices=("amd64", "arm64"),
        help="Target packaging arch (default: amd64).",
    )
    args = ap.parse_args()

    root = repo_root_from_script(Path(__file__))
    os.chdir(root)
    arch = args.arch
    parallel = max(1, args.parallel)

    chmod_scripts(root)

    py = sys.executable
    run_step(
        "run_package_tests.py",
        [py, str(root / "packaging" / "tests" / "run_package_tests.py"), arch],
        root,
    )
    run_step(
        "firewall docker_firewall_run_all.py --integration",
        [py, str(root / "packaging" / "ci" / "docker_firewall_run_all.py"), "--integration"],
        root,
    )
    run_step(
        "docker_install_smoke deb",
        [py, str(root / "packaging" / "ci" / "docker_install_smoke.py"), "deb", "--arch", arch],
        root,
    )
    run_step(
        "docker_install_smoke rpm",
        [py, str(root / "packaging" / "ci" / "docker_install_smoke.py"), "rpm", "--arch", arch],
        root,
    )

    rows = [
        ("debian", "deb", "nft"),
        ("debian", "deb", "iptables"),
        ("ubuntu", "deb", "ufw"),
        ("fedora", "rpm", "nft"),
        ("fedora", "rpm", "firewalld"),
        ("rocky", "rpm", "iptables"),
        ("opensuse", "rpm", "nft"),
        ("arch", "staged", "iptables"),
        ("manjaro", "staged", "nft"),
    ]
    fw_py = root / "packaging" / "tests" / "e2e" / "docker_grpc_firewall.py"

    def e2e_cmd(d: str, f: str, w: str, smoke_dir: Path | None) -> list[str]:
        cmd = [
            py,
            str(fw_py),
            "--arch",
            arch,
            "--distro",
            d,
            "--format",
            f,
            "--firewall",
            w,
        ]
        if smoke_dir is not None:
            cmd += ["--smoke-dir", str(smoke_dir)]
        return cmd

    if parallel == 1:
        for d, f, w in rows:
            run_step(
                f"e2e: {d} {f} {w}",
                e2e_cmd(d, f, w, None),
                root,
            )
    else:
        smoke_shared = ensure_shared_smoke_publish(root, arch)
        workers = min(parallel, len(rows))
        print(
            f"=== e2e gRPC firewall: {len(rows)} combinations, up to {workers} concurrent workers (-j {parallel}) ===",
            flush=True,
        )

        def run_one(label: str, cmd: list[str]) -> tuple[str, int]:
            print(f"[e2e] start {label}", flush=True)
            r = subprocess.run(cmd, cwd=root)
            print(f"[e2e] done {label} exit={r.returncode}", flush=True)
            return label, r.returncode

        failures: list[tuple[str, int]] = []
        with ThreadPoolExecutor(max_workers=workers) as ex:
            future_map = {
                ex.submit(run_one, f"e2e: {d} {f} {w}", e2e_cmd(d, f, w, smoke_shared)): (d, f, w)
                for d, f, w in rows
            }
            for fut in as_completed(future_map):
                label, code = fut.result()
                if code != 0:
                    failures.append((label, code))

        if failures:
            for label, code in sorted(failures, key=lambda x: x[0]):
                print(f"FAILED: {label} (exit {code})", file=sys.stderr, flush=True)
            raise SystemExit(1)

    print("ALL_LINUX_TESTS_OK", flush=True)


if __name__ == "__main__":
    main()
