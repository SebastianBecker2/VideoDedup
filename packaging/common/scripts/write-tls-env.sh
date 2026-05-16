#!/bin/sh
# Write /etc/videodedupserver/tls.env for systemd (Kestrel certificate path + password).
# Usage: write-tls-env.sh PFX_PATH PASSWORD [TLS_ENV_FILE]
set -eu

PFX_PATH="${1:?PFX_PATH required}"
PASSWORD="${2:?PASSWORD required}"
TLS_ENV="${3:-/etc/videodedupserver/tls.env}"

mkdir -p "$(dirname "${TLS_ENV}")"
umask 077
_esc_pw="$(printf '%s' "${PASSWORD}" | sed 's/\\/\\\\/g; s/"/\\"/g')"
cat > "${TLS_ENV}" <<EOF
Kestrel__Endpoints__gRPC__Certificate__Path=${PFX_PATH}
Kestrel__Endpoints__gRPC__Certificate__Password="${_esc_pw}"
EOF
chmod 0640 "${TLS_ENV}" 2>/dev/null || chmod 0600 "${TLS_ENV}"
if getent passwd videodedup >/dev/null 2>&1; then
  chown root:videodedup "${TLS_ENV}" 2>/dev/null || true
fi
