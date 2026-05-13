#!/usr/bin/env bash
# Run inside the Ubuntu container started by run-full-linux-build-docker.sh (repo mounted at /src).
# Must use LF line endings (see .gitattributes packaging/tools/*.sh); CRLF breaks `set -o pipefail`.
#
# When the image was built from packaging/docker/Dockerfile.packaging-worker-ubuntu24, /etc/videodedup-packaging-worker-base
# exists and the heavy apt-get block is skipped (flatpak packages are still installed when VD_INCLUDE_FLATPAK=1).
#
# Flatpak: skipped by default (flatpak-builder often fails under Docker Desktop / some hosts).
# Set VD_INCLUDE_FLATPAK=1 (e.g. from run-full-linux-build-docker.sh --include-flatpak) for CI parity.
set -euxo pipefail
cd /src

ARCH="${ARCH:-amd64}"
INCLUDE_FLATPAK="${VD_INCLUDE_FLATPAK:-0}"
case "${INCLUDE_FLATPAK}" in 1|true|yes|YES) INCLUDE_FLATPAK=1 ;; *) INCLUDE_FLATPAK=0 ;; esac

if [[ -f /etc/videodedup-packaging-worker-base ]]; then
  echo "Using packaging worker base (skipping bulk apt-get from inner script)" >&2
  if [[ "${INCLUDE_FLATPAK}" -eq 1 ]]; then
    apt-get update -qq
    DEBIAN_FRONTEND=noninteractive apt-get install -y -qq --no-install-recommends \
      flatpak flatpak-builder
  fi
else
  apt-get update -qq
  DEBIAN_FRONTEND=noninteractive apt-get install -y -qq -f

  # Split installs: one big line often triggers "held broken packages" in minimal images when
  # flatpak recommends pull a full desktop stack that is not wanted in Docker.
  DEBIAN_FRONTEND=noninteractive apt-get install -y -qq \
    ca-certificates curl git python3 \
    fakeroot dpkg-dev lintian rpm rpmlint

  DEBIAN_FRONTEND=noninteractive apt-get install -y -qq docker.io

  if [[ "${INCLUDE_FLATPAK}" -eq 1 ]]; then
    DEBIAN_FRONTEND=noninteractive apt-get install -y -qq --no-install-recommends \
      flatpak flatpak-builder
  else
    echo "Skipping flatpak / flatpak-builder install (set VD_INCLUDE_FLATPAK=1 for Flatpak build + tests)" >&2
  fi

  DEBIAN_FRONTEND=noninteractive apt-get install -y -qq -f
fi

export SOURCE_DATE_EPOCH="${SOURCE_DATE_EPOCH:-$(git log -1 --format=%ct)}"

if ! command -v dotnet >/dev/null 2>&1; then
  curl -fsSL https://dot.net/v1/dotnet-install.sh -o /tmp/dotnet-install.sh
  bash /tmp/dotnet-install.sh --channel 8.0 --install-dir /usr/share/dotnet
fi
export PATH="/usr/share/dotnet:${PATH}"
dotnet --version

if [[ "${INCLUDE_FLATPAK}" -eq 1 ]]; then
  flatpak remote-add --user --if-not-exists flathub https://flathub.org/repo/flathub.flatpakrepo
  flatpak install -y --user flathub \
    org.freedesktop.Platform//24.08 \
    org.freedesktop.Sdk//24.08 \
    org.freedesktop.Sdk.Extension.dotnet8//24.08
fi

# Snap uses systemd (e.g. systemctl daemon-reload) during install; a plain Ubuntu container is
# not PID1 systemd, so snapd/snap install snapcraft is unreliable. Build the snap in Canonical's
# snapcraft image instead (matches base: core24 → tag 8_core24). Override with SNAPCRAFT_IMAGE.
_vd_run_snapcraft_docker() {
  local img="${SNAPCRAFT_IMAGE:-ghcr.io/canonical/snapcraft:8_core24}"
  docker run --rm --privileged \
    -v /var/run/docker.sock:/var/run/docker.sock \
    -v "${VD_DOCKER_BIND_SRC:?VD_DOCKER_BIND_SRC unset; use run-full-linux-build-docker.sh}:/src:rw" \
    -w /src \
    -e "ARCH=${ARCH}" \
    -e "SOURCE_DATE_EPOCH=${SOURCE_DATE_EPOCH}" \
    --entrypoint bash \
    "${img}" \
    -c 'set -euxo pipefail
        cd /src
        ./packaging/tools/build-snap.sh --arch "${ARCH}" --require-snapcraft'
}

chmod +x packaging/tools/*.sh packaging/common/generate-metadata.sh packaging/tests/*.sh \
  packaging/tests/firewall/*.sh packaging/tests/install/*.sh packaging/tests/e2e/*.sh \
  packaging/tools/sbom-and-attest.sh 2>/dev/null || true

./packaging/tools/stage.sh --arch "${ARCH}"
./packaging/tools/build-deb.sh --arch "${ARCH}"
./packaging/tools/build-rpm.sh --arch "${ARCH}"
./packaging/tools/build-pacman.sh --arch "${ARCH}"
if [[ "${INCLUDE_FLATPAK}" -eq 1 ]]; then
  ./packaging/tools/build-flatpak.sh --arch "${ARCH}" --require-flatpak-builder
fi
_vd_run_snapcraft_docker
./packaging/tools/write-checksums.sh --arch "${ARCH}"

./packaging/tests/run-package-tests.sh "${ARCH}"
./packaging/tests/firewall/docker-run-all.sh --integration
./packaging/tests/install/docker-install-deb.sh --arch "${ARCH}"
./packaging/tests/install/docker-install-rpm.sh --arch "${ARCH}"
./packaging/tests/install/docker-install-pacman.sh --arch "${ARCH}"
if [[ "${INCLUDE_FLATPAK}" -eq 1 ]]; then
  ./packaging/tests/install/docker-install-flatpak.sh --arch "${ARCH}"
fi

echo "=== run-full-linux-build-docker: ALL_STEPS_DONE ==="
