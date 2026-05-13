#!/usr/bin/env bash
# Install videodedupserver from a .deb inside an Ubuntu Docker container, publish gRPC on host :51726,
# then run VideoDedupGrpcSmoke from the host (or from a throwaway container if VD_SMOKE_IN_DOCKER=1).
#
# If no matching .deb exists under packaging/out/<arch>/deb/, this script builds one using, in order:
#   1) Host: ./packaging/tools/stage.sh + ./packaging/tools/build-deb.sh (requires dotnet, python3,
#      fakeroot, dpkg-deb, git — typical on Debian/Ubuntu dev machines)
#   2) Docker: packaging/docker/Dockerfile.build-deb (BuildKit; works on Windows/macOS/Linux with Docker)
#
# Requirements: Docker; dotnet 8 SDK on the host for the smoke client unless VD_SMOKE_IN_DOCKER=1.
#
# Usage (from repo root or any cwd):
#   ./packaging/tests/e2e/docker-ubuntu-grpc-smoke.sh [--arch amd64|arm64] [--image ubuntu:24.04] [--deb PATH.deb] [--host-port PORT]
#
# Host port (default 51726): use --host-port or env VD_UBUNTU_SMOKE_HOST_PORT if 51726 is already taken on the machine.
#
# Examples:
#   ./packaging/tests/e2e/docker-ubuntu-grpc-smoke.sh
#   ./packaging/tests/e2e/docker-ubuntu-grpc-smoke.sh --arch arm64 --image ubuntu:24.04
#   ./packaging/tests/e2e/docker-ubuntu-grpc-smoke.sh --deb ./packaging/out/amd64/deb/mypackage.deb
#
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../../.." && pwd)"
SCRIPT_PATH="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)/$(basename "${BASH_SOURCE[0]}")"
ARCH=""
UBUNTU_IMAGE="ubuntu:24.04"
EXPLICIT_DEB=""
HOST_GRPC_PORT="${VD_UBUNTU_SMOKE_HOST_PORT:-51726}"

usage() {
  sed -n '2,20p' "${SCRIPT_PATH}"
  exit 0
}

while [[ $# -gt 0 ]]; do
  case "$1" in
    --arch)
      ARCH="$2"
      shift 2
      ;;
    --image)
      UBUNTU_IMAGE="$2"
      shift 2
      ;;
    --deb)
      EXPLICIT_DEB="$2"
      shift 2
      ;;
    --host-port)
      HOST_GRPC_PORT="$2"
      shift 2
      ;;
    -h|--help)
      usage
      ;;
    *)
      echo "Unknown option: $1 (try --help)" >&2
      exit 1
      ;;
  esac
done

if [[ -z "${ARCH}" ]]; then
  case "$(uname -m 2>/dev/null || echo amd64)" in
    x86_64|amd64) ARCH=amd64 ;;
    aarch64|arm64) ARCH=arm64 ;;
    *) ARCH=amd64 ;;
  esac
fi

case "${ARCH}" in
  amd64) DOTNET_RID="linux-x64" ;;
  arm64) DOTNET_RID="linux-arm64" ;;
  *) echo "Unsupported --arch ${ARCH} (use amd64 or arm64)" >&2; exit 1 ;;
esac

if ! [[ "${HOST_GRPC_PORT}" =~ ^[0-9]+$ ]] || [[ "${HOST_GRPC_PORT}" -lt 1 || "${HOST_GRPC_PORT}" -gt 65535 ]]; then
  echo "Invalid host port: ${HOST_GRPC_PORT}" >&2
  exit 1
fi

host_can_build_deb() {
  local c
  for c in dotnet git python3 fakeroot dpkg-deb; do
    command -v "${c}" >/dev/null 2>&1 || return 1
  done
  return 0
}

