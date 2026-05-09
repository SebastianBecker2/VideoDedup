#!/usr/bin/env bash
# Build videodedupserver_<version>_<arch>.deb from the repository root.
#
# Debian 12+ / Ubuntu 22.04+ (amd64): install prerequisites, then run this script.
#   sudo apt-get update
#   sudo apt-get install -y wget ca-certificates
#   wget -qO /tmp/packages-microsoft-prod.deb https://packages.microsoft.com/config/debian/12/packages-microsoft-prod.deb
#   sudo dpkg -i /tmp/packages-microsoft-prod.deb && sudo apt-get update
#   sudo apt-get install -y dotnet-sdk-8.0 fakeroot dpkg-dev python3 git
#   git clone <your-repo-url> && cd VideoDedup && git checkout Linux
#   chmod +x packaging/tools/build-deb-one-shot.sh
#   ./packaging/tools/build-deb-one-shot.sh
#
# On Ubuntu, use https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb
# (or the matching 24.04 URL) instead of the Debian 12 package above.
#
# Output: packaging/out/<arch>/deb/*.deb
#
# Docker (optional): from repo root with BuildKit and Docker daemon running:
#   docker build -f packaging/docker/Dockerfile.build-deb --target artifacts -o type=local,dest=./packaging-deb-out .

set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
ARCH="${1:-amd64}"
cd "${ROOT}"

missing=()
for cmd in dotnet git python3 fakeroot dpkg-deb; do
  command -v "${cmd}" >/dev/null 2>&1 || missing+=("${cmd}")
done
if ((${#missing[@]})); then
  echo "Missing command(s): ${missing[*]}" >&2
  echo "Install hints are in the header comments of this script." >&2
  exit 1
fi

chmod +x packaging/tools/*.sh packaging/common/generate-metadata.sh 2>/dev/null || true
export SOURCE_DATE_EPOCH="${SOURCE_DATE_EPOCH:-$(git log -1 --format=%ct)}"
./packaging/tools/stage.sh --arch "${ARCH}"
./packaging/tools/build-deb.sh --arch "${ARCH}"
shopt -s nullglob
debs=(packaging/out/"${ARCH}"/deb/*.deb)
shopt -u nullglob
if ((${#debs[@]} == 0)); then
  echo "No .deb produced under packaging/out/${ARCH}/deb/" >&2
  exit 1
fi
for f in "${debs[@]}"; do
  echo "Built: ${ROOT}/${f#./}"
done
