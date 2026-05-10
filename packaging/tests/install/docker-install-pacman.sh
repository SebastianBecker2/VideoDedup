#!/usr/bin/env bash
# Smoke-test videodedupserver .pkg.tar.zst inside Arch (Docker): pacman -U, file checks, short process run.
#
# Usage:
#   ./packaging/tests/install/docker-install-pacman.sh [--arch amd64|arm64] [path/to.pkg.tar.zst]
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../../.." && pwd)"
ARCH="amd64"
PKG=""

while [[ $# -gt 0 ]]; do
  case "$1" in
    --arch)
      ARCH="$2"
      shift 2
      ;;
    -h|--help)
      echo "Usage: $0 [--arch amd64|arm64] [path/to/*.pkg.tar.zst]"
      exit 0
      ;;
    *)
      if [[ -n "${PKG}" ]]; then
        echo "Extra argument: $1" >&2
        exit 1
      fi
      PKG="$1"
      shift
      ;;
  esac
done

if [[ -z "${PKG}" ]]; then
  shopt -s nullglob
  candidates=( "${ROOT}/packaging/out/${ARCH}/pacman/"*.pkg.tar.zst )
  shopt -u nullglob
  if ((${#candidates[@]} == 0)); then
    echo "No .pkg.tar.zst in ${ROOT}/packaging/out/${ARCH}/pacman/ — build one first (packaging/tools/build-pacman.sh)" >&2
    exit 1
  fi
  PKG="$(ls -t "${candidates[@]}" | head -1)"
fi

if [[ ! -f "${PKG}" ]]; then
  echo "Not a file: ${PKG}" >&2
  exit 1
fi

PKG_ABS="$(cd "$(dirname "${PKG}")" && pwd)/$(basename "${PKG}")"

if ! command -v docker >/dev/null 2>&1; then
  echo "docker not found" >&2
  exit 1
fi
if ! docker info >/dev/null 2>&1; then
  echo "Docker daemon not reachable" >&2
  exit 1
fi

echo "Using ${PKG_ABS}"

docker run --rm \
  -v "${PKG_ABS}:/tmp/videodedupserver.pkg.tar.zst:ro" \
  archlinux:latest \
  bash -s <<'EOS'
set -eu
pacman-key --init 2>/dev/null || true
pacman-key --populate archlinux 2>/dev/null || true
pacman -Sy --noconfirm archlinux-keyring
pacman -Sy --noconfirm --needed iproute2
pacman -U --noconfirm /tmp/videodedupserver.pkg.tar.zst

test -x /usr/lib/videodedupserver/VideoDedupService
test -f /usr/lib/videodedupserver/appsettings.json
grep -q "51726" /usr/lib/videodedupserver/appsettings.json
test -f /usr/lib/videodedupserver/firewall/open-port-nftables.sh
id videodedup >/dev/null

runuser -u videodedup -- env \
  ASPNETCORE_ENVIRONMENT=Production \
  DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1 \
  VIDEODEDUP_APP_DATA=/var/lib/videodedupserver \
  timeout 25s /usr/lib/videodedupserver/VideoDedupService 2>&1 | tee /tmp/vd-smoke.log

if ! grep -qE "Now listening|Application started" /tmp/vd-smoke.log; then
  echo "--- smoke log ---" >&2
  cat /tmp/vd-smoke.log >&2
  exit 1
fi

echo "pacman install smoke OK"
EOS

echo "Docker pacman install test passed."

