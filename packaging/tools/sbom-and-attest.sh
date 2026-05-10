#!/usr/bin/env bash
set -euo pipefail

# Optional: generate CycloneDX SBOM with syft and (if configured) cosign attest.
# Requires: syft on PATH; for attestation: cosign + COSIGN_EXPERIMENTAL=1 and key material.

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
ARCH="${1:-amd64}"
STAGE="${ROOT}/packaging/.stage/${ARCH}/server"
OUT="${ROOT}/packaging/out/${ARCH}/sbom"
mkdir -p "${OUT}"

if [[ ! -d "${STAGE}" ]]; then
  echo "Missing ${STAGE}; run stage.sh first" >&2
  exit 1
fi

if command -v syft >/dev/null 2>&1; then
  syft scan "dir:${STAGE}" -o cyclonedx-json > "${OUT}/sbom.cdx.json"
  echo "Wrote ${OUT}/sbom.cdx.json"
else
  echo "syft not installed; skipping SBOM (install from https://github.com/anchore/syft)" >&2
fi

if command -v cosign >/dev/null 2>&1 && [[ -n "${COSIGN_YES:-}" ]]; then
  for f in "${ROOT}/packaging/out/${ARCH}"/deb/*.deb "${ROOT}/packaging/out/${ARCH}"/rpm/*.rpm; do
    [[ -f "$f" ]] || continue
    cosign attest-blob --yes --type cyclonedx --predicate "${OUT}/sbom.cdx.json" "$f" || true
  done
fi
