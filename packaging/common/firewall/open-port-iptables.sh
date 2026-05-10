#!/bin/sh
# Insert an IPv4 iptables ACCEPT rule for the VideoDedup gRPC port (default 51726/tcp).
# Live rule by default. Persistence: --persist or env PERSIST=1 (use env with sudo).
set -eu

PORT="${PORT:-51726}"

for arg in "$@"; do
  case "${arg}" in
    --persist) PERSIST=1 ;;
    --help|-h)
      echo "Usage: $0 [--persist]" >&2
      echo "  Port: PORT=51726. With sudo use: sudo $0 --persist  or  sudo env PERSIST=1 $0" >&2
      exit 0
      ;;
  esac
done

if [ "$(id -u)" -ne 0 ]; then
  echo "Run as root, e.g.: sudo $0" >&2
  exit 1
fi

if ! command -v iptables >/dev/null 2>&1; then
  echo "iptables not found." >&2
  exit 1
fi

RULE_SPEC="-p tcp --dport ${PORT} -j ACCEPT"
INSTALLED_SCRIPT="/usr/lib/videodedupserver/firewall/open-port-iptables.sh"

wants_persist() {
  case "${PERSIST:-}" in 1|yes|true|YES|TRUE) return 0 ;; esac
  case "${VIDEODEDUP_FIREWALL_PERSIST:-}" in 1|yes|true|YES|TRUE) return 0 ;; esac
  return 1
}

print_persist_howto() {
  echo "iptables: rule is live only until reboot unless saved (e.g. netfilter-persistent)." >&2
  echo "iptables: To save current iptables rules after installing iptables-persistent:" >&2
  echo "  sudo ${INSTALLED_SCRIPT} --persist" >&2
  echo "  or: sudo env PERSIST=1 ${INSTALLED_SCRIPT}" >&2
  echo "iptables: Docs: /usr/share/doc/videodedupserver/README.firewall" >&2
}

persist_iptables() {
  if command -v netfilter-persistent >/dev/null 2>&1; then
    netfilter-persistent save
    echo "iptables: netfilter-persistent save OK (reboot-safe if service is enabled)"
    return 0
  fi
  echo "iptables: persistence needs netfilter-persistent (apt install iptables-persistent)" >&2
  return 1
}

if iptables -C INPUT ${RULE_SPEC} 2>/dev/null; then
  echo "iptables: ACCEPT rule for TCP ${PORT} is already present in INPUT."
  if wants_persist; then
    echo "iptables: saving rules to disk (--persist) …" >&2
    persist_iptables || exit 1
  else
    print_persist_howto
  fi
  exit 0
fi

iptables -I INPUT 1 ${RULE_SPEC}

echo "iptables: inserted ACCEPT for TCP ${PORT} at top of INPUT chain."

if wants_persist; then
  persist_iptables || exit 1
else
  print_persist_howto
fi
