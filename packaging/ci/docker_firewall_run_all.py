#!/usr/bin/env python3
"""
Firewall helper checks: static (validate_static.py) + optional Docker integration.
Python port of packaging/tests/firewall/docker-run-all.sh (no host bash required).
"""

from __future__ import annotations

import argparse
import os
import subprocess
import sys
from pathlib import Path

_REPO_ROOT = Path(__file__).resolve().parents[2]
if str(_REPO_ROOT) not in sys.path:
    sys.path.insert(0, str(_REPO_ROOT))

from packaging.ci.docker_paths import docker_ok, docker_host_path, repo_path_for_docker_bind


def repo_root() -> Path:
    return _REPO_ROOT


def firewall_dir(root: Path) -> Path:
    return root / "packaging/common/firewall"


def fw_volume_path(root: Path) -> str:
    fw = firewall_dir(root)
    host_fw = repo_path_for_docker_bind(root, fw)
    return docker_host_path(host_fw)


def _bash_stdin(script: str) -> str:
    """Strip CR so bash -s inside Linux containers never sees CRLF from a Windows-checked-out repo."""
    s = script.replace("\r\n", "\n").replace("\r", "\n")
    return s


def run_docker_bash_stdin(title: str, image: str, script: str) -> None:
    print(title, flush=True)
    payload = _bash_stdin(script).encode("utf-8")
    r = subprocess.run(
        [
            "docker",
            "run",
            "--rm",
            "-i",
            "--privileged",
            "-v",
            f"{fw_volume_path(repo_root())}:/fw:ro",
            image,
            "bash",
            "-s",
        ],
        input=payload,
    )
    if r.returncode != 0:
        raise SystemExit(r.returncode)


SCRIPT_NFT = r"""set -eu
export DEBIAN_FRONTEND=noninteractive
apt-get update -qq
apt-get install -y -qq nftables >/dev/null
nft flush ruleset
install -m 0755 /fw/open-port-nftables.sh /tmp/open-port-nftables.sh
/tmp/open-port-nftables.sh
nft list chain inet filter input | grep -q "tcp dport 51726 accept" \
  || { nft list ruleset; exit 1; }
/tmp/open-port-nftables.sh --persist
test -s /etc/nftables.conf
grep -qF "51726" /etc/nftables.conf
echo "nftables integration OK"
"""

SCRIPT_IPTABLES = r"""set -eu
export DEBIAN_FRONTEND=noninteractive
apt-get update -qq
apt-get install -y -qq iptables iptables-persistent >/dev/null
install -m 0755 /fw/open-port-iptables.sh /tmp/open-port-iptables.sh
/tmp/open-port-iptables.sh
iptables -C INPUT -p tcp --dport 51726 -j ACCEPT
/tmp/open-port-iptables.sh --persist
test -f /etc/iptables/rules.v4
grep -qF "51726" /etc/iptables/rules.v4
echo "iptables integration OK"
"""

SCRIPT_UFW = r"""set -eu
export DEBIAN_FRONTEND=noninteractive
apt-get update -qq
apt-get install -y -qq ufw >/dev/null
install -m 0755 /fw/open-port-ufw.sh /tmp/open-port-ufw.sh
/tmp/open-port-ufw.sh
if ! grep -qE "51726|dport 51726" /etc/ufw/user.rules 2>/dev/null; then
  echo "ufw: expected rule not found in user.rules" >&2
  cat /etc/ufw/user.rules >&2 || true
  exit 1
fi
echo "ufw integration OK"
"""

SCRIPT_FIREWALLD = r"""set -eu
dnf install -y -q firewalld >/dev/null
install -m 0755 /fw/open-port-firewalld.sh /tmp/open-port-firewalld.sh
firewall-offline-cmd --add-port=51726/tcp >/dev/null
firewall-offline-cmd --list-ports | grep -q "51726/tcp" \
  || { firewall-offline-cmd --list-ports; exit 1; }
grep -q "firewall-cmd" /tmp/open-port-firewalld.sh
grep -q "add-port" /tmp/open-port-firewalld.sh
echo "firewalld (offline list-ports) OK - full firewall-cmd path needs a running daemon on real hosts"
"""


def main() -> int:
    root = repo_root()
    here = root / "packaging/tests/firewall"
    ap = argparse.ArgumentParser(description="Firewall static + optional Docker integration checks.")
    ap.add_argument(
        "--integration",
        action="store_true",
        help="Run Docker integration tests (requires privileged docker).",
    )
    args = ap.parse_args()

    r = subprocess.run([sys.executable, str(here / "validate_static.py")], cwd=root)
    if r.returncode != 0:
        return r.returncode

    if not args.integration:
        print("Skipping Docker integration (pass --integration to run).")
        return 0

    if not docker_ok():
        print("docker not found or daemon not reachable; install/start Docker or omit --integration", file=sys.stderr)
        return 1

    os.chdir(root)
    run_docker_bash_stdin(
        "=== Docker: nftables (empty ruleset - script creates inet filter input, Debian) ===",
        "debian:bookworm-slim",
        SCRIPT_NFT,
    )
    run_docker_bash_stdin(
        "=== Docker: iptables (Debian, live rule + --persist file) ===",
        "debian:bookworm-slim",
        SCRIPT_IPTABLES,
    )
    run_docker_bash_stdin(
        "=== Docker: ufw (Ubuntu) ===",
        "ubuntu:22.04",
        SCRIPT_UFW,
    )
    run_docker_bash_stdin(
        "=== Docker: firewalld (Fedora, offline permanent config) ===",
        "fedora:40",
        SCRIPT_FIREWALLD,
    )
    print("All firewall Docker integration checks passed.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
