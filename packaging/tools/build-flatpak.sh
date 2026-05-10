#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
ARCH="amd64"

while [[ $# -gt 0 ]]; do
  case "$1" in
    --arch) ARCH="$2"; shift 2 ;;
    *) echo "Unknown arg: $1" >&2; exit 1 ;;
  esac
done

META="${ROOT}/packaging/out/metadata.json"
MANIFEST="${ROOT}/packaging/flatpak/io.github.sebastianbecker2.videodedup.server.yml"
OUT="${ROOT}/packaging/out/${ARCH}/flatpak"
BUILD_DIR="${ROOT}/packaging/flatpak/build-dir-${ARCH}"
REPO_DIR="${ROOT}/packaging/flatpak/repo-${ARCH}"

if [[ ! -f "${META}" ]]; then
  "${ROOT}/packaging/common/generate-metadata.sh"
fi

VER="$(python3 -c "import json; print(json.load(open('${META}'))['version'])")"
VER_SAFE="${VER//\"/}"

mkdir -p "${OUT}"

if ! command -v flatpak-builder >/dev/null 2>&1; then
  echo "flatpak-builder not installed; skipping flatpak build" >&2
  exit 0
fi

case "${ARCH}" in
  amd64) FB_ARCH="x86_64"; DOTNET_RID="linux-x64" ;;
  arm64) FB_ARCH="aarch64"; DOTNET_RID="linux-arm64" ;;
  *) echo "Unsupported arch ${ARCH}" >&2; exit 1 ;;
esac

rm -rf "${BUILD_DIR}" "${REPO_DIR}"
mkdir -p "${REPO_DIR}"

# Install runtimes (no-op if present). CI should pre-install for speed.
flatpak remote-add --user --if-not-exists flathub https://flathub.org/repo/flathub.flatpakrepo || true
flatpak install -y --user flathub \
  org.freedesktop.Platform//24.08 \
  org.freedesktop.Sdk//24.08 \
  org.freedesktop.Sdk.Extension.dotnet8//24.08 || true

export FLATPAK_ARCH="${FB_ARCH}"

MANIFEST_RUN="$(mktemp)"
sed "s/-r linux-x64/-r ${DOTNET_RID}/g" "${MANIFEST}" > "${MANIFEST_RUN}"
trap 'rm -f "${MANIFEST_RUN}"' EXIT

flatpak-builder \
  --share=network \
  --force-clean \
  --repo="${REPO_DIR}" \
  --arch="${FB_ARCH}" \
  "${BUILD_DIR}" \
  "${MANIFEST_RUN}"

flatpak build-bundle \
  "${REPO_DIR}" \
  "${OUT}/videodedupserver-${VER_SAFE}-${ARCH}.flatpak" \
  io.github.sebastianbecker2.videodedup.server \
  stable

echo "Flatpak bundle in ${OUT}"
