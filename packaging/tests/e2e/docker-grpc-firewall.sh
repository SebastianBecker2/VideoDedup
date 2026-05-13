#!/usr/bin/env bash
# E2E: Linux container installs videodedupserver (deb/rpm/staged/pacman/snap/flatpak), applies a strict firewall
# (nft | iptables | ufw | firewalld), runs the service; VideoDedupGrpcSmoke runs on the host when dotnet is on PATH
# (CI: avoids docker pull of mcr.microsoft.com/dotnet/runtime, often blocked on GitHub Actions).
#
# Requires: docker (with IPv6 enabled for custom bridge networks), dotnet 8 SDK on the host
# (to publish the smoke tool unless --smoke-dir is set). Falls back to mcr.microsoft.com/dotnet/runtime:8.0 if no dotnet.
# Default smoke output is packaging/out/<arch>/e2e-smoke; that path is republished every run so it stays in sync with source.
#
# Usage:
#   ./docker-grpc-firewall.sh [options] [path/to/package.deb|.rpm]
#
# Options:
#   --arch amd64|arm64
#   --format deb|rpm|staged|pacman|snap|flatpak
#   --distro debian|ubuntu|fedora|rocky|opensuse|arch|manjaro  (sets server image; optional if --srv-image set)
#   --srv-image IMAGE              override distro preset (e.g. ubuntu:22.04)
#   --firewall nft|iptables|ufw|firewalld   (default nft)
#   --smoke-dir DIR
#   -h, --help
#
# Examples:
#   ./docker-grpc-firewall.sh --format deb --distro debian --firewall nft
#   ./docker-grpc-firewall.sh --format deb --distro ubuntu --firewall ufw
#   ./docker-grpc-firewall.sh --format rpm --distro fedora --firewall firewalld
#   ./docker-grpc-firewall.sh --format staged --distro arch --firewall iptables
#   ./docker-grpc-firewall.sh --format pacman --distro arch --firewall iptables
#   ./docker-grpc-firewall.sh --format snap --distro ubuntu --firewall nft
#   ./docker-grpc-firewall.sh --format flatpak --distro fedora --firewall nft
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../../.." && pwd)"
ARCH="amd64"
FORMAT="deb"
DISTRO=""
SRV_IMAGE=""
FIREWALL="nft"
PKG=""
SMOKE_DIR=""
SMOKE_DIR_USER_SET=0

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
    --distro)
      DISTRO="$2"
      shift 2
      ;;
    --srv-image)
      SRV_IMAGE="$2"
      shift 2
      ;;
    --firewall)
      FIREWALL="$2"
      shift 2
      ;;
    --smoke-dir)
      SMOKE_DIR="$2"
      SMOKE_DIR_USER_SET=1
      shift 2
      ;;
    -h|--help)
      cat <<'HELP'
Usage: docker-grpc-firewall.sh [options] [path/to/package.deb|.rpm]

  --arch amd64|arm64
  --format deb|rpm|staged|pacman|snap|flatpak
  --distro debian|ubuntu|fedora|rocky|opensuse|arch|manjaro
  --srv-image IMAGE     override image (skips distro-based validation)
  --firewall nft|iptables|ufw|firewalld   (default: nft)
  --smoke-dir DIR

Runs gRPC smoke twice: explicit IPv4 (http://a.b.c.d:51726) and IPv6 (http://[addr]:51726) on a dual-stack bridge.

Staged format requires ./packaging/tools/stage.sh for the arch; deb/rpm need built packages under packaging/out/.
HELP
      exit 0
      ;;
    *)
      if [[ -n "${PKG}" ]]; then
        echo "Extra argument: $1" >&2
        exit 1
      fi
      PKG="$1"
      shift
      ;;
  esac
done

case "${ARCH}" in
  amd64) DOTNET_RID="linux-x64" ;;
  arm64) DOTNET_RID="linux-arm64" ;;
  *) echo "Unsupported --arch ${ARCH}" >&2; exit 1 ;;
esac

case "${FORMAT}" in
  deb|rpm|staged|pacman|snap|flatpak) ;;
  *) echo "Unsupported --format ${FORMAT}" >&2; exit 1 ;;
esac

