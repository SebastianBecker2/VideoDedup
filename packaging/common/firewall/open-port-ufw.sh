#!/bin/sh
# Add a UFW allow rule for the VideoDedup gRPC port (default 51726/tcp).
set -eu

PORT="${PORT:-51726}"

if [ "$(id -u)" -ne 0 ]; then
  echo "Run as root, e.g.: sudo $0" >&2
  exit 1
fi

if ! command -v ufw >/dev/null 2>&1; then
  echo "ufw is not installed. On Debian/Ubuntu: apt install ufw" >&2
  exit 1
fi

ufw allow "${PORT}/tcp"

echo "UFW: allowed TCP ${PORT} (VideoDedup gRPC)."
echo "If UFW is disabled, enable when ready: ufw enable"
