#!/usr/bin/env bash
# Smoke-test videodedupserver .flatpak inside Fedora (Docker): install runtime + bundle, short flatpak run.
#
# Usage:
#   ./packaging/tests/install/docker-install-flatpak.sh [--arch amd64|arm64] [path/to.flatpak]
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../../.." && pwd)"
ARCH="amd64"
FB=""

while [[ $# -gt 0 ]]; do
  case "$1" in
    --arch)
      ARCH="$2"
      shift 2
      ;;
    -h|--help)
      echo "Usage: $0 [--arch amd64|arm64] [path/to/*.flatpak]"
      exit 0
      ;;
    *)
      if [[ -n "${FB}" ]]; then
        echo "Extra argument: $1" >&2
        exit 1
      fi
      FB="$1"
      shift
      ;;
  esac
done

if [[ -z "${FB}" ]]; then
  shopt -s nullglob
  candidates=( "${ROOT}/packaging/out/${ARCH}/flatpak/"*.flatpak )
  shopt -u nullglob
  if ((${#candidates[@]} == 0)); then
    echo "No .flatpak in ${ROOT}/packaging/out/${ARCH}/flatpak/ — build one first (packaging/tools/build-flatpak.sh)" >&2
    exit 1
  fi
  FB="$(ls -t "${candidates[@]}" | head -1)"
fi

if [[ ! -f "${FB}" ]]; then
  echo "Not a file: ${FB}" >&2
  exit 1
fi

FB_ABS="$(cd "$(dirname "${FB}")" && pwd)/$(basename "${FB}")"

if ! command -v docker >/dev/null 2>&1; then
  echo "docker not found" >&2
  exit 1
fi
if ! docker info >/dev/null 2>&1; then
  echo "Docker daemon not reachable" >&2
  exit 1
fi

echo "Using ${FB_ABS}"

docker run --rm \
  -v "${FB_ABS}:/tmp/videodedupserver.flatpak:ro" \
  fedora:40 \
  bash -s <<'EOS'
set -eu
dnf -y -q install flatpak
flatpak remote-add --if-not-exists flathub https://flathub.org/repo/flathub.flatpakrepo
flatpak install -y --noninteractive flathub org.freedesktop.Platform//24.08
flatpak install -y --noninteractive --bundle /tmp/videodedupserver.flatpak

install -d -m 0755 -o root -g root /tmp/vd-xdg
export XDG_RUNTIME_DIR=/tmp/vd-xdg
timeout 25s flatpak run io.github.sebastianbecker2.videodedup.server 2>&1 | tee /tmp/vd-smoke.log

if ! grep -qE "Now listening|Application started" /tmp/vd-smoke.log; then
  echo "--- smoke log ---" >&2
  cat /tmp/vd-smoke.log >&2
  exit 1
fi

echo "flatpak install smoke OK"
EOS

echo "Docker flatpak install test passed."

