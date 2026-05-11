#!/usr/bin/env bash
set -euo pipefail

# Lint and quick validation for built packages (run on Linux CI or developer machine).
# Full install/start/stop/uninstall is environment-specific; extend with Docker if needed.

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
ARCH="${1:-amd64}"
OUT_DEB="${ROOT}/packaging/out/${ARCH}/deb"
OUT_RPM="${ROOT}/packaging/out/${ARCH}/rpm"

FAILED=0

if [[ -x "${ROOT}/packaging/tests/firewall/validate-static.sh" ]]; then
  "${ROOT}/packaging/tests/firewall/validate-static.sh" || FAILED=1
fi

if compgen -G "${OUT_DEB}/*.deb" > /dev/null; then
  if command -v lintian >/dev/null 2>&1; then
    # Lint only the newest .deb: packaging/out often accumulates older artifacts and
    # lintian would otherwise fail on stale packages missing current overrides/metadata.
    latest_deb="$(ls -t "${OUT_DEB}"/*.deb | head -1)"
    lintian --fail-on error "${latest_deb}" || FAILED=1
  else
    echo "lintian not installed; skipping DEB lint"
  fi
else
  echo "No .deb in ${OUT_DEB}; run build-deb.sh first"
fi

RPMLINT_CFG="${ROOT}/packaging/rpm/videodedupserver-rpmlint.toml"
if command -v rpmlint >/dev/null 2>&1; then
  rpm_found=0
  shopt -s nullglob
  for archdir in "${OUT_RPM}"/*/; do
    [[ -d "${archdir}" ]] || continue
    rpms=( "${archdir}"*.rpm )
    ((${#rpms[@]})) || continue
    rpm_found=1
    latest_rpm="$(ls -t "${rpms[@]}" | head -1)"
    if [[ -f "${RPMLINT_CFG}" ]]; then
      rpmlint --config "${RPMLINT_CFG}" "${latest_rpm}" || FAILED=1
    else
      rpmlint "${latest_rpm}" || FAILED=1
    fi
  done
  shopt -u nullglob
  if [[ "${rpm_found}" -eq 0 ]]; then
    echo "No .rpm in ${OUT_RPM}/*/ — run build-rpm.sh first"
  fi
else
  echo "rpmlint not installed; skipping RPM lint"
fi

shopt -s nullglob
SNAPS=( "${ROOT}/packaging/out/${ARCH}/snap/"*.snap )
shopt -u nullglob

if ((${#SNAPS[@]} > 0)); then
  if command -v review-tools >/dev/null 2>&1; then
    review-tools.snap-review "${SNAPS[@]}" || FAILED=1
  else
    echo "review-tools not installed; skipping snap-review"
  fi
fi

shopt -s nullglob
PKGS=( "${ROOT}/packaging/out/${ARCH}/pacman/"*.pkg.tar.zst )
shopt -u nullglob

if ((${#PKGS[@]} > 0)); then
  if command -v namcap >/dev/null 2>&1; then
    namcap "${PKGS[@]}" || FAILED=1
  else
    echo "namcap not installed; skipping Arch package checks"
  fi
fi

exit "${FAILED}"
