"""Docker bind-mount paths and daemon checks (stdlib; usable without tests/e2e on sys.path)."""

from __future__ import annotations

import os
import shutil
import subprocess
from pathlib import Path


def docker_host_path(path: Path | str) -> str:
    """Path for Docker -v on Windows (cygpath -w when Git Bash cygpath exists)."""
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


def repo_path_for_docker_bind(repo_root: Path, host_path: Path) -> Path:
    """When nested docker uses VD_DOCKER_BIND_SRC, rewrite repo-prefixed paths for the host daemon."""
    bind = os.environ.get("VD_DOCKER_BIND_SRC", "").strip()
    if not bind:
        return host_path.resolve()
    try:
        host_r = host_path.resolve()
        root_r = repo_root.resolve()
        if root_r in host_r.parents or host_r == root_r:
            rel = host_r.relative_to(root_r)
            return Path(bind) / rel
    except ValueError:
        pass
    return host_path.resolve()


def docker_ok() -> bool:
    if not shutil.which("docker"):
        return False
    return subprocess.run(["docker", "info"], capture_output=True).returncode == 0
