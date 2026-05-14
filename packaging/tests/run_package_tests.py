#!/usr/bin/env python3
"""
Lint and quick validation for built packages (Linux CI or developer machine).
Replaces run-package-tests.sh for host/CI.
"""

from __future__ import annotations

import glob
import shutil
import subprocess
import sys
from pathlib import Path

from host_script_paths import repo_root_from_script


def latest_glob(pattern: str) -> Path | None:
    paths = [Path(p) for p in glob.glob(pattern)]
    if not paths:
        return None
    paths.sort(key=lambda p: p.stat().st_mtime, reverse=True)
    return paths[0]


def main() -> int:
    root = repo_root_from_script(Path(__file__))
    arch = sys.argv[1] if len(sys.argv) > 1 else "amd64"

    failed = 0

    vs = root / "packaging" / "tests" / "firewall" / "validate_static.py"
    if vs.is_file():
        r = subprocess.run([sys.executable, str(vs)])
        if r.returncode != 0:
            failed = 1

    out_deb = root / "packaging" / "out" / arch / "deb"
    latest_deb = latest_glob(str(out_deb / "*.deb"))
    if latest_deb is not None:
        if shutil.which("lintian"):
            r = subprocess.run(["lintian", "--fail-on", "error", str(latest_deb)])
            if r.returncode != 0:
                failed = 1
        else:
            print("lintian not installed; skipping DEB lint")
    else:
        print(f"No .deb in {out_deb}; run build-deb.sh first")

    rpmlint_cfg = root / "packaging" / "rpm" / "videodedupserver-rpmlint.toml"
    out_rpm = root / "packaging" / "out" / arch / "rpm"
    if shutil.which("rpmlint"):
        rpm_found = False
        if out_rpm.is_dir():
            for archdir in sorted(out_rpm.iterdir()):
                if not archdir.is_dir():
                    continue
                rpms = sorted(glob.glob(str(archdir / "*.rpm")))
                if not rpms:
                    continue
                rpm_found = True
                latest_rpm = max((Path(p) for p in rpms), key=lambda p: p.stat().st_mtime)
                cmd = ["rpmlint", str(latest_rpm)]
                if rpmlint_cfg.is_file():
                    cmd[1:1] = ["--config", str(rpmlint_cfg)]
                r = subprocess.run(cmd)
                if r.returncode != 0:
                    failed = 1
        if not rpm_found:
            print(f"No .rpm in {out_rpm}/*/ - run build-rpm.sh first")
    else:
        print("rpmlint not installed; skipping RPM lint")

    snaps = glob.glob(str(root / "packaging" / "out" / arch / "snap" / "*.snap"))
    if snaps:
        if shutil.which("review-tools"):
            r = subprocess.run(["review-tools.snap-review", *snaps])
            if r.returncode != 0:
                failed = 1
        else:
            print("review-tools not installed; skipping snap-review")

    pkgs = glob.glob(str(root / "packaging" / "out" / arch / "pacman" / "*.pkg.tar.zst"))
    if pkgs:
        if shutil.which("namcap"):
            r = subprocess.run(["namcap", *pkgs])
            if r.returncode != 0:
                failed = 1
        else:
            print("namcap not installed; skipping Arch package checks")

    return failed


if __name__ == "__main__":
    raise SystemExit(main())