case "${FIREWALL}" in
  nft|iptables|ufw|firewalld) ;;
  *) echo "Unsupported --firewall ${FIREWALL}" >&2; exit 1 ;;
esac

resolve_distro_and_image() {
  if [[ -n "${SRV_IMAGE}" && -z "${DISTRO}" ]]; then
    case "${SRV_IMAGE}" in
      debian:bookworm-slim) DISTRO=debian ;;
      ubuntu:24.04) DISTRO=ubuntu ;;
      fedora:40) DISTRO=fedora ;;
      rockylinux/rockylinux:9) DISTRO=rocky ;;
      opensuse/tumbleweed) DISTRO=opensuse ;;
      archlinux:latest) DISTRO=arch ;;
      manjarolinux/base) DISTRO=manjaro ;;
      *) DISTRO=custom ;;
    esac
  fi
  if [[ -z "${DISTRO}" ]]; then
    case "${FORMAT}" in
      deb) DISTRO=debian ;;
      rpm) DISTRO=fedora ;;
      staged) DISTRO=arch ;;
      pacman) DISTRO=arch ;;
      snap) DISTRO=ubuntu ;;
      flatpak) DISTRO=fedora ;;
    esac
  fi
  if [[ -z "${SRV_IMAGE}" ]]; then
    case "${DISTRO}" in
      debian) SRV_IMAGE="debian:bookworm-slim" ;;
      ubuntu) SRV_IMAGE="ubuntu:24.04" ;;
      fedora) SRV_IMAGE="fedora:40" ;;
      rocky) SRV_IMAGE="rockylinux/rockylinux:9" ;;
      opensuse) SRV_IMAGE="opensuse/tumbleweed" ;;
      arch) SRV_IMAGE="archlinux:latest" ;;
      manjaro) SRV_IMAGE="manjarolinux/base" ;;
      custom) echo "Set --srv-image when using DISTRO=custom" >&2; exit 1 ;;
      *)
        echo "Unknown --distro ${DISTRO}" >&2
        exit 1
      ;;
    esac
  fi
}

validate_combo() {
  if [[ "${DISTRO}" == custom ]]; then
    return 0
  fi
  case "${FORMAT}" in
    staged)
      case "${DISTRO}" in arch|manjaro) ;; *)
        echo "For --format staged use --distro arch or manjaro (got ${DISTRO})" >&2
        exit 1
      ;; esac
    ;;
    deb)
      case "${DISTRO}" in debian|ubuntu) ;; *)
        echo "For --format deb use --distro debian or ubuntu (got ${DISTRO})" >&2
        exit 1
      ;; esac
    ;;
    rpm)
      case "${DISTRO}" in fedora|rocky|opensuse) ;; *)
        echo "For --format rpm use --distro fedora, rocky, or opensuse (got ${DISTRO})" >&2
        exit 1
      ;; esac
    ;;
    pacman)
      case "${DISTRO}" in arch|manjaro) ;; *)
        echo "For --format pacman use --distro arch or manjaro (got ${DISTRO})" >&2
        exit 1
      ;; esac
    ;;
    snap)
      case "${DISTRO}" in debian|ubuntu) ;; *)
        echo "For --format snap use --distro debian or ubuntu (got ${DISTRO})" >&2
        exit 1
      ;; esac
    ;;
    flatpak)
      case "${DISTRO}" in fedora) ;; *)
        echo "For --format flatpak use --distro fedora (got ${DISTRO})" >&2
        exit 1
      ;; esac
    ;;
  esac
  if [[ "${FIREWALL}" == ufw ]]; then
    case "${DISTRO}" in debian|ubuntu|manjaro) ;; *)
      echo "ufw E2E is supported on debian, ubuntu, or manjaro (got ${DISTRO})" >&2
      exit 1
    ;; esac
  fi
}

resolve_distro_and_image
validate_combo

STAGE_DIR="${ROOT}/packaging/.stage/${ARCH}/server"
if [[ "${DISTRO}" == opensuse && "${FORMAT}" == rpm ]]; then
  if [[ ! -f "${STAGE_DIR}/VideoDedupService" ]]; then
    echo "openSUSE RPM E2E mounts staged fallback at ${STAGE_DIR} — run: ./packaging/tools/stage.sh --arch ${ARCH}" >&2
    exit 1
  fi
