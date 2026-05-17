#!/usr/bin/env bash
# Reassemble packaging/out from split GitHub Actions artifacts (linux-*-<arch>-<sha>).
set -euo pipefail

ARCH="${1:?arch required (e.g. amd64)}"
ART_ROOT="${2:-${RUNNER_TEMP}/vd-pkg-art}"

if [[ ! -d "${ART_ROOT}" ]]; then
  echo "Artifact root not found: ${ART_ROOT}" >&2
  exit 1
fi

mkdir -p "packaging/out/${ARCH}"

for kind in deb rpm pacman snap flatpak; do
  found=""
  shopt -s nullglob
  for d in "${ART_ROOT}"/linux-"${kind}"-"${ARCH}"-*; do
    if [[ -d "${d}" ]]; then
      found="${d}"
      break
    fi
  done
  shopt -u nullglob
  if [[ -z "${found}" ]]; then
    echo "Warning: no linux-${kind}-${ARCH} artifact under ${ART_ROOT}" >&2
    continue
  fi
  mkdir -p "packaging/out/${ARCH}/${kind}"
  cp -a "${found}/." "packaging/out/${ARCH}/${kind}/"
  echo "Restored ${kind} from ${found}"
done

shopt -s nullglob
for d in "${ART_ROOT}"/linux-metadata-"${ARCH}"-*; do
  if [[ -d "${d}" ]]; then
    cp -a "${d}/." packaging/out/
    echo "Restored metadata from ${d}"
    break
  fi
done
shopt -u nullglob

if [[ ! -f packaging/out/metadata.json ]]; then
  echo "packaging/out/metadata.json missing after restore" >&2
  exit 1
fi
