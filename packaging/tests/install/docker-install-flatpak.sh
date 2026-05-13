#!/usr/bin/env bash
# Smoke-test videodedupserver .flatpak inside Fedora (Docker): install runtime + bundle, short flatpak run.
#
# By default builds/uses packaging/docker/Dockerfile.flatpak-smoke-fedora so dnf + Flathub runtime install
# are skipped when the image exists (Windows: build context under repo tree).
# VD_FLATPAK_SMOKE_BASE_IMAGE — tag for that base (default videodedup/flatpak-smoke-fedora:40).
# VD_REBUILD_FLATPAK_SMOKE_BASE=1 — force docker build.
# Use raw fedora:40: VD_FLATPAK_SMOKE_BASE_IMAGE=fedora:40 (no preinstall; script still works).
#
# Usage:
#   ./packaging/tests/install/docker-install-flatpak.sh [--arch amd64|arm64] [path/to.flatpak]
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../../.." && pwd)"
ARCH="amd64"
FB=""

while [[ $# -gt 0 ]]; do
  case "$1" in
    --arch)
      ARCH="$2"
      shift 2
      ;;
    -h|--help)
      echo "Usage: $0 [--arch amd64|arm64] [path/to/*.flatpak]"
      exit 0
      ;;
    *)
      if [[ -n "${FB}" ]]; then
        echo "Extra argument: $1" >&2
        exit 1
      fi
      FB="$1"
      shift
      ;;
  esac
done

if [[ -z "${FB}" ]]; then
  shopt -s nullglob
  candidates=( "${ROOT}/packaging/out/${ARCH}/flatpak/"*.flatpak )
  shopt -u nullglob
  if ((${#candidates[@]} == 0)); then
    echo "No .flatpak in ${ROOT}/packaging/out/${ARCH}/flatpak/ — build one first (packaging/tools/build-flatpak.sh)" >&2
    exit 1
  fi
  FB="$(ls -t "${candidates[@]}" | head -1)"
fi

if [[ ! -f "${FB}" ]]; then
  echo "Not a file: ${FB}" >&2
  exit 1
fi

FB_ABS="$(cd "$(dirname "${FB}")" && pwd)/$(basename "${FB}")"

docker_host_path() {
  if command -v cygpath >/dev/null 2>&1; then
    cygpath -w "$1"
  else
    printf '%s' "$1"
  fi
}

_repo_path_for_docker() {
  local p="$1"
  if [[ -n "${VD_DOCKER_BIND_SRC:-}" ]] && [[ "${p}" == "${ROOT}/"* ]]; then
    printf '%s' "${VD_DOCKER_BIND_SRC}/${p#${ROOT}/}"
  else
    printf '%s' "${p}"
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

FLATPAK_SMOKE_BASE_IMAGE="${VD_FLATPAK_SMOKE_BASE_IMAGE:-videodedup/flatpak-smoke-fedora:40}"

ensure_flatpak_smoke_base_image() {
  local tag="$1"
  # Stock images: no custom Dockerfile in-repo for those tags.
  case "${tag}" in
    fedora:40) return 0 ;;
  esac
  local dockerfile="${ROOT}/packaging/docker/Dockerfile.flatpak-smoke-fedora"
  if [[ "${VD_REBUILD_FLATPAK_SMOKE_BASE:-0}" == "1" ]] || ! MSYS2_ARG_CONV_EXCL='*' docker image inspect "${tag}" >/dev/null 2>&1; then
    echo "Building flatpak-smoke base image ${tag} …"
    mkdir -p "${ROOT}/packaging/out"
    local ctx
    if ! ctx="$(mktemp -d "${ROOT}/packaging/out/.flatpak-smoke-base-ctx.XXXXXX" 2>/dev/null)"; then
      ctx="${ROOT}/packaging/out/.flatpak-smoke-base-ctx.$$"
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

ensure_flatpak_smoke_base_image "${FLATPAK_SMOKE_BASE_IMAGE}"

echo "Using ${FB_ABS} (image=${FLATPAK_SMOKE_BASE_IMAGE})"

FB_VOL="$(docker_host_path "$(_repo_path_for_docker "${FB_ABS}")")"

docker run --rm \
  -v "${FB_VOL}:/tmp/videodedupserver.flatpak:ro" \
  "${FLATPAK_SMOKE_BASE_IMAGE}" \
  bash -s <<'EOS'
set -eu
if [[ ! -f /etc/videodedup-flatpak-smoke-base ]]; then
  dnf -y -q install flatpak
  flatpak remote-add --if-not-exists flathub https://flathub.org/repo/flathub.flatpakrepo
  flatpak install -y --noninteractive flathub org.freedesktop.Platform//24.08
fi
flatpak install -y --noninteractive --bundle /tmp/videodedupserver.flatpak

install -d -m 0755 -o root -g root /tmp/vd-xdg
export XDG_RUNTIME_DIR=/tmp/vd-xdg
timeout 25s flatpak run io.github.sebastianbecker2.videodedup.server 2>&1 | tee /tmp/vd-smoke.log

if ! grep -qE "Now listening|Application started" /tmp/vd-smoke.log; then
  echo "--- smoke log ---" >&2
  cat /tmp/vd-smoke.log >&2
  exit 1
fi

echo "flatpak install smoke OK"
EOS

echo "Docker flatpak install test passed."