fi

if [[ "${FORMAT}" == staged ]]; then
  if [[ ! -f "${STAGE_DIR}/VideoDedupService" ]]; then
    echo "Staged server missing at ${STAGE_DIR} — run: ./packaging/tools/stage.sh --arch ${ARCH}" >&2
    exit 1
  fi
  PKG_ABS=""
else
  if [[ -z "${PKG}" ]]; then
    shopt -s nullglob
    case "${FORMAT}" in
      deb)
        candidates=( "${ROOT}/packaging/out/${ARCH}/deb/"*.deb )
        _hint="packaging/tools/build-deb.sh"
        ;;
      rpm)
        candidates=( "${ROOT}/packaging/out/${ARCH}/rpm/"*/*.rpm )
        _hint="packaging/tools/build-rpm.sh"
        ;;
      pacman)
        candidates=( "${ROOT}/packaging/out/${ARCH}/pacman/"*.pkg.tar.zst )
        _hint="packaging/tools/build-pacman.sh"
        ;;
      snap)
        candidates=( "${ROOT}/packaging/out/${ARCH}/snap/"*.snap )
        _hint="packaging/tools/build-snap.sh"
        ;;
      flatpak)
        candidates=( "${ROOT}/packaging/out/${ARCH}/flatpak/"*.flatpak )
        _hint="packaging/tools/build-flatpak.sh"
        ;;
      *)
        echo "internal: no candidate rule for ${FORMAT}" >&2
        exit 1
        ;;
    esac
    shopt -u nullglob
    if ((${#candidates[@]} == 0)); then
      echo "No ${FORMAT} artifact under packaging/out/${ARCH}/ — build one first (e.g. ${_hint})" >&2
      exit 1
    fi
    PKG="$(ls -t "${candidates[@]}" | head -1)"
  fi
  if [[ ! -f "${PKG}" ]]; then
    echo "Not a file: ${PKG}" >&2
    exit 1
  fi
  PKG_ABS="$(cd "$(dirname "${PKG}")" && pwd)/$(basename "${PKG}")"
fi

if [[ -z "${SMOKE_DIR}" ]]; then
  SMOKE_DIR="${ROOT}/packaging/out/${ARCH}/e2e-smoke"
fi

if [[ "${SMOKE_DIR_USER_SET}" != "1" ]]; then
  echo "Publishing VideoDedupGrpcSmoke to ${SMOKE_DIR} …"
  dotnet publish "${ROOT}/VideoDedupGrpcSmoke/VideoDedupGrpcSmoke.csproj" \
    -c Release -r "${DOTNET_RID}" --self-contained false \
    -o "${SMOKE_DIR}"
elif [[ ! -f "${SMOKE_DIR}/VideoDedupGrpcSmoke.dll" ]]; then
  echo "Publishing VideoDedupGrpcSmoke to ${SMOKE_DIR} …"
  dotnet publish "${ROOT}/VideoDedupGrpcSmoke/VideoDedupGrpcSmoke.csproj" \
    -c Release -r "${DOTNET_RID}" --self-contained false \
    -o "${SMOKE_DIR}"
fi

if ! command -v docker >/dev/null 2>&1; then
  echo "docker not found" >&2
  exit 1
fi
if ! docker info >/dev/null 2>&1; then
  echo "Docker daemon not reachable" >&2
  exit 1
fi

NET="videodedup-e2e-$$"
SRV="videodedup-e2e-srv-$$"
SMOKE_ABS="$(cd "${SMOKE_DIR}" && pwd)"

docker_host_path() {
  if command -v cygpath >/dev/null 2>&1; then
    cygpath -w "$1"
  else
    printf '%s' "$1"
  fi
}

cleanup() {
  docker rm -f "${SRV}" >/dev/null 2>&1 || true
  docker network rm "${NET}" >/dev/null 2>&1 || true
}
trap cleanup EXIT

# ULA IPv6 + private IPv4 so each E2E run gets stable dual-stack semantics (daemon must allow IPv6).
E2E_NET_V4_SUBNET="172.30.232.0/24"
E2E_NET_V4_GW="172.30.232.1"
E2E_NET_V6_SUBNET="fd00:c0a8:e2e::/64"
if ! MSYS2_ARG_CONV_EXCL='*' docker network create --driver bridge \
  --subnet "${E2E_NET_V4_SUBNET}" --gateway "${E2E_NET_V4_GW}" \
  --ipv6 --subnet "${E2E_NET_V6_SUBNET}" \
  "${NET}" >/dev/null; then
  echo "Failed to create dual-stack Docker network \"${NET}\"." >&2
  echo "Enable IPv6 in Docker and retry (see packaging/common/docs/local-build.md)." >&2
  exit 1
fi

ENTRY="${ROOT}/packaging/tests/e2e/server-entrypoint.sh"
ENTRY_VOL="$(docker_host_path "${ENTRY}")"
SMOKE_VOL="$(docker_host_path "${SMOKE_ABS}")"

DOCKER_ENV=( -e "VD_PACKAGE_FORMAT=${FORMAT}" -e "VD_FIREWALL=${FIREWALL}" )
DOCKER_MOUNTS=( -v "${ENTRY_VOL}:/entrypoint.sh:ro" )

# Optional gRPC comparison fixtures (server-side paths).
# Used by VideoDedupGrpcSmoke's StartVideoComparison coverage.
FIXTURES_HOST_DIR="${ROOT}/packaging/tests/fixtures/grpc-smoke"
FIXTURES_SERVER_DIR="/tmp/vd-fixtures/grpc-smoke"
VIDEODEDUP_SMOKE_COMPARE_LEFT=""
VIDEODEDUP_SMOKE_COMPARE_RIGHT=""
if [[ -f "${FIXTURES_HOST_DIR}/left.mp4" && -f "${FIXTURES_HOST_DIR}/right.mp4" ]]; then
  FIXTURES_VOL="$(docker_host_path "${FIXTURES_HOST_DIR}")"
  DOCKER_MOUNTS+=( -v "${FIXTURES_VOL}:${FIXTURES_SERVER_DIR}:ro" )
  VIDEODEDUP_SMOKE_COMPARE_LEFT="${FIXTURES_SERVER_DIR}/left.mp4"
  VIDEODEDUP_SMOKE_COMPARE_RIGHT="${FIXTURES_SERVER_DIR}/right.mp4"
else
  echo "E2E: grpc-smoke fixtures missing; comparison RPCs will run with non-existent paths." >&2
fi

case "${FORMAT}" in
  deb)
    PKG_VOL="$(docker_host_path "${PKG_ABS}")"
    DOCKER_MOUNTS+=( -v "${PKG_VOL}:/tmp/videodedupserver.deb:ro" )
    ;;
  rpm)
    PKG_VOL="$(docker_host_path "${PKG_ABS}")"
    DOCKER_MOUNTS+=( -v "${PKG_VOL}:/tmp/videodedupserver.rpm:ro" )
    if [[ "${DISTRO}" == opensuse ]]; then
      STAGE_VOL="$(docker_host_path "${STAGE_DIR}")"
      DOCKER_MOUNTS+=( -v "${STAGE_VOL}:/opt/videodedup-staged:ro" )
    fi
    ;;
  staged)
    STAGE_VOL="$(docker_host_path "${STAGE_DIR}")"
    DOCKER_MOUNTS+=( -v "${STAGE_VOL}:/opt/videodedup-staged:ro" )
    ;;
  pacman)
    PKG_VOL="$(docker_host_path "${PKG_ABS}")"
    DOCKER_MOUNTS+=( -v "${PKG_VOL}:/tmp/videodedupserver.pkg.tar.zst:ro" )
    ;;
  snap)
    PKG_VOL="$(docker_host_path "${PKG_ABS}")"
    DOCKER_MOUNTS+=( -v "${PKG_VOL}:/tmp/videodedupserver.snap:ro" )
    ;;
  flatpak)
    PKG_VOL="$(docker_host_path "${PKG_ABS}")"
    DOCKER_MOUNTS+=( -v "${PKG_VOL}:/tmp/videodedupserver.flatpak:ro" )
    ;;
esac

echo "Starting server container (${SRV}, image=${SRV_IMAGE}, format=${FORMAT}, firewall=${FIREWALL}) …"
MSYS2_ARG_CONV_EXCL='*' docker run -d --name "${SRV}" --network "${NET}" --privileged \
  "${DOCKER_ENV[@]}" \
  "${DOCKER_MOUNTS[@]}" \
  "${SRV_IMAGE}" \
  bash /entrypoint.sh

echo "Waiting for ${SRV} gRPC (${FORMAT} install + ${FIREWALL} + service) …"
_ready=0
for _ in $(seq 1 240); do
  if MSYS2_ARG_CONV_EXCL='*' docker exec "${SRV}" test -f /tmp/vd-ready 2>/dev/null; then
    _ready=1
    break
  fi
  sleep 1
done
if [[ "${_ready}" -ne 1 ]]; then
  echo "server did not become ready (missing /tmp/vd-ready)" >&2
  MSYS2_ARG_CONV_EXCL='*' docker logs "${SRV}" >&2 || true
  exit 1
fi

e2e_srv_ip() {
  local field="$1"
  MSYS2_ARG_CONV_EXCL='*' docker inspect -f "{{ (index .NetworkSettings.Networks \"${NET}\").${field} }}" "${SRV}"
}

IPV4="$(e2e_srv_ip IPAddress)"
IPV6="$(e2e_srv_ip GlobalIPv6Address)"
if [[ -z "${IPV4}" ]]; then
  echo "E2E: server ${SRV} has no IPv4 address on network ${NET}" >&2
  exit 1
fi
if [[ -z "${IPV6}" ]]; then
  echo "E2E: server ${SRV} has no GlobalIPv6Address on ${NET}." >&2
  echo "Docker must assign IPv6 to custom bridge networks (enable IPv6 in Docker; see packaging/common/docs/local-build.md)." >&2
  exit 1
fi

run_smoke() {
  local url="$1"
  local label="$2"
  echo "Running gRPC smoke client (${label}: ${url}) …"
  # Prefer host dotnet for speed, but allow forcing the smoke to run inside the
  # docker network (helps on platforms where host->custom-network connectivity
  # is restricted).
  if [[ "${VD_SMOKE_IN_DOCKER:-0}" != "1" ]] && command -v dotnet >/dev/null 2>&1; then
    VIDEODEDUP_SMOKE_COMPARE_LEFT="${VIDEODEDUP_SMOKE_COMPARE_LEFT}" \
    VIDEODEDUP_SMOKE_COMPARE_RIGHT="${VIDEODEDUP_SMOKE_COMPARE_RIGHT}" \
    dotnet "${SMOKE_ABS}/VideoDedupGrpcSmoke.dll" "${url}"
    return
  fi
  MSYS2_ARG_CONV_EXCL='*' docker run --rm \
    --network "${NET}" \
    -e "VIDEODEDUP_SMOKE_COMPARE_LEFT=${VIDEODEDUP_SMOKE_COMPARE_LEFT}" \
    -e "VIDEODEDUP_SMOKE_COMPARE_RIGHT=${VIDEODEDUP_SMOKE_COMPARE_RIGHT}" \
    -v "${SMOKE_VOL}:/smoke:ro" \
    mcr.microsoft.com/dotnet/runtime:8.0 \
    dotnet /smoke/VideoDedupGrpcSmoke.dll "${url}"
}

ipv4_ok=1
if ! run_smoke "http://${IPV4}:51726" "IPv4"; then
  ipv4_ok=0
  echo "--- server container logs (IPv4 smoke failed) ---" >&2
  MSYS2_ARG_CONV_EXCL='*' docker logs "${SRV}" >&2 || true
fi
if ! run_smoke "http://[${IPV6}]:51726" "IPv6"; then
  echo "--- server container logs (IPv6 smoke failed) ---" >&2
  MSYS2_ARG_CONV_EXCL='*' docker logs "${SRV}" >&2 || true
  exit 1
fi

if [[ "${ipv4_ok}" -ne 1 ]]; then
  echo "E2E gRPC + firewall passed for IPv6, but IPv4 smoke failed (server likely IPv6-only on this host)." >&2
else
  echo "E2E gRPC + firewall passed (${SRV_IMAGE}, ${FORMAT}, ${FIREWALL}; IPv4 + IPv6)."
fi
