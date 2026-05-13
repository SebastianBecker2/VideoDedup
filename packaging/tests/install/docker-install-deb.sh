#!/usr/bin/env bash
# Smoke-test videodedupserver .deb inside Debian (Docker): apt install, file checks, short process run.
#
# Usage:
#   ./packaging/tests/install/docker-install-deb.sh [path/to/package.deb]
#   ./packaging/tests/install/docker-install-deb.sh --arch arm64
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../../.." && pwd)"
ARCH="amd64"
DEB=""

while [[ $# -gt 0 ]]; do
  case "$1" in
    --arch)
      ARCH="$2"
      shift 2
      ;;
    -h|--help)
      echo "Usage: $0 [--arch amd64|arm64] [path/to/videodedupserver_*.deb]"
      exit 0
      ;;
    *)
      if [[ -n "${DEB}" ]]; then
        echo "Extra argument: $1" >&2
        exit 1
      fi
      DEB="$1"
      shift
      ;;
  esac
done

if [[ -z "${DEB}" ]]; then
  shopt -s nullglob
  candidates=( "${ROOT}/packaging/out/${ARCH}/deb/"*.deb )
  shopt -u nullglob
  if ((${#candidates[@]} == 0)); then
    echo "No .deb in ${ROOT}/packaging/out/${ARCH}/deb/ — build one first (e.g. packaging/tools/build-deb.sh)" >&2
    exit 1
  fi
  # Newest by mtime
  DEB="$(ls -t "${candidates[@]}" | head -1)"
fi

if [[ ! -f "${DEB}" ]]; then
  echo "Not a file: ${DEB}" >&2
  exit 1
fi

DEB_ABS="$(cd "$(dirname "${DEB}")" && pwd)/$(basename "${DEB}")"

docker_host_path() {
  if command -v cygpath >/dev/null 2>&1; then
    cygpath -w "$1"
  else
    printf '%s' "$1"
  fi
}

# See docker-install-rpm.sh: nested docker needs host paths (VD_DOCKER_BIND_SRC).
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

echo "Using ${DEB_ABS}"

DEB_VOL="$(docker_host_path "$(_repo_path_for_docker "${DEB_ABS}")")"

docker run --rm \
  -v "${DEB_VOL}:/tmp/videodedupserver.deb:ro" \
  debian:bookworm-slim \
  bash -s <<'EOS'
set -eu
export DEBIAN_FRONTEND=noninteractive
apt-get update -qq
# Install local .deb and its Depends (ffmpeg, systemd, …)
apt-get install -y -qq /tmp/videodedupserver.deb

dpkg -l videodedupserver | grep -q ^ii

test -x /usr/lib/videodedupserver/VideoDedupService
test -f /usr/lib/videodedupserver/appsettings.json
grep -q "51726" /usr/lib/videodedupserver/appsettings.json
test -f /usr/lib/videodedupserver/firewall/open-port-nftables.sh
test -f /usr/lib/videodedupserver/firewall/configure-firewall-interactive.sh
test -f /usr/share/doc/videodedupserver/README.firewall
id videodedup >/dev/null

# No PID 1 systemd in plain Docker; run binary briefly as packaged user.
runuser -u videodedup -- env \
  ASPNETCORE_ENVIRONMENT=Production \
  DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1 \
  VIDEODEDUP_APP_DATA=/var/lib/videodedupserver \
  timeout 25s /usr/lib/videodedupserver/VideoDedupService 2>&1 | tee /tmp/vd-smoke.log

if ! grep -qE "Now listening|Application started" /tmp/vd-smoke.log; then
  echo "--- smoke log ---" >&2
  cat /tmp/vd-smoke.log >&2
  exit 1
fi

echo "deb install smoke OK"
EOS

echo "Docker .deb install test passed."
