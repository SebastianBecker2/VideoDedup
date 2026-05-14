"""Shared helpers for Docker gRPC E2E drivers (no MSYS; use Python + subprocess)."""

from __future__ import annotations

import os
import shutil
import subprocess
import sys
import tempfile
from pathlib import Path


def repo_root_from_e2e_dir(e2e_dir: Path) -> Path:
    """e2e_dir is packaging/tests/e2e (directory containing this file)."""
    return e2e_dir.resolve().parent.parent.parent


def docker_host_path(path: Path | str) -> str:
    """
    Path string for Docker bind mounts and for Windows dotnet loading the DLL.
    Uses cygpath -w when available (Git Bash); otherwise resolved native path.
    """
    p = Path(path).resolve()
    cygpath = shutil.which("cygpath")
    if cygpath and os.name == "nt":
        r = subprocess.run(
            [cygpath, "-w", str(p)],
            capture_output=True,
            text=True,
            check=False,
        )
        if r.returncode == 0 and r.stdout.strip():
            return r.stdout.strip()
    return str(p)


def which(name: str) -> str | None:
    return shutil.which(name)


def run(
    cmd: list[str],
    *,
    cwd: Path | None = None,
    env=None,
    check: bool = True,
) -> None:
    subprocess.run(cmd, cwd=cwd, env=env, check=check)


def run_capture(cmd: list[str], *, cwd: Path | None = None, text: bool = True) -> subprocess.CompletedProcess[str]:
    return subprocess.run(cmd, cwd=cwd, capture_output=True, text=text, check=False)


def docker_ok() -> bool:
    if not which("docker"):
        return False
    return subprocess.run(["docker", "info"], capture_output=True).returncode == 0


def mktemp_dir_under(parent: Path, prefix: str) -> Path:
    parent.mkdir(parents=True, exist_ok=True)
    return Path(tempfile.mkdtemp(prefix=prefix, dir=str(parent)))


def latest_artifact(glob_pattern: str) -> Path | None:
    from glob import glob

    paths = [Path(p) for p in glob(glob_pattern)]
    if not paths:
        return None
    paths.sort(key=lambda p: p.stat().st_mtime, reverse=True)
    return paths[0]


def publish_dotnet_project(
    root: Path,
    rel_csproj: str,
    out_dir: Path,
    rid: str,
) -> None:
    """Publish a single csproj to out_dir (host dotnet, or Docker SDK if dotnet missing)."""
    out_dir.mkdir(parents=True, exist_ok=True)
    csproj = root / rel_csproj
    if which("dotnet"):
        run(
            [
                "dotnet",
                "publish",
                str(csproj),
                "-c",
                "Release",
                "-r",
                rid,
                "--self-contained",
                "false",
                "-o",
                str(out_dir),
            ]
        )
        return
    print(f"dotnet not on PATH; publishing {rel_csproj} via mcr.microsoft.com/dotnet/sdk:8.0 ...", file=sys.stderr)
    root_vol = docker_host_path(root)
    rel_posix = rel_csproj.replace("\\", "/")
    out_posix = str(out_dir.relative_to(root)).replace("\\", "/")
    run(
        [
            "docker",
            "run",
            "--rm",
            "-v",
            f"{root_vol}:/src:rw",
            "-w",
            "/src",
            "mcr.microsoft.com/dotnet/sdk:8.0",
            "dotnet",
            "publish",
            rel_posix,
            "-c",
            "Release",
            "-r",
            rid,
            "--self-contained",
            "false",
            "-o",
            out_posix,
        ]
    )