build_deb_on_host() {
  echo "Building .deb on host (stage + build-deb, arch=${ARCH}) …"
  ( cd "${ROOT}" && chmod +x packaging/tools/*.sh packaging/common/generate-metadata.sh 2>/dev/null || true
    export SOURCE_DATE_EPOCH="${SOURCE_DATE_EPOCH:-$(git -C "${ROOT}" log -1 --format=%ct 2>/dev/null || echo 0)}"
    "${ROOT}/packaging/tools/stage.sh" --arch "${ARCH}"
    "${ROOT}/packaging/tools/build-deb.sh" --arch "${ARCH}"
  )
}

build_deb_in_docker() {
  echo "Building .deb in Docker (${ROOT}/packaging/docker/Dockerfile.build-deb, arch=${ARCH}) …"
  local out_tmp
  out_tmp="$(mktemp -d "${TMPDIR:-/tmp}/vd-deb-docker.XXXXXX")"
  cleanup_tmp() { rm -rf "${out_tmp}"; }
  if ! DOCKER_BUILDKIT=1 docker build \
    -f "${ROOT}/packaging/docker/Dockerfile.build-deb" \
    --build-arg "ARCH=${ARCH}" \
    --target artifacts \
    --output "type=local,dest=${out_tmp}" \
    "${ROOT}"; then
    cleanup_tmp
    echo "Docker build of .deb failed." >&2
    return 1
  fi
  mkdir -p "${ROOT}/packaging/out/${ARCH}/deb"
  shopt -s nullglob
  local f found=0
  for f in "${out_tmp}"/*.deb; do
    cp -f "${f}" "${ROOT}/packaging/out/${ARCH}/deb/"
    echo "Wrote ${ROOT}/packaging/out/${ARCH}/deb/$(basename "${f}")"
    found=1
  done
  shopt -u nullglob
  cleanup_tmp
  if [[ "${found}" -ne 1 ]]; then
    echo "Docker build produced no .deb in output directory." >&2
    return 1
  fi
}

ensure_deb_package() {
  if [[ -n "${EXPLICIT_DEB}" ]]; then
    if [[ ! -f "${EXPLICIT_DEB}" ]]; then
      echo "Not a file: ${EXPLICIT_DEB}" >&2
      exit 1
    fi
    PKG_ABS="$(cd "$(dirname "${EXPLICIT_DEB}")" && pwd)/$(basename "${EXPLICIT_DEB}")"
    return 0
  fi

  shopt -s nullglob
  local candidates=( "${ROOT}/packaging/out/${ARCH}/deb/"*.deb )
  shopt -u nullglob
  if ((${#candidates[@]} > 0)); then
    PKG_ABS="$(ls -t "${candidates[@]}" | head -1)"
    echo "Using existing package: ${PKG_ABS}"
    return 0
  fi

  echo "No .deb under ${ROOT}/packaging/out/${ARCH}/deb/ — building …" >&2
  if host_can_build_deb; then
    build_deb_on_host
  elif command -v docker >/dev/null 2>&1 && docker info >/dev/null 2>&1; then
    build_deb_in_docker
  else
    echo "Cannot build .deb: install dotnet/git/python3/fakeroot/dpkg-deb, or install/start Docker." >&2
    echo "  Host: ./packaging/tools/build-deb-one-shot.sh --arch ${ARCH}" >&2
    echo "  Docker: DOCKER_BUILDKIT=1 docker build -f packaging/docker/Dockerfile.build-deb --build-arg ARCH=${ARCH} --target artifacts -o type=local,dest=./deb-out ${ROOT}" >&2
    exit 1
  fi

  shopt -s nullglob
  candidates=( "${ROOT}/packaging/out/${ARCH}/deb/"*.deb )
  shopt -u nullglob
  if ((${#candidates[@]} == 0)); then
    echo "Build finished but no .deb found under packaging/out/${ARCH}/deb/" >&2
    exit 1
  fi
  PKG_ABS="$(ls -t "${candidates[@]}" | head -1)"
  echo "Built package: ${PKG_ABS}"
}

publish_smoke_if_needed() {
  local smoke_dir="${ROOT}/packaging/out/${ARCH}/e2e-smoke"
  if [[ ! -f "${smoke_dir}/VideoDedupGrpcSmoke.dll" ]]; then
    echo "Publishing VideoDedupGrpcSmoke to ${smoke_dir} …"
    dotnet publish "${ROOT}/VideoDedupGrpcSmoke/VideoDedupGrpcSmoke.csproj" \
      -c Release -r "${DOTNET_RID}" --self-contained false \
      -o "${smoke_dir}"
  fi
  SMOKE_ABS="$(cd "${smoke_dir}" && pwd)"
}

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

ensure_deb_package
publish_smoke_if_needed

ENTRY="${ROOT}/packaging/tests/e2e/server-entrypoint.sh"
ENTRY_VOL="$(docker_host_path "${ENTRY}")"
PKG_VOL="$(docker_host_path "${PKG_ABS}")"
SMOKE_VOL="$(docker_host_path "${SMOKE_ABS}")"

FIXTURES_HOST_DIR="${ROOT}/packaging/tests/fixtures/grpc-smoke"
FIXTURES_SERVER_DIR="/tmp/vd-fixtures/grpc-smoke"
VIDEODEDUP_SMOKE_COMPARE_LEFT=""
VIDEODEDUP_SMOKE_COMPARE_RIGHT=""
DOCKER_MOUNTS=( -v "${ENTRY_VOL}:/entrypoint.sh:ro" -v "${PKG_VOL}:/tmp/videodedupserver.deb:ro" )
if [[ -f "${FIXTURES_HOST_DIR}/left.mp4" && -f "${FIXTURES_HOST_DIR}/right.mp4" ]]; then
  DOCKER_MOUNTS+=( -v "$(docker_host_path "${FIXTURES_HOST_DIR}"):${FIXTURES_SERVER_DIR}:ro" )
  VIDEODEDUP_SMOKE_COMPARE_LEFT="${FIXTURES_SERVER_DIR}/left.mp4"
  VIDEODEDUP_SMOKE_COMPARE_RIGHT="${FIXTURES_SERVER_DIR}/right.mp4"
  echo "Using gRPC comparison fixtures at ${FIXTURES_SERVER_DIR} (in container)."
else
  echo "Note: grpc-smoke fixtures not found under ${FIXTURES_HOST_DIR}; comparison uses placeholder paths." >&2
fi

CONTAINER="videodedup-ubuntu-smoke-$$"
cleanup() {
  MSYS2_ARG_CONV_EXCL='*' docker rm -f "${CONTAINER}" >/dev/null 2>&1 || true
}
trap cleanup EXIT

echo "Starting ${CONTAINER} (image=${UBUNTU_IMAGE}, published localhost:${HOST_GRPC_PORT} -> container :51726) …"
MSYS2_ARG_CONV_EXCL='*' docker run -d --name "${CONTAINER}" --privileged \
  -p "${HOST_GRPC_PORT}:51726" \
  -e "VD_PACKAGE_FORMAT=deb" \
  -e "VD_FIREWALL=nft" \
  "${DOCKER_MOUNTS[@]}" \
  "${UBUNTU_IMAGE}" \
  bash /entrypoint.sh

echo "Waiting for gRPC readiness (/tmp/vd-ready in container) …"
_ready=0
for _ in $(seq 1 180); do
  if MSYS2_ARG_CONV_EXCL='*' docker exec "${CONTAINER}" test -f /tmp/vd-ready 2>/dev/null; then
    _ready=1
    break
  fi
  sleep 1
done
if [[ "${_ready}" -ne 1 ]]; then
  echo "Server did not become ready in time." >&2
  MSYS2_ARG_CONV_EXCL='*' docker logs "${CONTAINER}" >&2 || true
  exit 1
fi

run_smoke() {
  local url="$1"
  if [[ "${VD_SMOKE_IN_DOCKER:-0}" != "1" ]] && command -v dotnet >/dev/null 2>&1; then
    VIDEODEDUP_SMOKE_COMPARE_LEFT="${VIDEODEDUP_SMOKE_COMPARE_LEFT}" \
    VIDEODEDUP_SMOKE_COMPARE_RIGHT="${VIDEODEDUP_SMOKE_COMPARE_RIGHT}" \
      dotnet "${SMOKE_ABS}/VideoDedupGrpcSmoke.dll" "${url}"
    return
  fi
  echo "Running smoke inside mcr.microsoft.com/dotnet/runtime:8.0 (host has no dotnet; set VD_SMOKE_IN_DOCKER=1 to force) …"
  local smoke_url="${url}"
  if [[ "${smoke_url}" == *"127.0.0.1"* || "${smoke_url}" == *"localhost"* ]]; then
    smoke_url="${smoke_url//127.0.0.1/host.docker.internal}"
    smoke_url="${smoke_url//localhost/host.docker.internal}"
  fi
  MSYS2_ARG_CONV_EXCL='*' docker run --rm \
    --add-host=host.docker.internal:host-gateway \
    -e "VIDEODEDUP_SMOKE_COMPARE_LEFT=${VIDEODEDUP_SMOKE_COMPARE_LEFT}" \
    -e "VIDEODEDUP_SMOKE_COMPARE_RIGHT=${VIDEODEDUP_SMOKE_COMPARE_RIGHT}" \
    -v "${SMOKE_VOL}:/smoke:ro" \
    mcr.microsoft.com/dotnet/runtime:8.0 \
    dotnet /smoke/VideoDedupGrpcSmoke.dll "${smoke_url}"
}

GRPC_SMOKE_URL="http://127.0.0.1:${HOST_GRPC_PORT}"
echo "Running gRPC smoke against ${GRPC_SMOKE_URL} …"
if ! run_smoke "${GRPC_SMOKE_URL}"; then
  echo "--- server container logs ---" >&2
  MSYS2_ARG_CONV_EXCL='*' docker logs "${CONTAINER}" >&2 || true
  exit 1
fi

echo "OK: Ubuntu Docker gRPC smoke passed (${UBUNTU_IMAGE}, ${PKG_ABS})."
