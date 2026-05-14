#!/usr/bin/env python3
"""
Linux packaging CI/local orchestration (Python entrypoint).
Mirrors .github/workflows/linux-packaging.yml linux-packages job (except SBOM curl step).

Requires on POSIX hosts: bash (for packaging/tools/build-*.sh inside this orchestrator),
git, python3. For Windows-only hosts without bash, build .deb/.rpm via Docker using
packaging/docker/Dockerfile.build-deb / Dockerfile.build-rpm (see run_rebuild_and_e2e.py).

The packaging-worker image (packaging/docker/Dockerfile.packaging-worker-ubuntu24) now includes
dotnet-sdk-8.0 for in-container builds used by run-full-linux-build-docker-inner.sh.
"""

from __future__ import annotations

import argparse
import os
import shutil
import subprocess
import sys
from pathlib import Path

_REPO_ROOT = Path(__file__).resolve().parents[2]
if str(_REPO_ROOT) not in sys.path:
    sys.path.insert(0, str(_REPO_ROOT))


def repo_root() -> Path:
    return _REPO_ROOT


def die(msg: str, code: int = 1) -> None:
    print(msg, file=sys.stderr)
    raise SystemExit(code)


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
    for rel in (
        "packaging/tools/sbom-and-attest.sh",
        "packaging/tools/run-full-linux-build-docker-inner.sh",
    ):
        p = root / rel
        if p.is_file():
            try:
                p.chmod(p.stat().st_mode | 0o111)
            except OSError:
                pass


def run_cmd(cmd: list[str], *, cwd: Path, env: dict | None = None) -> None:
    r = subprocess.run(cmd, cwd=cwd, env=env)
    if r.returncode != 0:
        die(f"Command failed ({r.returncode}): {' '.join(cmd)}", r.returncode)


def bash() -> str:
    b = shutil.which("bash")
    if not b:
        die("bash is required on this host to run packaging/tools/build-*.sh (Linux CI).")
    return b


def phase_packages(root: Path, arch: str) -> None:
    """stage.py + native build-*.sh + write-checksums (matches CI Stage and package)."""
    b = bash()
    py = sys.executable
    env = os.environ.copy()
    r = subprocess.run(["git", "-C", str(root), "log", "-1", "--format=%ct"], capture_output=True, text=True)
    if r.returncode == 0 and r.stdout.strip():
        env["SOURCE_DATE_EPOCH"] = r.stdout.strip()

    run_cmd([py, str(root / "packaging/tools/stage.py"), "--arch", arch], cwd=root, env=env)
    run_cmd([b, str(root / "packaging/tools/build-deb.sh"), "--arch", arch], cwd=root, env=env)
    run_cmd([b, str(root / "packaging/tools/build-rpm.sh"), "--arch", arch], cwd=root, env=env)
    run_cmd([b, str(root / "packaging/tools/build-pacman.sh"), "--arch", arch], cwd=root, env=env)
    run_cmd(
        [b, str(root / "packaging/tools/build-flatpak.sh"), "--arch", arch, "--require-flatpak-builder"],
        cwd=root,
        env=env,
    )
    run_cmd([b, str(root / "packaging/tools/build-snap.sh"), "--arch", arch, "--require-snapcraft"], cwd=root, env=env)
    run_cmd([b, str(root / "packaging/tools/write-checksums.sh"), "--arch", arch], cwd=root, env=env)


def phase_lint(root: Path, arch: str) -> None:
    run_cmd([sys.executable, str(root / "packaging/tests/run_package_tests.py"), arch], cwd=root)


def phase_smoke(root: Path, arch: str) -> None:
    py = sys.executable
    run_cmd(
        [py, str(root / "packaging/ci/docker_firewall_run_all.py"), "--integration"],
        cwd=root,
    )
    run_cmd([py, str(root / "packaging/ci/docker_install_smoke.py"), "deb", "--arch", arch], cwd=root)
    run_cmd([py, str(root / "packaging/ci/docker_install_smoke.py"), "rpm", "--arch", arch], cwd=root)
    run_cmd([py, str(root / "packaging/ci/docker_install_smoke.py"), "pacman", "--arch", arch], cwd=root)
    run_cmd([py, str(root / "packaging/ci/docker_install_smoke.py"), "flatpak", "--arch", arch], cwd=root)


def main() -> int:
    root = repo_root()
    ap = argparse.ArgumentParser(description="Linux packaging: stage, build, lint, Docker smoke tests.")
    ap.add_argument("--arch", default="amd64", choices=("amd64", "arm64"))
    ap.add_argument(
        "--phase",
        choices=("all", "packages", "lint", "smoke"),
        default="all",
        help="Run only a subset (default: all).",
    )
    ap.add_argument(
        "--no-chmod",
        action="store_true",
        help="Skip chmod +x on packaging shell scripts.",
    )
    args = ap.parse_args()
    os.chdir(root)

    if not args.no_chmod:
        chmod_scripts(root)

    if args.phase in ("all", "packages"):
        phase_packages(root, args.arch)
    if args.phase in ("all", "lint"):
        phase_lint(root, args.arch)
    if args.phase in ("all", "smoke"):
        phase_smoke(root, args.arch)
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
