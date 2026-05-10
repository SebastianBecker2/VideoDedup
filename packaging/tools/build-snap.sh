#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
ARCH="amd64"
REQUIRE_SNAPCRAFT=0

while [[ $# -gt 0 ]]; do
  case "$1" in
    --arch) ARCH="$2"; shift 2 ;;
    --require-snapcraft) REQUIRE_SNAPCRAFT=1; shift ;;
    *) echo "Unknown arg: $1" >&2; exit 1 ;;
  esac
done

META="${ROOT}/packaging/out/metadata.json"
STAGE="${ROOT}/packaging/.stage/${ARCH}/server"
SNAP_DIR="${ROOT}/packaging/snap"
OUT="${ROOT}/packaging/out/${ARCH}/snap"

if [[ ! -f "${META}" ]]; then
  "${ROOT}/packaging/common/generate-metadata.sh"
fi
if [[ ! -d "${STAGE}" ]]; then
  echo "Missing staged payload; run stage.sh" >&2
  exit 1
fi

VER="$(python3 -c "import json; print(json.load(open('${META}'))['version'])")"
VER_SAFE="${VER//\"/}"

rm -rf "${SNAP_DIR}/_dump"
mkdir -p "${SNAP_DIR}/_dump"
cp -a "${STAGE}/." "${SNAP_DIR}/_dump/"

mkdir -p "${OUT}"

if ! command -v snapcraft >/dev/null 2>&1; then
  echo "snapcraft not installed; skipping snap build" >&2
  if [[ "${REQUIRE_SNAPCRAFT}" -eq 1 ]]; then
    exit 1
  fi
  exit 0
fi

BACKUP="$(mktemp)"
cp "${SNAP_DIR}/snapcraft.yaml" "${BACKUP}"
restore() {
  cp "${BACKUP}" "${SNAP_DIR}/snapcraft.yaml"
  rm -f "${BACKUP}"
}
trap restore EXIT

sed "s/^version: \".*\"/version: \"${VER_SAFE}\"/" "${BACKUP}" > "${SNAP_DIR}/snapcraft.yaml"

(
  cd "${SNAP_DIR}"
  if snapcraft --destructive-mode --output "${OUT}/videodedupserver_${VER_SAFE}_${ARCH}.snap"; then
    :
  else
    snapcraft --destructive-mode
    shopt -s nullglob
    BUILT=(./*.snap)
    shopt -u nullglob
    if ((${#BUILT[@]} > 0)); then
      mv -f "${BUILT[0]}" "${OUT}/videodedupserver_${VER_SAFE}_${ARCH}.snap"
    else
      exit 1
    fi
  fi
)

echo "Snap built in ${OUT}"
