#!/usr/bin/env bash
# Run from WSL after: git config --global --add safe.directory "$(wslpath -a <win-repo>)"
# Uses ~/.dotnet if present; set DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1 if libicu is missing.
set -euo pipefail
if [[ -n "${BUILD_DEB_ROOT:-}" ]]; then
  ROOT="${BUILD_DEB_ROOT}"
else
  ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
fi
ORIG_ROOT="${ROOT}"
# MSBuild on DrvFs (/mnt/...) often fails with MSB3374 (cannot set last write time). Clone to ext4.
# DrvFs breaks MSBuild timestamps (MSB3374). Copy working tree to ext4 (keeps uncommitted files vs git clone).
if [[ "${ROOT}" == /mnt/* ]]; then
  WORK="/tmp/videodedup-debbuild-$(id -u)"
  rm -rf "${WORK}"
  mkdir -p "${WORK}"
  rsync -a "${ORIG_ROOT}/" "${WORK}/" \
    --exclude=bin \
    --exclude=obj \
    --exclude=packaging/.stage \
    --exclude=packaging/out
  ROOT="${WORK}"
  git config --global --add safe.directory "${ROOT}" 2>/dev/null || true
fi
cd "${ROOT}"
export DOTNET_ROOT="${DOTNET_ROOT:-$HOME/.dotnet}"
# Avoid inheriting Windows PATH via WSL (unquoted parentheses break bash -c from PowerShell).
export PATH="/usr/local/sbin:/usr/local/bin:/usr/sbin:/usr/bin:/sbin:/bin:${DOTNET_ROOT}:${DOTNET_ROOT}/tools"
if [[ -z "${DOTNET_SYSTEM_GLOBALIZATION_INVARIANT:-}" ]] && ! ldconfig -p 2>/dev/null | grep -q libicu; then
  export DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1
fi
chmod +x packaging/tools/*.sh packaging/common/generate-metadata.sh 2>/dev/null || true
export SOURCE_DATE_EPOCH="${SOURCE_DATE_EPOCH:-$(git log -1 --format=%ct)}"
ARCH="${1:-amd64}"
./packaging/tools/stage.sh --arch "${ARCH}"
./packaging/tools/build-deb.sh --arch "${ARCH}"
if [[ "${ORIG_ROOT}" != "${ROOT}" ]]; then
  mkdir -p "${ORIG_ROOT}/packaging/out/${ARCH}/deb"
  cp -f "${ROOT}/packaging/out/${ARCH}/deb/"*.deb "${ORIG_ROOT}/packaging/out/${ARCH}/deb/"
  echo "Copied .deb to ${ORIG_ROOT}/packaging/out/${ARCH}/deb/"
fi
ls -la "${ORIG_ROOT}/packaging/out/${ARCH}/deb/"
