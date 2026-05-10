#!/bin/sh
# Add an nftables accept rule for VideoDedup gRPC (default TCP 51726) on inet filter input.
# If no ruleset is loaded yet (common on Debian), creates table inet filter + input hook chain
# with policy accept, then adds the rule. If you use ufw/firewalld, prefer their helpers instead.
#
# Persistence (optional): sudo strips environment variables by default — use either:
#   sudo /usr/lib/videodedupserver/firewall/open-port-nftables.sh --persist
#   sudo env PERSIST=1 /usr/lib/videodedupserver/firewall/open-port-nftables.sh
# Also: VIDEODEDUP_FIREWALL_PERSIST=1. Refuses if ufw is active unless VIDEODEDUP_NFT_PERSIST_FORCE=1.
set -eu

PORT="${PORT:-51726}"

for arg in "$@"; do
  case "${arg}" in
    --persist) PERSIST=1 ;;
    --help|-h)
      echo "Usage: $0 [--persist]" >&2
      echo "  Port: PORT=51726 (default). Persistence: --persist or env PERSIST=1 (use env with sudo)." >&2
      exit 0
      ;;
  esac
done

if [ "$(id -u)" -ne 0 ]; then
  echo "Run as root, e.g.: sudo $0" >&2
  exit 1
fi

if ! command -v nft >/dev/null 2>&1; then
  echo "nft not found. Install nftables (apt install nftables)." >&2
  exit 1
fi

RULE_LINE="tcp dport ${PORT} accept"
INSTALLED_SCRIPT="/usr/lib/videodedupserver/firewall/open-port-nftables.sh"

wants_persist() {
  case "${PERSIST:-}" in 1|yes|true|YES|TRUE) return 0 ;; esac
  case "${VIDEODEDUP_FIREWALL_PERSIST:-}" in 1|yes|true|YES|TRUE) return 0 ;; esac
  return 1
}

print_persist_howto() {
  echo "nftables: current rules are live in the kernel only — they are lost on reboot until saved." >&2
  echo "nftables: To write the live ruleset to /etc/nftables.conf and enable nftables.service, run:" >&2
  echo "  sudo ${INSTALLED_SCRIPT} --persist" >&2
  echo "  or: sudo env PERSIST=1 ${INSTALLED_SCRIPT}" >&2
  echo "nftables: (Plain \"sudo PERSIST=1 ...\" often fails because sudo drops PERSIST.)" >&2
  echo "nftables: Docs: /usr/share/doc/videodedupserver/README.firewall" >&2
}

persist_ruleset_to_disk() {
  if command -v ufw >/dev/null 2>&1; then
    if ufw status 2>/dev/null | grep -qi "Status: active"; then
      if [ "${VIDEODEDUP_NFT_PERSIST_FORCE:-}" != "1" ]; then
        echo "nftables: ufw is active — not writing /etc/nftables.conf (avoids fighting ufw)." >&2
        echo "nftables: use open-port-ufw.sh instead, or set VIDEODEDUP_NFT_PERSIST_FORCE=1 if you insist." >&2
        return 1
      fi
    fi
  fi

  conf="/etc/nftables.conf"
  if [ -f "${conf}" ] && [ -s "${conf}" ]; then
    bak="${conf}.bak.videodedup.$(date +%Y%m%d%H%M%S)"
    cp -a "${conf}" "${bak}" || { echo "nftables: could not backup ${conf}" >&2; return 1; }
    echo "nftables: backed up ${conf} to ${bak}"
  fi

  tmp="${conf}.new.$$"
  nft list ruleset > "${tmp}" || { rm -f "${tmp}"; return 1; }
  mv "${tmp}" "${conf}"
  echo "nftables: wrote live ruleset to ${conf}"

  if command -v systemctl >/dev/null 2>&1; then
    systemctl enable nftables.service 2>/dev/null || true
    if systemctl restart nftables.service 2>/dev/null; then
      echo "nftables: restarted nftables.service"
    else
      systemctl reload nftables.service 2>/dev/null || true
      echo "nftables: enabled nftables.service; restart manually if needed: systemctl restart nftables"
    fi
  fi
  echo "nftables: persistence complete (reboot-safe if nftables.service is enabled)."
}

print_manual_hint() {
  echo "nftables: automatic setup failed. Add a rule manually, for example:" >&2
  echo "  table inet filter {" >&2
  echo "    chain input {" >&2
  echo "      type filter hook input priority filter; policy accept;" >&2
  echo "      ${RULE_LINE}" >&2
  echo "    }" >&2
  echo "  }" >&2
}

rule_already_present() {
  nft list chain inet filter input 2>/dev/null | grep -qF "dport ${PORT}"
}

# Debian often has nft installed but no inet table until nftables.service loads a file — create a minimal hook chain.
if ! nft list table inet filter >/dev/null 2>&1; then
  nft add table inet filter || { print_manual_hint; exit 1; }
fi

if ! nft list chain inet filter input >/dev/null 2>&1; then
  if ! nft add chain inet filter input '{ type filter hook input priority filter; policy accept; }'; then
    echo "nftables: could not add chain inet filter input (another tool may own the input hook)." >&2
    print_manual_hint
    exit 1
  fi
  echo "nftables: created chain inet filter input (policy accept). Use 'nft list ruleset' to review."
fi

if rule_already_present; then
  echo "nftables: tcp port ${PORT} is already allowed in inet filter input (live ruleset)."
  if wants_persist; then
    echo "nftables: saving the current live ruleset to disk (--persist) …" >&2
    persist_ruleset_to_disk || exit 1
  else
    print_persist_howto
  fi
  exit 0
fi

nft add rule inet filter input ${RULE_LINE}
echo "nftables: added live rule to inet filter input: ${RULE_LINE}"

if wants_persist; then
  persist_ruleset_to_disk || exit 1
else
  print_persist_howto
fi
