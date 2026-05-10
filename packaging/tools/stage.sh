#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
ARCH="amd64"

usage() {
  echo "Usage: $0 [--arch amd64|arm64]" >&2
  exit 1
}

while [[ $# -gt 0 ]]; do
  case "$1" in
    --arch)
      [[ $# -ge 2 ]] || usage
      ARCH="$2"
      shift 2
      ;;
    *)
      usage
      ;;
  esac
done

case "${ARCH}" in
  amd64) RID="linux-x64" ;;
  arm64) RID="linux-arm64" ;;
  *) echo "Unsupported --arch ${ARCH}" >&2; exit 1 ;;
esac

export SOURCE_DATE_EPOCH="${SOURCE_DATE_EPOCH:-$(git -C "${ROOT}" log -1 --format=%ct)}"

# shellcheck source=../common/packaging-python.sh disable=SC1091
source "${ROOT}/packaging/common/packaging-python.sh"
resolve_packaging_python || exit 1

STAGE="${ROOT}/packaging/.stage/${ARCH}/server"
rm -rf "${STAGE}"
mkdir -p "${STAGE}"

# Self-contained so DEB/RPM install on stock Debian/Fedora without Microsoft's dotnet-runtime apt/dnf feed.
dotnet publish "${ROOT}/VideoDedupService/VideoDedupService.csproj" \
  -c Release \
  -r "${RID}" \
  --self-contained true \
  -p:PublishSingleFile=true \
  -o "${STAGE}"

cp -f "${ROOT}/VideoDedupService/appsettings.json" "${STAGE}/"
if [[ -f "${ROOT}/VideoDedupService/appsettings.Development.json" ]]; then
  cp -f "${ROOT}/VideoDedupService/appsettings.Development.json" "${STAGE}/"
fi

# Linux packages do not ship server.pfx; default appsettings.json binds HTTPS with a missing cert and
# the service exits before systemd Type=notify readiness. Force cleartext HTTP/2 (h2c) for gRPC here.
# Use [::] (not *) so Kestrel listens dual-stack (IPv4 + IPv6); http://*:port is IPv4-only on Linux.
APPS_JSON="${STAGE}/appsettings.json"
if command -v cygpath >/dev/null 2>&1; then
  APPS_JSON="$(cygpath -w "${STAGE}/appsettings.json")"
fi
"${PACKAGING_PYTHON[@]}" <<PY
import json
from pathlib import Path

path = Path(r"${APPS_JSON}")
data = json.loads(path.read_text(encoding="utf-8"))
grpc = data.setdefault("Kestrel", {}).setdefault("Endpoints", {}).setdefault("gRPC", {})
grpc["Url"] = "http://[::]:51726"
grpc["Protocols"] = "Http2"
grpc.pop("Certificate", None)
path.write_text(json.dumps(data, indent=2) + "\n", encoding="utf-8")
PY

chmod -R a+rX "${STAGE}" || true

"${ROOT}/packaging/common/generate-metadata.sh"

echo "Staged server payload: ${STAGE}"
