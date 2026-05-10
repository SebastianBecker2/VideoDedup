#!/usr/bin/env bash
# Full Linux packaging build + tests in Docker (no WSL required). Uses the host Docker daemon
# for nested images (Arch makepkg, install smokes).
#
# Requirements: Docker Desktop or Linux Docker; Git Bash or any bash with docker on PATH.
#
# Usage:
#   ./packaging/tools/run-full-linux-build-docker.sh [--arch amd64|arm64] [--image ubuntu:24.04]
#
# Env:
#   DOCKER_IMAGE     default ubuntu:24.04
#   ARCH             default amd64
#   SNAPCRAFT_IMAGE  optional; default ghcr.io/canonical/snapcraft:8_core24 (snap base core24)
#
# Git Bash / MSYS: path rewriting turns /var/run/docker.sock and /src into paths under
# C:\Program Files\Git\ — set MSYS2_ARG_CONV_EXCL='*' and use a Windows repo path from cygpath.
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
IMAGE="${DOCKER_IMAGE:-ubuntu:24.04}"
ARCH="${ARCH:-amd64}"

while [[ $# -gt 0 ]]; do
  case "$1" in
    --arch)
      ARCH="$2"
      shift 2
      ;;
    --image)
      IMAGE="$2"
      shift 2
      ;;
    -h|--help)
      echo "Usage: $0 [--arch amd64|arm64] [--image ubuntu:24.04]"
      exit 0
      ;;
    *)
      echo "Unknown option: $1" >&2
      exit 1
      ;;
  esac
done

case "${ARCH}" in
  amd64|arm64) ;;
  *) echo "Unsupported --arch ${ARCH}" >&2; exit 1 ;;
esac

if ! command -v docker >/dev/null 2>&1; then
  echo "docker not found on PATH; install Docker Desktop (Windows) or docker.io (Linux)." >&2
  exit 1
fi
if ! docker info >/dev/null 2>&1; then
  echo "Docker daemon not reachable (start Docker Desktop / the docker service)." >&2
  exit 1
fi

if ! git -C "${ROOT}" rev-parse --git-dir >/dev/null 2>&1; then
  echo "Not a git checkout: ${ROOT}" >&2
  exit 1
fi

export SOURCE_DATE_EPOCH
SOURCE_DATE_EPOCH="$(git -C "${ROOT}" log -1 --format=%ct)"

DOCKER_SOCK_VOL="/var/run/docker.sock:/var/run/docker.sock"
REPO_VOL="${ROOT}:/src:rw"
if [[ "${OSTYPE:-}" == msys* ]] || [[ "${OSTYPE:-}" == cygwin* ]] || [[ -n "${MSYSTEM:-}" ]]; then
  export MSYS2_ARG_CONV_EXCL='*'
  if ! command -v cygpath >/dev/null 2>&1; then
    echo "Git Bash should provide cygpath; cannot build Windows path for Docker bind mount." >&2
    exit 1
  fi
  REPO_VOL="$(cygpath -w "$ROOT"):/src:rw"
fi

echo "Repo: ${ROOT}"
echo "Image: ${IMAGE}  arch: ${ARCH}  SOURCE_DATE_EPOCH=${SOURCE_DATE_EPOCH}"

DOCKER_ENV=( -e "ARCH=${ARCH}" -e "SOURCE_DATE_EPOCH=${SOURCE_DATE_EPOCH}" )
[[ -n "${SNAPCRAFT_IMAGE:-}" ]] && DOCKER_ENV+=( -e "SNAPCRAFT_IMAGE=${SNAPCRAFT_IMAGE}" )

# --privileged: nested snapcraft image expects it; some install smokes need it too.
# --network host / --pid=host: flatpak-builder/bubblewrap can hit EPERM on /proc/sys/... under
# default Docker isolation (common on Docker Desktop + bind-mounted repos).
# Strip CR so a Windows CRLF checkout of the inner script does not break `set -o pipefail`.
exec docker run --rm --privileged --network host --pid=host \
  -v "${DOCKER_SOCK_VOL}" \
  -v "${REPO_VOL}" \
  -w /src \
  "${DOCKER_ENV[@]}" \
  "${IMAGE}" \
  bash -c 'sed "s/\r$//" /src/packaging/tools/run-full-linux-build-docker-inner.sh | bash -s --'
