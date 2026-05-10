#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
ARCH="amd64"

while [[ $# -gt 0 ]]; do
  case "$1" in
    --arch) ARCH="$2"; shift 2 ;;
    *) shift ;;
  esac
done

OUT="${ROOT}/packaging/out/${ARCH}"
SUM="${ROOT}/packaging/out/SHA256SUMS-${ARCH}.txt"
mkdir -p "${OUT}"

if [[ ! -d "${OUT}" ]]; then
  echo "No output dir ${OUT}" >&2
  exit 0
fi

( cd "${ROOT}/packaging/out" && find "${ARCH}" -type f \( -name '*.deb' -o -name '*.rpm' -o -name '*.snap' -o -name '*.flatpak' -o -name '*.pkg.tar.zst' \) -print0 | sort -z | xargs -0 sha256sum ) > "${SUM}" || true

if [[ -s "${SUM}" ]]; then
  echo "Wrote ${SUM}"
else
  echo "No packages found to checksum under packaging/out/${ARCH}" >&2
  rm -f "${SUM}"
fi
