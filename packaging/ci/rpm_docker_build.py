"""Build .rpm via packaging/docker/Dockerfile.build-rpm."""

from __future__ import annotations

import os
import shutil
import subprocess
import tempfile
from pathlib import Path


def _rpmarch(arch: str) -> str:
    if arch == "amd64":
        return "x86_64"
    if arch == "arm64":
        return "aarch64"
    raise ValueError(f"Unsupported arch: {arch}")


def build_rpm_via_docker(repo_root: Path, arch: str) -> None:
    dockerfile = repo_root / "packaging/docker/Dockerfile.build-rpm"
    if not dockerfile.is_file():
        raise FileNotFoundError(dockerfile)
    out_tmp = tempfile.mkdtemp(prefix="vd-rpm-docker.", dir=str(repo_root / "packaging/out"))
    out_tmp_path = Path(out_tmp)
    try:
        r = subprocess.run(
            [
                "docker",
                "build",
                "-f",
                str(dockerfile),
                "--build-arg",
                f"ARCH={arch}",
                "--target",
                "artifacts",
                "--output",
                f"type=local,dest={out_tmp_path}",
                str(repo_root),
            ],
            env={**os.environ, "DOCKER_BUILDKIT": "1"},
        )
        if r.returncode != 0:
            raise RuntimeError("Docker build of .rpm failed.")
        rpms = list(out_tmp_path.glob("*.rpm"))
        if not rpms:
            raise RuntimeError("Docker build produced no .rpm in output directory.")
        rpmarch = _rpmarch(arch)
        dest_dir = repo_root / "packaging/out" / arch / "rpm" / rpmarch
        dest_dir.mkdir(parents=True, exist_ok=True)
        for rpm in rpms:
            shutil.copy2(rpm, dest_dir / rpm.name)
            print(f"Wrote {dest_dir / rpm.name}")
    finally:
        shutil.rmtree(out_tmp_path, ignore_errors=True)
