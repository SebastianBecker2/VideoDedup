#!/usr/bin/env bash
# Install videodedupserver from a .deb inside Docker. By default builds/uses a small custom base image
# (packaging/docker/Dockerfile.grpc-smoke-base) so the container skips apt-get update on each run, then
# publish gRPC on host :51726 and run VideoDedupGrpcSmoke from the host (or a throwaway container if VD_SMOKE_IN_DOCKER=1).
# When grpc-smoke fixtures are mounted, also runs VideoDedupGrpcComparisonSmoke (DIFFERENT, DUPLICATE, then cancel).
#
# VideoDedupGrpcComparisonSmoke: (1) left vs right → DIFFERENT, (2) left vs left → DUPLICATE, (3) cancel → gRPC Cancelled or protobuf CANCELLED.
#
# If no matching .deb exists under packaging/out/<arch>/deb/, this script builds one using, in order:
#   1) Host: ./packaging/tools/stage.sh + ./packaging/tools/build-deb.sh (requires dotnet, python3,
#      fakeroot, dpkg-deb, git — typical on Debian/Ubuntu dev machines)
#   2) Docker: packaging/docker/Dockerfile.build-deb (BuildKit; works on Windows/macOS/Linux with Docker)
#
# Requirements: Docker. Host dotnet 8 SDK is optional: smoke DLLs are published with dotnet when on PATH, otherwise
# via mcr.microsoft.com/dotnet/sdk:8.0. For running smoke without host dotnet, see VD_SMOKE_IN_DOCKER=1 (runtime image).
#
# Each run publishes smoke tools to packaging/out/<arch>/e2e-smoke and (when comparison runs)
# e2e-comparison-smoke — not bin/Release. A plain "dotnet build" on the smoke csproj does not update
# those directories; this script republishes so your latest Program.cs is used.
# Usage (from repo root or any cwd):
#   ./packaging/tests/e2e/docker-grpc-deep-smoke.sh [--arch amd64|arm64] [--image IMAGE] [--deb PATH.deb] [--host-port PORT]
#
# Default: local image videodedup/grpc-smoke-base:trixie (built from Dockerfile.grpc-smoke-base on first use;
# Debian trixie FFmpeg matches FFmpeg.AutoGen 7.1 used by the server; bookworm FFmpeg is too old for comparison).
# Override with --image or VD_GRPC_SMOKE_IMAGE (e.g. debian:bookworm-slim for stock Debian + slow apt path).
# VD_GRPC_SMOKE_BASE_IMAGE — tag to build/use for the default base. VD_REBUILD_GRPC_SMOKE_BASE=1 — force docker build.
#
# Host port (default 51726): use --host-port or env VD_UBUNTU_SMOKE_HOST_PORT if 51726 is already taken on the machine.
#
# Examples:
#   ./packaging/tests/e2e/docker-grpc-deep-smoke.sh
#   ./packaging/tests/e2e/docker-grpc-deep-smoke.sh --arch arm64 --image ubuntu:24.04
#   ./packaging/tests/e2e/docker-grpc-deep-smoke.sh --deb ./packaging/out/amd64/deb/mypackage.deb
#
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../../.." && pwd)"
SCRIPT_PATH="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)/$(basename "${BASH_SOURCE[0]}")"
ARCH=""
OPT_IMAGE=""
EXPLICIT_DEB=""
HOST_GRPC_PORT="${VD_UBUNTU_SMOKE_HOST_PORT:-51726}"
SERVER_IMAGE=""

usage() {
  sed -n '2,38p' "${SCRIPT_PATH}"
  exit 0
}

while [[ $# -gt 0 ]]; do
  case "$1" in
    --arch)
      ARCH="$2"
      shift 2
      ;;
    --image)
      OPT_IMAGE="$2"
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

docker_host_path() {
  if command -v cygpath >/dev/null 2>&1; then
    cygpath -w "$1"
  else
    printf '%s' "$1"
  fi
}

