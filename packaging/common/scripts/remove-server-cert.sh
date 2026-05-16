#!/bin/sh
# Remove VideoDedup server TLS material installed by generate-server-cert.sh.
# Usage: remove-server-cert.sh INSTALL_ROOT [CERT_DIR] [TLS_ENV_FILE]
set -eu

INSTALL_ROOT="${1:?INSTALL_ROOT required}"
CERT_DIR="${2:-${INSTALL_ROOT}/cert}"
TLS_ENV="${3:-/etc/videodedupserver/tls.env}"

if [ -f /usr/local/share/ca-certificates/videodedupserver.crt ]; then
  rm -f /usr/local/share/ca-certificates/videodedupserver.crt
  if command -v update-ca-certificates >/dev/null 2>&1; then
    update-ca-certificates --fresh >/dev/null 2>&1 || true
  fi
fi

if [ -d "${CERT_DIR}" ]; then
  rm -rf "${CERT_DIR}"
fi

if [ -f "${TLS_ENV}" ]; then
  rm -f "${TLS_ENV}"
fi
