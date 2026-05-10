#!/usr/bin/env bash
# Interactive helper: detect firewall stacks, show status, recommend one, run the matching open-port-*.sh.
# Run as root: sudo /usr/lib/videodedupserver/firewall/configure-firewall-interactive.sh
#
# Optional: PORT=51726 (default 51726).
set -euo pipefail
shopt -s extglob

PORT="${PORT:-51726}"

SCRIPT_PATH="${BASH_SOURCE[0]}"
SCRIPT_DIR="$(CDPATH= cd -- "$(dirname "${SCRIPT_PATH}")" && pwd)"

usage() {
  echo "Usage: sudo $0" >&2
  echo "  Optional: PORT=51726" >&2
  exit "${1:-0}"
}

for arg in "$@"; do
  case "${arg}" in
    -h|--help) usage 0 ;;
    *) echo "Unknown option: ${arg}" >&2; usage 1 ;;
  esac
done

if [ "$(id -u)" -ne 0 ]; then
  echo "Run as root, e.g.: sudo $0" >&2
  exit 1
fi

have_ufw() { command -v ufw >/dev/null 2>&1; }
ufw_active() {
  have_ufw || return 1
  ufw status 2>/dev/null | grep -qi 'Status: active'
}

have_firewalld_cmd() { command -v firewall-cmd >/dev/null 2>&1; }
firewalld_running() {
  have_firewalld_cmd || return 1
  firewall-cmd --state >/dev/null 2>&1
}

have_nft() { command -v nft >/dev/null 2>&1; }
have_iptables() { command -v iptables >/dev/null 2>&1; }

pick_recommended() {
  if firewalld_running; then
    echo firewalld
    return
  fi
  if ufw_active; then
    echo ufw
    return
  fi
  if have_nft; then
    echo nftables
    return
  fi
  if have_ufw; then
    echo ufw
    return
  fi
  if have_iptables; then
    echo iptables
    return
  fi
  if have_firewalld_cmd; then
    echo firewalld
    return
  fi
  echo ''
}

recommendation_reason() {
  local r="$1"
  case "${r}" in
    firewalld)
      echo "firewalld is running - use it so you do not bypass the distro firewall daemon."
      ;;
    ufw)
      if ufw_active; then
        echo "UFW is active - use the UFW helper instead of raw nftables/iptables."
      else
        echo "UFW is installed (not active) - typical on Ubuntu; good default if you use UFW."
      fi
      ;;
    nftables)
      echo "nftables is available and no ufw/firewalld is active - common on Debian and minimal servers."
      ;;
    iptables)
      echo "Only legacy iptables matched - use if your host still filters with iptables only."
      ;;
    *)
      echo "Could not pick a default; choose the stack that actually filters traffic on this host."
      ;;
  esac
}

mark_ufw() {
  if ! have_ufw; then
    printf '%s' "[ ] not installed"
    return
  fi
  if ufw_active; then
    printf '%s' "[*] installed, ACTIVE"
  else
    printf '%s' "[*] installed, not active"
  fi
}

mark_firewalld() {
  if ! have_firewalld_cmd; then
    printf '%s' "[ ] not installed"
    return
  fi
  if firewalld_running; then
    printf '%s' "[*] installed, RUNNING"
  else
    printf '%s' "[*] installed, not running"
  fi
}

mark_nft() {
  if have_nft; then
    printf '%s' "[*] installed"
  else
    printf '%s' "[ ] not installed"
  fi
}

mark_ipt() {
  if have_iptables; then
    printf '%s' "[*] installed"
  else
    printf '%s' "[ ] not installed"
  fi
}

export PORT

rec="$(pick_recommended)"
row_note() { [ "$1" = "${rec}" ] && printf '%s' '  <- suggested' || true; }
echo ""
echo "VideoDedup server - open TCP ${PORT} (gRPC) in the host firewall"
echo "----------------------------------------------------------------"
printf '  %-12s  %s%s\n' "ufw" "$(mark_ufw)" "$(row_note ufw)"
printf '  %-12s  %s%s\n' "firewalld" "$(mark_firewalld)" "$(row_note firewalld)"
printf '  %-12s  %s%s\n' "nftables" "$(mark_nft)" "$(row_note nftables)"
printf '  %-12s  %s%s\n' "iptables" "$(mark_ipt)" "$(row_note iptables)"
echo "----------------------------------------------------------------"
if [ -n "${rec}" ]; then
  echo "Suggested backend: ${rec}"
  echo "  ($(recommendation_reason "${rec}"))"
