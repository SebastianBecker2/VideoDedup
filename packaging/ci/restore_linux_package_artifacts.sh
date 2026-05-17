#!/usr/bin/env bash
# Reassemble packaging/out from split GitHub Actions artifacts (linux-*-<arch>-<sha>).
# download-artifact must use merge-multiple: false so each artifact has its own subdirectory.
set -euo pipefail

ARCH="${1:?arch required (e.g. amd64)}"
ART_ROOT="${2:-${RUNNER_TEMP}/vd-pkg-art}"

if [[ ! -d "${ART_ROOT}" ]]; then
  echo "Artifact root not found: ${ART_ROOT}" >&2
  exit 1
fi

find_artifact_dir() {
  local prefix="$1"
  find "${ART_ROOT}" -maxdepth 1 -type d -name "${prefix}*" -print -quit
}

mkdir -p "packaging/out/${ARCH}"

for kind in deb rpm pacman snap flatpak; do
  found="$(find_artifact_dir "linux-${kind}-${ARCH}-")"
  if [[ -z "${found}" ]]; then
    echo "Warning: no linux-${kind}-${ARCH} artifact under ${ART_ROOT}" >&2
    continue
  fi
  mkdir -p "packaging/out/${ARCH}/${kind}"
  cp -a "${found}/." "packaging/out/${ARCH}/${kind}/"
  echo "Restored ${kind} from ${found}"
done

found="$(find_artifact_dir "linux-metadata-${ARCH}-")"
if [[ -n "${found}" ]]; then
  cp -a "${found}/." packaging/out/
  echo "Restored metadata from ${found}"
fi

if [[ ! -f packaging/out/metadata.json ]]; then
  echo "packaging/out/metadata.json missing after restore" >&2
  echo "Contents of ${ART_ROOT}:" >&2
  ls -la "${ART_ROOT}" >&2 || true
  find "${ART_ROOT}" -maxdepth 3 >&2 || true
  exit 1
fi
