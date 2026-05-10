#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"

rm -rf \
  "${ROOT}/packaging/.stage" \
  "${ROOT}/packaging/out/deb-work" \
  "${ROOT}/packaging/out/rpm-build" \
  "${ROOT}/packaging/out" \
  "${ROOT}/packaging/snap/_dump" \
  "${ROOT}/packaging/snap/parts" \
  "${ROOT}/packaging/snap/stage" \
  "${ROOT}/packaging/snap/prime" \
  "${ROOT}/packaging/flatpak/.flatpak-builder" \
  "${ROOT}/packaging/flatpak/build-dir" \
  "${ROOT}/packaging/flatpak/build-dir-amd64" \
  "${ROOT}/packaging/flatpak/build-dir-arm64" \
  "${ROOT}/packaging/flatpak/repo" \
  "${ROOT}/packaging/flatpak/repo-amd64" \
  "${ROOT}/packaging/flatpak/repo-arm64"

echo "Cleaned packaging build trees under packaging/"
