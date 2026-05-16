#!/bin/sh
set -eu

INSTALL_ROOT="${SNAP}/usr/lib/videodedupserver"
BIN="${INSTALL_ROOT}/VideoDedupService"
CERT_SETUP="${INSTALL_ROOT}/cert-setup"
CERT_DIR="${SNAP_COMMON}/cert"
PFX="${CERT_DIR}/VideoDedup.pfx"

mkdir -p "${CERT_DIR}"
if [ ! -f "${PFX}" ] && [ -x "${CERT_SETUP}/generate-server-cert.sh" ]; then
  PASS="$("${CERT_SETUP}/generate-server-cert.sh" "${INSTALL_ROOT}" "${CERT_DIR}")"
  if [ -n "${PASS:-}" ] && [ -f "${PFX}" ]; then
    export Kestrel__Endpoints__gRPC__Certificate__Path="${PFX}"
    export Kestrel__Endpoints__gRPC__Certificate__Password="${PASS}"
  fi
elif [ -f "${PFX}" ] && [ -f "${CERT_DIR}/.pfx-password" ]; then
  export Kestrel__Endpoints__gRPC__Certificate__Path="${PFX}"
  export Kestrel__Endpoints__gRPC__Certificate__Password="$(cat "${CERT_DIR}/.pfx-password")"
fi

export Kestrel__Endpoints__gRPC__Url="${Kestrel__Endpoints__gRPC__Url:-https://[::]:51726}"
export Kestrel__Endpoints__gRPC__Protocols="${Kestrel__Endpoints__gRPC__Protocols:-Http2}"
export VIDEODEDUP_APP_DATA="${VIDEODEDUP_APP_DATA:-${SNAP_COMMON}/data}"

exec "${BIN}" "$@"
