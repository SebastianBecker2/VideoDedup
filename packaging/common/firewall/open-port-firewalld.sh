#!/bin/sh
# Open the VideoDedup gRPC port in firewalld (default 51726/tcp, permanent).
set -eu

PORT="${PORT:-51726}"

if [ "$(id -u)" -ne 0 ]; then
  echo "Run as root, e.g.: sudo $0" >&2
  exit 1
fi

if ! command -v firewall-cmd >/dev/null 2>&1; then
  echo "firewall-cmd not found. Install firewalld for your distribution." >&2
  exit 1
fi

if ! firewall-cmd --state >/dev/null 2>&1; then
  echo "firewalld is not running. Start it first, e.g.: systemctl start firewalld" >&2
  exit 1
fi

firewall-cmd --permanent "--add-port=${PORT}/tcp"
firewall-cmd --reload

echo "firewalld: added permanent TCP port ${PORT} (VideoDedup gRPC)."
