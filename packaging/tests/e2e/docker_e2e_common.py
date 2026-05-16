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


TLS_CERT_SETUP_MARKER = "cert-setup/generate-server-cert.sh"


def strip_crlf_bytes(data: bytes) -> bytes:
    if b"\r\n" in data:
        return data.replace(b"\r\n", b"\n")
    if b"\r" in data:
        return data.replace(b"\r", b"")
    return data


def file_has_crlf(path: Path) -> bool:
    if not path.is_file():
        return False
    data = path.read_bytes()
    return b"\r\n" in data or b"\r" in data


def deb_package_has_tls_support(deb: Path) -> bool:
    """True if .deb includes cert-setup scripts from the TLS packaging work."""
    if which("dpkg-deb"):
        r = run_capture(["dpkg-deb", "-c", str(deb)])
        if r.returncode == 0 and TLS_CERT_SETUP_MARKER in (r.stdout or ""):
            return True
    if docker_ok():
        deb_vol = docker_host_path(deb)
        r = run_capture(
            [
                "docker",
                "run",
                "--rm",
                "-v",
                f"{deb_vol}:/p.deb:ro",
                "debian:trixie-slim",
                "dpkg-deb",
                "-c",
                "/p.deb",
            ]
        )
        if r.returncode == 0 and TLS_CERT_SETUP_MARKER in (r.stdout or ""):
            return True
    return False


def rpm_package_has_tls_support(rpm: Path) -> bool:
    """True if .rpm includes cert-setup scripts from the TLS packaging work."""
    if which("rpm"):
        r = run_capture(["rpm", "-qpl", str(rpm)])
        if r.returncode == 0 and TLS_CERT_SETUP_MARKER in (r.stdout or ""):
            return True
    if docker_ok():
        rpm_vol = docker_host_path(rpm)
        r = run_capture(
            [
                "docker",
                "run",
                "--rm",
                "-v",
                f"{rpm_vol}:/p.rpm:ro",
                "fedora:40",
                "rpm",
                "-qpl",
                "/p.rpm",
            ]
        )
        if r.returncode == 0 and TLS_CERT_SETUP_MARKER in (r.stdout or ""):
            return True
    return False


def latest_artifact(glob_pattern: str) -> Path | None:
    from glob import glob

    paths = [Path(p) for p in glob(glob_pattern)]
    if not paths:
        return None
    paths.sort(key=lambda p: p.stat().st_mtime, reverse=True)
    return paths[0]


def server_cert_path_in_container(package_format: str) -> str:
    """Path to VideoDedup.crt inside the server container (after install / first-run)."""
    fmt = (package_format or "deb").strip().lower()
    if fmt == "flatpak":
        # flatpak run as videodedup (HOME=/var/lib/videodedupserver); not root's ~/.var
        return (
            "/var/lib/videodedupserver/.var/app/"
            "io.github.sebastianbecker2.videodedup.server/data/cert/VideoDedup.crt"
        )
    if fmt == "snap":
        return "/tmp/vd-snap-common/cert/VideoDedup.crt"
    return "/usr/lib/videodedupserver/cert/VideoDedup.crt"


def extract_server_cert(
    container: str,
    host_out: Path,
    *,
    package_format: str = "deb",
    cert_in_container: str | None = None,
) -> Path:
    """Copy VideoDedup.crt from server container to host_out/VideoDedup.crt."""
    host_out.mkdir(parents=True, exist_ok=True)
    dest = host_out / "VideoDedup.crt"
    inner = cert_in_container or server_cert_path_in_container(package_format)
    r = subprocess.run(
        ["docker", "cp", f"{container}:{inner}", docker_host_path(dest)],
        capture_output=True,
        text=True,
    )
    if r.returncode != 0:
        err = (r.stderr or r.stdout or "").strip()
        raise RuntimeError(f"docker cp server cert ({inner}) failed ({r.returncode}): {err}")
    if not dest.is_file():
        raise RuntimeError(f"extract_server_cert: missing {dest}")
    return dest


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
