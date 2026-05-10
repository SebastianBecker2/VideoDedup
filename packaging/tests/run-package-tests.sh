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
    lintian --fail-on error "${OUT_DEB}"/*.deb || FAILED=1
  else
    echo "lintian not installed; skipping DEB lint"
  fi
else
  echo "No .deb in ${OUT_DEB}; run build-deb.sh first"
fi

shopt -s nullglob
RPMS=( "${OUT_RPM}"/*/*.rpm )
SNAPS=( "${ROOT}/packaging/out/${ARCH}/snap/"*.snap )
shopt -u nullglob

if ((${#RPMS[@]} > 0)); then
  if command -v rpmlint >/dev/null 2>&1; then
    rpmlint "${RPMS[@]}" || FAILED=1
  else
    echo "rpmlint not installed; skipping RPM lint"
  fi
else
  echo "No .rpm in ${OUT_RPM}/*/ — run build-rpm.sh first"
fi

if ((${#SNAPS[@]} > 0)); then
  if command -v review-tools >/dev/null 2>&1; then
    review-tools.snap-review "${SNAPS[@]}" || FAILED=1
  else
    echo "review-tools not installed; skipping snap-review"
  fi
fi

exit "${FAILED}"
