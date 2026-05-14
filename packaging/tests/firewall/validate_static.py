#!/usr/bin/env python3
"""
Syntax + CRLF checks for packaging/common/firewall/*.sh (no Docker).
Replaces validate-static.sh for host/CI.

On Windows, sh/bash -n is skipped (paths and System32 bash are unreliable); CRLF and file checks still run.
Linux CI runs full syntax checks.
"""

from __future__ import annotations

import os
import shutil
import subprocess
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent.parent))
from host_script_paths import repo_root_from_script  # noqa: E402


def have_sh() -> str | None:
    return shutil.which("sh") or shutil.which("bash")


def main() -> int:
    root = repo_root_from_script(Path(__file__))
    fw = root / "packaging" / "common" / "firewall"
    run_syntax = os.name != "nt"
    if run_syntax and not have_sh():
        print("sh/bash not found; skipping shell syntax checks", file=sys.stderr)
        run_syntax = False
    sh_exe = have_sh() if run_syntax else None

    for name in (
        "open-port-ufw.sh",
        "open-port-firewalld.sh",
        "open-port-iptables.sh",
        "open-port-nftables.sh",
    ):
        f = fw / name
        if not f.is_file():
            print(f"missing {f}", file=sys.stderr)
            return 1
        if sh_exe:
            r = subprocess.run([sh_exe, "-n", str(f)], capture_output=True)
            if r.returncode != 0:
                print(f"{sh_exe} -n failed: {f}", file=sys.stderr)
                return 1
        data = f.read_bytes()
        if b"\r" in data:
            print(f"CRLF in {f} - use LF only for shell scripts", file=sys.stderr)
            return 1

    cf = fw / "configure-firewall-interactive.sh"
    if not cf.is_file():
        print(f"missing {cf}", file=sys.stderr)
        return 1
    bash_exe = shutil.which("bash") if run_syntax else None
    if bash_exe:
        r = subprocess.run([bash_exe, "-n", str(cf)], capture_output=True)
        if r.returncode != 0:
            print(f"bash -n failed: {cf}", file=sys.stderr)
            return 1
    if b"\r" in cf.read_bytes():
        print(f"CRLF in {cf} - use LF only for shell scripts", file=sys.stderr)
        return 1

    readme = fw / "README.firewall"
    if not readme.is_file():
        print(f"missing {readme}", file=sys.stderr)
        return 1

    print(f"firewall static validation OK ({fw})")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
