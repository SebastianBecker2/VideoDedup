#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
ARCH="amd64"

# shellcheck source=../common/packaging-python.sh disable=SC1091
source "${ROOT}/packaging/common/packaging-python.sh"
resolve_packaging_python || exit 1

while [[ $# -gt 0 ]]; do
  case "$1" in
    --arch) ARCH="$2"; shift 2 ;;
    *) echo "Unknown arg: $1" >&2; exit 1 ;;
  esac
done

case "${ARCH}" in
  amd64) RPMARCH="x86_64" ;;
  arm64) RPMARCH="aarch64" ;;
  *) echo "Unsupported arch ${ARCH}" >&2; exit 1 ;;
esac

META="${ROOT}/packaging/out/metadata.json"
STAGE="${ROOT}/packaging/.stage/${ARCH}/server"
OUT="${ROOT}/packaging/out/${ARCH}/rpm"
# rpmbuild helpers (find-debuginfo, debugedit) break on spaces in %{buildroot}. Use a short TMPDIR path.
TOP="$(mktemp -d "${TMPDIR:-/tmp}/videodedup-rpm-${ARCH}.XXXXXX")"
_cleanup_rpm_top() { rm -rf "${TOP}"; }
trap _cleanup_rpm_top EXIT

if [[ ! -f "${META}" ]]; then
  "${ROOT}/packaging/common/generate-metadata.sh"
fi
if [[ ! -d "${STAGE}" ]]; then
  echo "Missing staged payload; run stage.sh" >&2
  exit 1
fi

VD_META_JSON="${META}"
if command -v cygpath >/dev/null 2>&1; then
  VD_META_JSON="$(cygpath -w "${META}")"
fi
export VD_META_JSON

VER_RAW="$("${PACKAGING_PYTHON[@]}" -c "import json, os; print(json.load(open(os.environ['VD_META_JSON'], encoding='utf-8'))['version'])")"
# RPM Version must not contain '-' (reserved for Name-Version-Release). Git describe uses hyphens.
VER_RPM="${VER_RAW//+/_}"
VER_RPM="${VER_RPM//-/.}"
PAYLOAD_NAME="videodedupserver-${VER_RPM}"
DESC="$("${PACKAGING_PYTHON[@]}" -c "import json, os; print(json.load(open(os.environ['VD_META_JSON'], encoding='utf-8'))['description'].strip())")"
HOME="$("${PACKAGING_PYTHON[@]}" -c "import json, os; print(json.load(open(os.environ['VD_META_JSON'], encoding='utf-8'))['homepage'])")"
MAINTAINER="$("${PACKAGING_PYTHON[@]}" -c "import json, os; print(json.load(open(os.environ['VD_META_JSON'], encoding='utf-8'))['maintainer'])")"
CHANGELOG_DATE="$(date -u '+%a %b %d %Y')"

mkdir -p "${TOP}/"{BUILD,RPMS,SRPMS,SOURCES,SPECS}

# rpmbuild %setup expects a single top-level directory matching Name-Version.
rm -rf "${TOP}/SOURCES/${PAYLOAD_NAME}"
mkdir -p "${TOP}/SOURCES/${PAYLOAD_NAME}"
cp -a "${STAGE}/." "${TOP}/SOURCES/${PAYLOAD_NAME}/"

(
  cd "${TOP}/SOURCES"
  rm -f "videodedupserver-${VER_RPM}.tar.gz"
  tar --sort=name --mtime="@${SOURCE_DATE_EPOCH:-$(date +%s)}" \
    --owner=0 --group=0 --numeric-owner \
    -czf "videodedupserver-${VER_RPM}.tar.gz" "${PAYLOAD_NAME}"
)

SPEC_IN="${ROOT}/packaging/rpm/videodedupserver.spec"
SPEC_OUT="${TOP}/SPECS/videodedupserver.spec"

sed \
  -e "s|@VERSION@|${VER_RPM}|g" \
  -e "s|@RPMARCH@|${RPMARCH}|g" \
  -e "s|@HOMEPAGE@|${HOME}|g" \
  -e "s|@DESCRIPTION@|${DESC}|g" \
  -e "s|@REPO@|${ROOT}|g" \
  -e "s|@MAINTAINER@|${MAINTAINER}|g" \
  -e "s|@CHANGELOG_DATE@|${CHANGELOG_DATE}|g" \
  "${SPEC_IN}" > "${SPEC_OUT}"
# CRLF in .spec breaks %prep (embedded script gets \r); normalize for Linux rpmbuild.
sed -i 's/\r$//' "${SPEC_OUT}"

# Write RPMs under TOP first: DrvFs (/mnt/d/...) often denies writes from rpmbuild; copy out afterward.
rpmbuild -bb "${SPEC_OUT}" --define "_topdir ${TOP}" --define "_rpmdir ${TOP}/RPMS" \
  --define "_builddir ${TOP}/BUILD"

mkdir -p "${OUT}/${RPMARCH}"
cp -a "${TOP}/RPMS/${RPMARCH}/"*.rpm "${OUT}/${RPMARCH}/"

echo "RPM artifacts in ${OUT}/${RPMARCH}"
