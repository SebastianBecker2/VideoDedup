#!/usr/bin/env bash
# Run firewall helper validation: static checks locally, optional Docker integration tests.
#
# Usage:
#   ./packaging/tests/firewall/docker-run-all.sh           # static only (no Docker)
#   ./packaging/tests/firewall/docker-run-all.sh --integration   # + Docker (needs docker, Linux daemon)
#
# On Windows: use WSL2 from repo root, or Docker Desktop with Linux engine; integration
# uses --privileged so Docker must be running.
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../../.." && pwd)"
FW="${ROOT}/packaging/common/firewall"
HERE="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

INTEGRATION=0
for a in "$@"; do
  case "${a}" in
    --integration) INTEGRATION=1 ;;
    -h|--help)
      echo "Usage: $0 [--integration]"
      exit 0
      ;;
    *) echo "Unknown arg: ${a}" >&2; exit 1 ;;
  esac
done

"${HERE}/validate-static.sh"

if [[ "${INTEGRATION}" -eq 0 ]]; then
  echo "Skipping Docker integration (pass --integration to run)."
  exit 0
fi

if ! command -v docker >/dev/null 2>&1; then
  echo "docker not found; install Docker or run without --integration" >&2
  exit 1
fi

if ! docker info >/dev/null 2>&1; then
  echo "Docker daemon not reachable (start Docker Desktop / docker service)" >&2
  exit 1
fi

echo "=== Docker: nftables (empty ruleset — script creates inet filter input, Debian) ==="
docker run --rm -i --privileged -v "${FW}:/fw:ro" debian:bookworm-slim bash -s <<'EOS'
set -eu
export DEBIAN_FRONTEND=noninteractive
apt-get update -qq
apt-get install -y -qq nftables >/dev/null
nft flush ruleset
install -m 0755 /fw/open-port-nftables.sh /tmp/open-port-nftables.sh
/tmp/open-port-nftables.sh
nft list chain inet filter input | grep -q "tcp dport 51726 accept" \
  || { nft list ruleset; exit 1; }
# Rule already present: --persist must still write /etc/nftables.conf (sudo/env issue covered by --persist).
/tmp/open-port-nftables.sh --persist
test -s /etc/nftables.conf
grep -qF "51726" /etc/nftables.conf
echo "nftables integration OK"
EOS

echo "=== Docker: iptables (Debian, live rule + --persist file) ==="
docker run --rm -i --privileged -v "${FW}:/fw:ro" debian:bookworm-slim bash -s <<'EOS'
set -eu
export DEBIAN_FRONTEND=noninteractive
apt-get update -qq
apt-get install -y -qq iptables iptables-persistent >/dev/null
install -m 0755 /fw/open-port-iptables.sh /tmp/open-port-iptables.sh
/tmp/open-port-iptables.sh
iptables -C INPUT -p tcp --dport 51726 -j ACCEPT
# Rule already present: --persist must still run netfilter-persistent save.
/tmp/open-port-iptables.sh --persist
test -f /etc/iptables/rules.v4
grep -qF "51726" /etc/iptables/rules.v4
echo "iptables integration OK"
EOS

echo "=== Docker: ufw (Ubuntu) ==="
docker run --rm -i --privileged -v "${FW}:/fw:ro" ubuntu:22.04 bash -s <<'EOS'
set -eu
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
EOS

echo "=== Docker: firewalld (Fedora, offline permanent config) ==="
docker run --rm -i --privileged -v "${FW}:/fw:ro" fedora:40 bash -s <<'EOS'
set -eu
dnf install -y -q firewalld >/dev/null
install -m 0755 /fw/open-port-firewalld.sh /tmp/open-port-firewalld.sh
firewall-offline-cmd --add-port=51726/tcp >/dev/null
firewall-offline-cmd --list-ports | grep -q "51726/tcp" \
  || { firewall-offline-cmd --list-ports; exit 1; }
grep -q "firewall-cmd" /tmp/open-port-firewalld.sh
grep -q "add-port" /tmp/open-port-firewalld.sh
echo "firewalld (offline list-ports) OK — full firewall-cmd path needs a running daemon on real hosts"
EOS

echo "All firewall Docker integration checks passed."
