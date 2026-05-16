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

# Snap version: alnum and . + - ~ : only (no underscore); trim invalid edges.
VER_SNAP="$(python3 -c "
import json, re, sys
v = json.load(open(sys.argv[1], encoding='utf-8'))['version']
v = v.replace('_', '.')
v = re.sub(r'[^a-zA-Z0-9.+:~-]', '-', v)
v = re.sub(r'^[.:+~-]+', '', v)
v = re.sub(r'[.:+-]$', '', v)
print(v or '0')
" "${META}")"

rm -rf "${SNAP_DIR}/_dump"
mkdir -p "${SNAP_DIR}/_dump"
cp -a "${STAGE}/." "${SNAP_DIR}/_dump/"
# Launch wrapper is installed by snapcraft.yaml override-build from repo root.

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

sed "s/^version: \".*\"/version: \"${VER_SNAP}\"/" "${BACKUP}" > "${SNAP_DIR}/snapcraft.yaml"

vd_snapcraft_fail() {
  local code=$1
  echo "snapcraft pack failed (exit ${code})" >&2
  local log
  log="$(ls -t "${HOME}/.local/state/snapcraft/log"/snapcraft-*.log 2>/dev/null | head -1 || true)"
  if [[ -n "${log}" && -f "${log}" ]]; then
    echo "--- tail of ${log} ---" >&2
    tail -n 80 "${log}" >&2 || true
  fi
  exit "${code}"
}

(
  cd "${SNAP_DIR}"
  snapcraft clean 2>/dev/null || true
  rm -rf parts prime stage .craft
  if ! snapcraft pack --destructive-mode -o "${OUT}" --verbosity verbose; then
    vd_snapcraft_fail $?
  fi
)

DEST="${OUT}/videodedupserver_${VER_SNAP}_${ARCH}.snap"
shopt -s nullglob
BUILT=("${OUT}"/*.snap)
shopt -u nullglob
if ((${#BUILT[@]} == 0)); then
  echo "snapcraft pack produced no .snap under ${OUT}" >&2
  exit 1
fi
NEWEST="$(ls -t "${BUILT[@]}" | head -1)"
mv -f "${NEWEST}" "${DEST}"

echo "Snap built: ${DEST}"