else
  echo "Suggested backend: (none detected - install a firewall tool or pick by hand.)"
fi
echo ""
echo "Individual scripts (unchanged) are still in:"
echo "  ${SCRIPT_DIR}/"
echo ""
echo "Choose action:"
suggest_num=""
case "${rec}" in
  ufw) suggest_num=1 ;;
  firewalld) suggest_num=2 ;;
  nftables) suggest_num=3 ;;
  iptables) suggest_num=4 ;;
esac
mark_sug() { [ "$1" = "${suggest_num}" ] && printf '%s' '  <- suggested' || true; }
echo "  1) ufw$(mark_sug 1)"
echo "  2) firewalld$(mark_sug 2)"
echo "  3) nftables$(mark_sug 3)"
echo "  4) iptables$(mark_sug 4)"
echo "  0) Exit without changes"
echo ""

if [ -n "${suggest_num}" ]; then
  printf 'Enter 0-4 [default %s for %s]: ' "${suggest_num}" "${rec}"
else
  printf 'Enter 0-4: '
fi
read -r choice_num || true
choice_num="${choice_num##+([[:space:]])}"
choice_num="${choice_num%%+([[:space:]])}"

map_choice() {
  case "$1" in
    0|'') echo none ;;
    1) echo ufw ;;
    2) echo firewalld ;;
    3) echo nftables ;;
    4) echo iptables ;;
    *)
      echo invalid
      ;;
  esac
}

# Default empty input -> recommended number if possible
if [ -z "${choice_num}" ] && [ -n "${rec}" ]; then
  case "${rec}" in
    ufw) choice_num=1 ;;
    firewalld) choice_num=2 ;;
    nftables) choice_num=3 ;;
    iptables) choice_num=4 ;;
  esac
  echo "(using suggested choice ${choice_num})"
fi

backend="$(map_choice "${choice_num}")"
if [ "${backend}" = invalid ]; then
  echo "Invalid choice." >&2
  exit 1
fi
if [ "${backend}" = none ]; then
  echo "Aborted."
  exit 0
fi

persist_ask='n'
case "${backend}" in
  ufw)
    echo ""
    echo "UFW: new rules are stored in UFW's own config (persistent across reboot when UFW is enabled)."
    ;;
  firewalld)
    echo ""
    echo "firewalld: this helper uses permanent zones; no extra persistence step."
    ;;
  nftables)
    echo ""
    echo "nftables: live rules are lost on reboot unless saved to /etc/nftables.conf (see README.firewall)."
    printf 'Save ruleset for reboot (--persist)? [y/N] '
    read -r persist_ask || true
    ;;
  iptables)
    echo ""
    echo "iptables: live rules are lost on reboot unless saved (e.g. netfilter-persistent)."
    printf 'Save IPv4 rules after adding the port (--persist)? [y/N] '
    read -r persist_ask || true
    ;;
esac

case "${persist_ask}" in
  y|Y|yes|YES) persist_ask=y ;;
  *) persist_ask=n ;;
esac

echo ""
echo "Running: backend=${backend} PORT=${PORT} persist=${persist_ask}"
echo ""

case "${backend}" in
  ufw)
    sh "${SCRIPT_DIR}/open-port-ufw.sh"
    ;;
  firewalld)
    sh "${SCRIPT_DIR}/open-port-firewalld.sh"
    ;;
  nftables)
    if [ "${persist_ask}" = y ]; then
      sh "${SCRIPT_DIR}/open-port-nftables.sh" --persist
    else
      sh "${SCRIPT_DIR}/open-port-nftables.sh"
    fi
    ;;
  iptables)
    if [ "${persist_ask}" = y ]; then
      sh "${SCRIPT_DIR}/open-port-iptables.sh" --persist
    else
      sh "${SCRIPT_DIR}/open-port-iptables.sh"
    fi
    ;;
esac

echo ""
echo "Done. Docs: /usr/share/doc/videodedupserver/README.firewall"