# Paths relative to repo root (used with -w /src in the SDK container).
publish_smoke_project() {
  local rel_csproj="$1" out_subdir="$2"
  local out_dir="${ROOT}/packaging/out/${ARCH}/${out_subdir}"
  mkdir -p "${out_dir}"
  if command -v dotnet >/dev/null 2>&1; then
    dotnet publish "${ROOT}/${rel_csproj}" \
      -c Release -r "${DOTNET_RID}" --self-contained false \
      -o "${out_dir}"
  else
    echo "dotnet not on PATH; publishing ${rel_csproj} via mcr.microsoft.com/dotnet/sdk:8.0 …" >&2
    local root_vol
    root_vol="$(docker_host_path "${ROOT}")"
    MSYS2_ARG_CONV_EXCL='*' docker run --rm \
      -v "${root_vol}:/src:rw" \
      -w /src \
      mcr.microsoft.com/dotnet/sdk:8.0 \
      dotnet publish "${rel_csproj}" \
        -c Release -r "${DOTNET_RID}" --self-contained false \
        -o "packaging/out/${ARCH}/${out_subdir}"
  fi
}

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
  echo "Publishing VideoDedupGrpcSmoke to ${ROOT}/packaging/out/${ARCH}/e2e-smoke …"
  publish_smoke_project "VideoDedupGrpcSmoke/VideoDedupGrpcSmoke.csproj" "e2e-smoke"
  SMOKE_ABS="$(cd "${ROOT}/packaging/out/${ARCH}/e2e-smoke" && pwd)"
}

publish_comparison_smoke_if_needed() {
  echo "Publishing VideoDedupGrpcComparisonSmoke to ${ROOT}/packaging/out/${ARCH}/e2e-comparison-smoke …"
  publish_smoke_project "VideoDedupGrpcComparisonSmoke/VideoDedupGrpcComparisonSmoke.csproj" "e2e-comparison-smoke"
  COMPARISON_SMOKE_ABS="$(cd "${ROOT}/packaging/out/${ARCH}/e2e-comparison-smoke" && pwd)"
}

if ! command -v docker >/dev/null 2>&1; then
  echo "docker not found" >&2
  exit 1
fi
if ! docker info >/dev/null 2>&1; then
  echo "Docker daemon not reachable" >&2
  exit 1
fi

GRPC_SMOKE_BASE_IMAGE="${VD_GRPC_SMOKE_BASE_IMAGE:-videodedup/grpc-smoke-base:trixie}"

ensure_grpc_smoke_base_image() {
  local tag="$1"
  local dockerfile="${ROOT}/packaging/docker/Dockerfile.grpc-smoke-base"
  if [[ "${VD_REBUILD_GRPC_SMOKE_BASE:-0}" == "1" ]] || ! MSYS2_ARG_CONV_EXCL='*' docker image inspect "${tag}" >/dev/null 2>&1; then
    echo "Building grpc-smoke base image ${tag} …"
    mkdir -p "${ROOT}/packaging/out"
    # Build context must live under the repo tree on Windows + Docker Desktop: MSYS /tmp is not visible to the Linux engine.
    local ctx
    if ! ctx="$(mktemp -d "${ROOT}/packaging/out/.grpc-smoke-base-ctx.XXXXXX" 2>/dev/null)"; then
      ctx="${ROOT}/packaging/out/.grpc-smoke-base-ctx.$$"
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

if [[ -n "${OPT_IMAGE}" ]]; then
  SERVER_IMAGE="${OPT_IMAGE}"
elif [[ -n "${VD_GRPC_SMOKE_IMAGE:-}" ]]; then
  SERVER_IMAGE="${VD_GRPC_SMOKE_IMAGE}"
else
  ensure_grpc_smoke_base_image "${GRPC_SMOKE_BASE_IMAGE}"
  SERVER_IMAGE="${GRPC_SMOKE_BASE_IMAGE}"
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
  # Comparison smoke: DIFFERENT, DUPLICATE, then cancel-after-start (gRPC Cancelled or protobuf CANCELLED).
else
  echo "Note: grpc-smoke fixtures not found under ${FIXTURES_HOST_DIR}; comparison deep smoke will be skipped." >&2
fi

CONTAINER="videodedup-grpc-smoke-$$"
cleanup() {
  MSYS2_ARG_CONV_EXCL='*' docker rm -f "${CONTAINER}" >/dev/null 2>&1 || true
}
trap cleanup EXIT

case "${ARCH}" in
  amd64) VIDEODEDUP_FFMPEG_LIB_ROOT="/usr/lib/x86_64-linux-gnu" ;;
  arm64) VIDEODEDUP_FFMPEG_LIB_ROOT="/usr/lib/aarch64-linux-gnu" ;;
  *) VIDEODEDUP_FFMPEG_LIB_ROOT="/usr/lib/x86_64-linux-gnu" ;;
