#!/usr/bin/env bash
# One-shot: stage Linux payload → build package(s) → docker-grpc-firewall E2E.
#
# Run from Git Bash on Windows (recommended) or from WSL at the same repo path.
# .deb / .rpm packaging runs inside WSL when the host has no dpkg-deb / rpmbuild.
#
# Usage:
#   ./packaging/tests/e2e/run-rebuild-and-e2e.sh [--arch amd64|arm64] [--format deb|rpm|both]
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../../.." && pwd)"
cd "${ROOT}"

ARCH="amd64"
FORMAT="deb"
while [[ $# -gt 0 ]]; do
  case "$1" in
    --arch)
      ARCH="$2"
      shift 2
      ;;
    --format)
      FORMAT="$2"
      shift 2
      ;;
    -h|--help)
      echo "Usage: $0 [--arch amd64|arm64] [--format deb|rpm|both]"
      exit 0
      ;;
    *)
      echo "Unknown arg: $1" >&2
      exit 1
      ;;
  esac
done

case "${ARCH}" in
  amd64|arm64) ;;
  *) echo "Unsupported --arch ${ARCH}" >&2; exit 1 ;;
esac

case "${FORMAT}" in
  deb|rpm|both) ;;
  *) echo "Unsupported --format ${FORMAT}" >&2; exit 1 ;;
esac

if ! command -v dotnet >/dev/null 2>&1 && [[ -x "${HOME}/.dotnet/dotnet" ]]; then
  export PATH="${HOME}/.dotnet:${PATH}"
  export DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1
fi

export SOURCE_DATE_EPOCH="$(git log -1 --format=%ct)"
chmod +x packaging/tools/*.sh packaging/common/generate-metadata.sh packaging/common/packaging-python.sh \
  packaging/tests/e2e/*.sh packaging/tests/install/*.sh packaging/tests/firewall/*.sh 2>/dev/null || true

to_wsl_path() {
  local p="$1"
  if [[ "${p}" == /mnt/* ]]; then
    printf '%s' "${p}"
    return
  fi
  if [[ "${p}" =~ ^/([a-zA-Z])/(.*)$ ]]; then
    local ld="${BASH_REMATCH[1],,}"
    printf '/mnt/%s/%s' "${ld}" "${BASH_REMATCH[2]}"
    return
  fi
  printf '%s' "${p}"
}

run_build_deb() {
  if command -v dpkg-deb >/dev/null 2>&1; then
    ./packaging/tools/build-deb.sh --arch "${ARCH}"
    return
  fi
  if command -v wsl.exe >/dev/null 2>&1; then
    wsl.exe -e bash -lc "cd '$(to_wsl_path "${ROOT}")' && ./packaging/tools/build-deb.sh --arch '${ARCH}'"
    return
  fi
  if command -v wsl >/dev/null 2>&1; then
    wsl -e bash -lc "cd '$(to_wsl_path "${ROOT}")' && ./packaging/tools/build-deb.sh --arch '${ARCH}'"
    return
  fi
  echo "packaging: install dpkg-deb (Linux/WSL) or use WSL so the .deb step can run." >&2
  exit 1
}

run_build_rpm() {
  if command -v rpmbuild >/dev/null 2>&1; then
    ./packaging/tools/build-rpm.sh --arch "${ARCH}"
    return
  fi
  if command -v wsl.exe >/dev/null 2>&1; then
    wsl.exe -e bash -lc "cd '$(to_wsl_path "${ROOT}")' && ./packaging/tools/build-rpm.sh --arch '${ARCH}'"
    return
  fi
  if command -v wsl >/dev/null 2>&1; then
    wsl -e bash -lc "cd '$(to_wsl_path "${ROOT}")' && ./packaging/tools/build-rpm.sh --arch '${ARCH}'"
    return
  fi
  echo "packaging: install rpmbuild (e.g. sudo apt install rpm on Debian/Ubuntu/WSL) for the .rpm step." >&2
  exit 1
}

./packaging/tools/stage.sh --arch "${ARCH}"

case "${FORMAT}" in
  deb)
    run_build_deb
    python3 ./packaging/tests/e2e/docker_grpc_firewall.py --arch "${ARCH}" --format deb
    ;;
  rpm)
    run_build_rpm
    python3 ./packaging/tests/e2e/docker_grpc_firewall.py --arch "${ARCH}" --format rpm
    ;;
  both)
    run_build_deb
    run_build_rpm
    python3 ./packaging/tests/e2e/docker_grpc_firewall.py --arch "${ARCH}" --format deb
    python3 ./packaging/tests/e2e/docker_grpc_firewall.py --arch "${ARCH}" --format rpm
    ;;
esac

echo "Rebuild + E2E complete (${FORMAT})."
