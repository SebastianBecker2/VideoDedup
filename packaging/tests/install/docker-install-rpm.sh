#!/usr/bin/env bash
# Smoke-test videodedupserver .rpm inside Fedora (Docker): dnf install, file checks, short process run.
#
# Usage:
#   ./packaging/tests/install/docker-install-rpm.sh [path/to/package.rpm]
#   ./packaging/tests/install/docker-install-rpm.sh --arch arm64
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../../.." && pwd)"
ARCH="amd64"
RPM=""

while [[ $# -gt 0 ]]; do
  case "$1" in
    --arch)
      ARCH="$2"
      shift 2
      ;;
    -h|--help)
      echo "Usage: $0 [--arch amd64|arm64] [path/to/videodedupserver-*.rpm]"
      exit 0
      ;;
    *)
      if [[ -n "${RPM}" ]]; then
        echo "Extra argument: $1" >&2
        exit 1
      fi
      RPM="$1"
      shift
      ;;
  esac
done

if [[ -z "${RPM}" ]]; then
  shopt -s nullglob
  candidates=( "${ROOT}/packaging/out/${ARCH}/rpm/"*/*.rpm )
  shopt -u nullglob
  if ((${#candidates[@]} == 0)); then
    echo "No .rpm in ${ROOT}/packaging/out/${ARCH}/rpm/*/ — build one first (e.g. packaging/tools/build-rpm.sh)" >&2
    exit 1
  fi
  RPM="$(ls -t "${candidates[@]}" | head -1)"
fi

if [[ ! -f "${RPM}" ]]; then
  echo "Not a file: ${RPM}" >&2
  exit 1
fi

RPM_ABS="$(cd "$(dirname "${RPM}")" && pwd)/$(basename "${RPM}")"

docker_host_path() {
  if command -v cygpath >/dev/null 2>&1; then
    cygpath -w "$1"
  else
    printf '%s' "$1"
  fi
}

if ! command -v docker >/dev/null 2>&1; then
  echo "docker not found" >&2
  exit 1
fi
if ! docker info >/dev/null 2>&1; then
  echo "Docker daemon not reachable" >&2
  exit 1
fi

RPM_VOL="$(docker_host_path "${RPM_ABS}")"
INNER="${ROOT}/packaging/tests/install/rpm-install-inner-smoke.sh"
INNER_VOL="$(docker_host_path "${INNER}")"
echo "Using ${RPM_ABS}"

MSYS2_ARG_CONV_EXCL='*' docker run --rm \
  -v "${RPM_VOL}:/tmp/videodedupserver.rpm:ro" \
  -v "${INNER_VOL}:/inner-smoke.sh:ro" \
  fedora:40 \
  bash /inner-smoke.sh

echo "Docker .rpm install test passed."
