#!/usr/bin/env bash
# Full Linux packaging build + tests in Docker (no WSL required). Uses the host Docker daemon
# for nested images (Arch makepkg, install smokes).
#
# Requirements: Docker Desktop or Linux Docker; Git Bash or any bash with docker on PATH.
#
# Usage:
#   ./packaging/tools/run-full-linux-build-docker.sh [--arch amd64|arm64] [--image ubuntu:24.04] [--include-flatpak]
#
# Env:
#   DOCKER_IMAGE          default ubuntu:24.04 (when it matches VD_PACKAGING_WORKER_FROM, a pre-apt worker image is built/used)
#   ARCH                  default amd64
#   SNAPCRAFT_IMAGE       optional; default ghcr.io/canonical/snapcraft:8_core24 (snap base core24)
#   VD_INCLUDE_FLATPAK    set to 1 to build Flatpak and run its Docker install smoke (default: skip)
#   VD_PACKAGING_WORKER_IMAGE  default videodedup/packaging-worker:ubuntu24
#   VD_PACKAGING_WORKER_FROM  default ubuntu:24.04 — stock image that triggers the worker base
#   VD_REBUILD_PACKAGING_WORKER_BASE=1  force rebuild of worker image
#   VD_SKIP_PACKAGING_WORKER_BASE=1     always use DOCKER_IMAGE as-is (no worker Dockerfile)
#
# Git Bash / MSYS: path rewriting turns /var/run/docker.sock and /src into paths under
# C:\Program Files\Git\ — set MSYS2_ARG_CONV_EXCL='*' and use a Windows repo path from cygpath.
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
IMAGE="${DOCKER_IMAGE:-ubuntu:24.04}"
ARCH="${ARCH:-amd64}"
VD_INCLUDE_FLATPAK="${VD_INCLUDE_FLATPAK:-0}"

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
    --include-flatpak)
      VD_INCLUDE_FLATPAK=1
      shift
      ;;
    -h|--help)
      echo "Usage: $0 [--arch amd64|arm64] [--image ubuntu:24.04] [--include-flatpak]"
      echo "  Flatpak is skipped by default locally; use --include-flatpak or VD_INCLUDE_FLATPAK=1 for CI parity."
      echo "  When --image matches VD_PACKAGING_WORKER_FROM (default ubuntu:24.04), builds videodedup/packaging-worker:ubuntu24"
      echo "  with apt deps preinstalled unless VD_SKIP_PACKAGING_WORKER_BASE=1."
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

PACKAGING_WORKER_FROM="${VD_PACKAGING_WORKER_FROM:-ubuntu:24.04}"
PACKAGING_WORKER_IMAGE="${VD_PACKAGING_WORKER_IMAGE:-videodedup/packaging-worker:ubuntu24}"

ensure_packaging_worker_base() {
  local tag="$1"
  local dockerfile="${ROOT}/packaging/docker/Dockerfile.packaging-worker-ubuntu24"
  if [[ "${VD_REBUILD_PACKAGING_WORKER_BASE:-0}" == "1" ]] || ! MSYS2_ARG_CONV_EXCL='*' docker image inspect "${tag}" >/dev/null 2>&1; then
    echo "Building packaging worker base ${tag} …"
    mkdir -p "${ROOT}/packaging/out"
    local ctx
    if ! ctx="$(mktemp -d "${ROOT}/packaging/out/.packaging-worker-ctx.XXXXXX" 2>/dev/null)"; then
      ctx="${ROOT}/packaging/out/.packaging-worker-ctx.$$"
      rm -rf "${ctx}"
      mkdir -p "${ctx}"
    fi
    cp "${dockerfile}" "${ctx}/Dockerfile"
    if ! MSYS2_ARG_CONV_EXCL='*' DOCKER_BUILDKIT=1 docker build -t "${tag}" "${ctx}"; then
      rm -rf "${ctx}"
      return 1
    fi
    rm -rf "${ctx}"
  fi
}

if [[ "${VD_SKIP_PACKAGING_WORKER_BASE:-0}" != "1" ]] && [[ "${IMAGE}" == "${PACKAGING_WORKER_FROM}" ]]; then
  ensure_packaging_worker_base "${PACKAGING_WORKER_IMAGE}"
  IMAGE="${PACKAGING_WORKER_IMAGE}"
fi

export SOURCE_DATE_EPOCH
SOURCE_DATE_EPOCH="$(git -C "${ROOT}" log -1 --format=%ct)"

DOCKER_SOCK_VOL="/var/run/docker.sock:/var/run/docker.sock"
# Host-side path for bind mounts: nested `docker run` talks to the host daemon, so this must be
# a path the host understands (see build-pacman.sh / docker cp workaround).
REPO_BIND_SRC="${ROOT}"
if [[ "${OSTYPE:-}" == msys* ]] || [[ "${OSTYPE:-}" == cygwin* ]] || [[ -n "${MSYSTEM:-}" ]]; then
  export MSYS2_ARG_CONV_EXCL='*'
  if ! command -v cygpath >/dev/null 2>&1; then
    echo "Git Bash should provide cygpath; cannot build Windows path for Docker bind mount." >&2
    exit 1
  fi
  REPO_BIND_SRC="$(cygpath -w "$ROOT")"
fi
REPO_VOL="${REPO_BIND_SRC}:/src:rw"

echo "Repo: ${ROOT}"
echo "Image: ${IMAGE}  arch: ${ARCH}  SOURCE_DATE_EPOCH=${SOURCE_DATE_EPOCH}  VD_INCLUDE_FLATPAK=${VD_INCLUDE_FLATPAK}"

DOCKER_ENV=(
  -e "ARCH=${ARCH}"
  -e "SOURCE_DATE_EPOCH=${SOURCE_DATE_EPOCH}"
  -e "VD_INCLUDE_FLATPAK=${VD_INCLUDE_FLATPAK}"
  -e "VD_DOCKER_BIND_SRC=${REPO_BIND_SRC}"
)
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
