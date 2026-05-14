#!/usr/bin/env python3
"""
One-shot: build Linux package(s) -> docker_grpc_firewall E2E.
When Docker is available, .deb / .rpm are built via packaging/docker/Dockerfile.build-* (no host bash).
Otherwise uses packaging/tools/build-*.sh (bash) or WSL on Windows.
"""

from __future__ import annotations

import os
import re
import shutil
import subprocess
import sys
from pathlib import Path

_REPO_ROOT = Path(__file__).resolve().parents[3]
sys.path.insert(0, str(_REPO_ROOT))
sys.path.insert(0, str(_REPO_ROOT / "packaging" / "tests"))
from host_script_paths import repo_root_from_script  # noqa: E402

from packaging.ci.deb_docker_build import build_deb_via_docker  # noqa: E402
from packaging.ci.docker_paths import docker_ok  # noqa: E402
from packaging.ci.rpm_docker_build import build_rpm_via_docker  # noqa: E402


def to_wsl_path(root: Path) -> str:
    """Git Bash /cygdrive style /d/foo -> /mnt/d/foo for wsl.exe -e bash -lc 'cd ...'."""
    s = str(root.resolve())
    if s.startswith("/mnt/"):
        return s
    m = re.match(r"^/([a-zA-Z])/(.*)$", s)
    if m:
        drive = m.group(1).lower()
        rest = m.group(2)
        return f"/mnt/{drive}/{rest}"
    return s


def chmod_scripts(root: Path) -> None:
    for pat in (
        root / "packaging" / "tools",
        root / "packaging" / "common",
        root / "packaging" / "tests" / "e2e",
        root / "packaging" / "tests" / "install",
        root / "packaging" / "tests" / "firewall",
    ):
        if not pat.is_dir():
            continue
        for p in pat.glob("*.sh"):
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


def run_bash_script(root: Path, rel: str, *args: str) -> None:
    r = subprocess.run(["bash", str(root / rel), *args], cwd=root)
    if r.returncode != 0:
        raise SystemExit(r.returncode)


def run_build_deb_native(root: Path, arch: str) -> None:
    if shutil.which("dpkg-deb"):
        run_bash_script(root, "packaging/tools/build-deb.sh", "--arch", arch)
        return
    wsl_root = to_wsl_path(root)
    inner = f"cd '{wsl_root}' && ./packaging/tools/build-deb.sh --arch '{arch}'"
    if shutil.which("wsl.exe"):
        subprocess.run(["wsl.exe", "-e", "bash", "-lc", inner], cwd=root, check=True)
        return
    if shutil.which("wsl"):
        subprocess.run(["wsl", "-e", "bash", "-lc", inner], cwd=root, check=True)
        return
    print(
        "packaging: install dpkg-deb (Linux/WSL) or use WSL so the .deb step can run.",
        file=sys.stderr,
    )
    raise SystemExit(1)


def run_build_rpm_native(root: Path, arch: str) -> None:
    if shutil.which("rpmbuild"):
        run_bash_script(root, "packaging/tools/build-rpm.sh", "--arch", arch)
        return
    wsl_root = to_wsl_path(root)
    inner = f"cd '{wsl_root}' && ./packaging/tools/build-rpm.sh --arch '{arch}'"
    if shutil.which("wsl.exe"):
        subprocess.run(["wsl.exe", "-e", "bash", "-lc", inner], cwd=root, check=True)
        return
    if shutil.which("wsl"):
        subprocess.run(["wsl", "-e", "bash", "-lc", inner], cwd=root, check=True)
        return
    print(
        "packaging: install rpmbuild (e.g. sudo apt install rpm on Debian/Ubuntu/WSL) for the .rpm step.",
        file=sys.stderr,
    )
    raise SystemExit(1)


def main() -> int:
    root = repo_root_from_script(Path(__file__))
    os.chdir(root)

    arch = "amd64"
    fmt = "deb"
    argv = sys.argv[1:]
    i = 0
    while i < len(argv):
        if argv[i] == "--arch" and i + 1 < len(argv):
            arch = argv[i + 1]
            i += 2
        elif argv[i] == "--format" and i + 1 < len(argv):
            fmt = argv[i + 1]
            i += 2
        elif argv[i] in ("-h", "--help"):
            print("Usage: run_rebuild_and_e2e.py [--arch amd64|arm64] [--format deb|rpm|both]")
            return 0
        else:
            print(f"Unknown arg: {argv[i]}", file=sys.stderr)
            return 1

    if arch not in ("amd64", "arm64"):
        print(f"Unsupported --arch {arch}", file=sys.stderr)
        return 1
    if fmt not in ("deb", "rpm", "both"):
        print(f"Unsupported --format {fmt}", file=sys.stderr)
        return 1

    if not shutil.which("dotnet") and (Path.home() / ".dotnet" / "dotnet").is_file():
        os.environ["PATH"] = str(Path.home() / ".dotnet") + os.pathsep + os.environ.get("PATH", "")
        os.environ.setdefault("DOTNET_SYSTEM_GLOBALIZATION_INVARIANT", "1")

    r = subprocess.run(
        ["git", "-C", str(root), "log", "-1", "--format=%ct"],
        capture_output=True,
        text=True,
    )
    if r.returncode == 0 and r.stdout.strip():
        os.environ["SOURCE_DATE_EPOCH"] = r.stdout.strip()

    chmod_scripts(root)

    py = sys.executable
    fw = root / "packaging" / "tests" / "e2e" / "docker_grpc_firewall.py"
    use_docker = docker_ok()

    def run_stage_host() -> None:
        subprocess.run(
            [py, str(root / "packaging" / "tools" / "stage.py"), "--arch", arch],
            cwd=root,
            check=True,
        )

    if fmt == "deb":
        if use_docker:
            build_deb_via_docker(root, arch)
        else:
            run_stage_host()
            run_build_deb_native(root, arch)
        subprocess.run([py, str(fw), "--arch", arch, "--format", "deb"], cwd=root, check=True)
    elif fmt == "rpm":
        if use_docker:
            build_rpm_via_docker(root, arch)
        else:
            run_stage_host()
            run_build_rpm_native(root, arch)
        subprocess.run([py, str(fw), "--arch", arch, "--format", "rpm"], cwd=root, check=True)
    else:
        if use_docker:
            build_deb_via_docker(root, arch)
            build_rpm_via_docker(root, arch)
        else:
            run_stage_host()
            run_build_deb_native(root, arch)
            run_build_rpm_native(root, arch)
        subprocess.run([py, str(fw), "--arch", arch, "--format", "deb"], cwd=root, check=True)
        subprocess.run([py, str(fw), "--arch", arch, "--format", "rpm"], cwd=root, check=True)

    print(f"Rebuild + E2E complete ({fmt}).")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
