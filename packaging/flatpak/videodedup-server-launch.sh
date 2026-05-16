#!/bin/sh
set -eu

# Under flatpak run, /app/lib/... is the install root; E2E runs the same script from the host files tree (VD_APP_LIB).
APP_LIB="${VD_APP_LIB:-/app/lib/videodedupserver}"
BIN="${APP_LIB}/VideoDedupService"
CERT_SETUP="${APP_LIB}/cert-setup"
if [ -n "${VD_CERT_DIR:-}" ]; then
  CERT_DIR="${VD_CERT_DIR}"
else
  DATA_HOME="${XDG_DATA_HOME:-${HOME}/.var/app/io.github.sebastianbecker2.videodedup.server/data}"
  CERT_DIR="${DATA_HOME}/cert"
fi
PFX="${CERT_DIR}/VideoDedup.pfx"

mkdir -p "${CERT_DIR}"
if [ ! -f "${PFX}" ] && [ -x "${CERT_SETUP}/generate-server-cert.sh" ]; then
  PASS="$("${CERT_SETUP}/generate-server-cert.sh" "${APP_LIB}" "${CERT_DIR}")"
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
export VIDEODEDUP_APP_DATA="${VIDEODEDUP_APP_DATA:-${DATA_HOME:-${HOME}/.var/app/io.github.sebastianbecker2.videodedup.server/data}}"

exec "${BIN}" "$@"
