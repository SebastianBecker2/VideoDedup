"""Build .deb via Dockerfile.build-deb (Docker BuildKit local output)."""

from __future__ import annotations

import os
import shutil
import subprocess
import tempfile
from pathlib import Path


def build_deb_via_docker(repo_root: Path, arch: str) -> None:
    """Run packaging/docker/Dockerfile.build-deb and copy the single .deb into packaging/out/<arch>/deb/."""
    dockerfile = repo_root / "packaging/docker/Dockerfile.build-deb"
    if not dockerfile.is_file():
        raise FileNotFoundError(dockerfile)
    out_tmp = tempfile.mkdtemp(prefix="vd-deb-docker.", dir=str(repo_root / "packaging/out"))
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
            raise RuntimeError("Docker build of .deb failed.")
        debs = list(out_tmp_path.glob("*.deb"))
        if len(debs) != 1:
            raise RuntimeError(
                f"Docker build produced {len(debs)} .deb file(s) in output directory (expected 1)."
            )
        deb_out = repo_root / "packaging/out" / arch / "deb"
        deb_out.mkdir(parents=True, exist_ok=True)
        dest = deb_out / debs[0].name
        shutil.copy2(debs[0], dest)
        print(f"Wrote {dest}")
    finally:
        shutil.rmtree(out_tmp_path, ignore_errors=True)
