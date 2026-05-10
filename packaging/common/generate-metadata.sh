#!/usr/bin/env bash
set -euo pipefail

# Emit packaging/out/metadata.json from project.meta.json + git.
# Requires: Python 3 (python3 or py -3), git.

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
# shellcheck source=packaging-python.sh disable=SC1091
source "${ROOT_DIR}/packaging/common/packaging-python.sh"
resolve_packaging_python || exit 1

OUT_DIR="${ROOT_DIR}/packaging/out"
mkdir -p "${OUT_DIR}"

GIT_SHA="$(git -C "${ROOT_DIR}" rev-parse --short HEAD 2>/dev/null || echo "unknown")"
GIT_TAG="$(git -C "${ROOT_DIR}" describe --tags --always --dirty 2>/dev/null || true)"
if [[ -z "${GIT_TAG}" ]]; then
  GIT_TAG="0.0.0+${GIT_SHA}"
fi
VERSION="${GIT_TAG#v}"
SOURCE_DATE_EPOCH="$(git -C "${ROOT_DIR}" log -1 --format=%ct 2>/dev/null || echo "0")"

CHANGELOG_LINE="$(printf '%s  %s  %s\n' "$(date -u -d "@${SOURCE_DATE_EPOCH}" '+%a, %d %b %Y %H:%M:%S +0000' 2>/dev/null || date -u '+%a, %d %b %Y %H:%M:%S +0000')" "${VERSION}" "Automated entry from git ${GIT_SHA}")"

export ROOT_DIR OUT_DIR VERSION GIT_SHA GIT_TAG SOURCE_DATE_EPOCH CHANGELOG_LINE

"${PACKAGING_PYTHON[@]}" <<'PY'
import json, os, pathlib

root = pathlib.Path(os.environ["ROOT_DIR"])
static_path = root / "packaging" / "common" / "project.meta.json"
out_path = pathlib.Path(os.environ["OUT_DIR"]) / "metadata.json"

data = json.loads(static_path.read_text(encoding="utf-8"))
data["version"] = os.environ["VERSION"]
data["git_sha"] = os.environ["GIT_SHA"]
data["git_tag"] = os.environ["GIT_TAG"]
data["source_date_epoch"] = int(os.environ["SOURCE_DATE_EPOCH"])
data["changelog_debian"] = os.environ["CHANGELOG_LINE"]
out_path.write_text(json.dumps(data, indent=2, ensure_ascii=False) + "\n", encoding="utf-8")
print(f"Wrote {out_path}")
PY
