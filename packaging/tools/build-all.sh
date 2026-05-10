#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
ARCH="amd64"
FORMATS="deb"

usage() {
  echo "Usage: $0 [--arch amd64|arm64] [--formats deb,rpm,snap,flatpak,pacman]" >&2
  exit 1
}

while [[ $# -gt 0 ]]; do
  case "$1" in
    --arch)
      [[ $# -ge 2 ]] || usage
      ARCH="$2"
      shift 2
      ;;
    --formats)
      [[ $# -ge 2 ]] || usage
      FORMATS="$2"
      shift 2
      ;;
    *)
      usage
      ;;
  esac
done

"${ROOT}/packaging/tools/stage.sh" --arch "${ARCH}"

IFS=',' read -ra FMT <<< "${FORMATS}"
for f in "${FMT[@]}"; do
  case "${f}" in
    deb) "${ROOT}/packaging/tools/build-deb.sh" --arch "${ARCH}" ;;
    rpm) "${ROOT}/packaging/tools/build-rpm.sh" --arch "${ARCH}" ;;
    snap) "${ROOT}/packaging/tools/build-snap.sh" --arch "${ARCH}" ;;
    flatpak) "${ROOT}/packaging/tools/build-flatpak.sh" --arch "${ARCH}" ;;
    pacman) "${ROOT}/packaging/tools/build-pacman.sh" --arch "${ARCH}" ;;
    *) echo "Unknown format: ${f}" >&2; exit 1 ;;
  esac
done

"${ROOT}/packaging/tools/write-checksums.sh" --arch "${ARCH}"
