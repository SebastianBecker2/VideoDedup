#!/usr/bin/env bash
# Syntax + CRLF checks for packaging/common/firewall/*.sh (no Docker).
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../../.." && pwd)"
FW="${ROOT}/packaging/common/firewall"

for s in open-port-ufw.sh open-port-firewalld.sh open-port-iptables.sh open-port-nftables.sh; do
  f="${FW}/${s}"
  [[ -f "${f}" ]] || { echo "missing ${f}" >&2; exit 1; }
  sh -n "${f}"
  if grep -q $'\r' "${f}" 2>/dev/null; then
    echo "CRLF in ${f} — use LF only for shell scripts" >&2
    exit 1
  fi
done

cf="${FW}/configure-firewall-interactive.sh"
[[ -f "${cf}" ]] || { echo "missing ${cf}" >&2; exit 1; }
bash -n "${cf}"
if grep -q $'\r' "${cf}" 2>/dev/null; then
  echo "CRLF in ${cf} — use LF only for shell scripts" >&2
  exit 1
fi

[[ -f "${FW}/README.firewall" ]] || { echo "missing README.firewall" >&2; exit 1; }

echo "firewall static validation OK (${FW})"