esac

echo "Starting ${CONTAINER} (image=${SERVER_IMAGE}, published localhost:${HOST_GRPC_PORT} -> container :51726) …"
MSYS2_ARG_CONV_EXCL='*' docker run -d --name "${CONTAINER}" --privileged \
  -p "${HOST_GRPC_PORT}:51726" \
  -e "VD_PACKAGE_FORMAT=deb" \
  -e "VD_FIREWALL=nft" \
  -e "VIDEODEDUP_FFMPEG_LIB_ROOT=${VIDEODEDUP_FFMPEG_LIB_ROOT}" \
  "${DOCKER_MOUNTS[@]}" \
  "${SERVER_IMAGE}" \
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

run_comparison_smoke() {
  local url="$1"
  if [[ "${VD_SMOKE_IN_DOCKER:-0}" != "1" ]] && command -v dotnet >/dev/null 2>&1; then
    VIDEODEDUP_SMOKE_COMPARE_LEFT="${VIDEODEDUP_SMOKE_COMPARE_LEFT}" \
    VIDEODEDUP_SMOKE_COMPARE_RIGHT="${VIDEODEDUP_SMOKE_COMPARE_RIGHT}" \
      dotnet "${COMPARISON_SMOKE_ABS}/VideoDedupGrpcComparisonSmoke.dll" "${url}"
    return
  fi
  echo "Running VideoComparison deep smoke inside mcr.microsoft.com/dotnet/runtime:8.0 (host has no dotnet; set VD_SMOKE_IN_DOCKER=1 to force) …"
  local smoke_url="${url}"
  if [[ "${smoke_url}" == *"127.0.0.1"* || "${smoke_url}" == *"localhost"* ]]; then
    smoke_url="${smoke_url//127.0.0.1/host.docker.internal}"
    smoke_url="${smoke_url//localhost/host.docker.internal}"
  fi
  MSYS2_ARG_CONV_EXCL='*' docker run --rm \
    --add-host=host.docker.internal:host-gateway \
    -e "VIDEODEDUP_SMOKE_COMPARE_LEFT=${VIDEODEDUP_SMOKE_COMPARE_LEFT}" \
    -e "VIDEODEDUP_SMOKE_COMPARE_RIGHT=${VIDEODEDUP_SMOKE_COMPARE_RIGHT}" \
    -v "${COMPARISON_SMOKE_VOL}:/comparison-smoke:ro" \
    mcr.microsoft.com/dotnet/runtime:8.0 \
    dotnet /comparison-smoke/VideoDedupGrpcComparisonSmoke.dll "${smoke_url}"
}

GRPC_SMOKE_URL="http://127.0.0.1:${HOST_GRPC_PORT}"
echo "Running gRPC smoke against ${GRPC_SMOKE_URL} …"
if ! run_smoke "${GRPC_SMOKE_URL}"; then
  echo "--- server container logs ---" >&2
  MSYS2_ARG_CONV_EXCL='*' docker logs "${CONTAINER}" >&2 || true
  exit 1
fi

if [[ -n "${VIDEODEDUP_SMOKE_COMPARE_LEFT}" && -n "${VIDEODEDUP_SMOKE_COMPARE_RIGHT}" ]]; then
  publish_comparison_smoke_if_needed
  COMPARISON_SMOKE_VOL="$(docker_host_path "${COMPARISON_SMOKE_ABS}")"
  echo "Running gRPC comparison deep smoke (DIFFERENT, DUPLICATE, cancel) against ${GRPC_SMOKE_URL} …"
  if ! run_comparison_smoke "${GRPC_SMOKE_URL}"; then
    echo "--- server container logs (comparison deep smoke failed) ---" >&2
    MSYS2_ARG_CONV_EXCL='*' docker logs "${CONTAINER}" >&2 || true
    exit 1
  fi
else
  echo "Skipping VideoComparison deep smoke (grpc-smoke fixtures not mounted / paths unset)." >&2
fi

echo "OK: Docker gRPC smoke passed (${SERVER_IMAGE}, ${PKG_ABS})."
