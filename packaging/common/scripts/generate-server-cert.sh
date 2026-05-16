#!/bin/sh
# Generate VideoDedup server TLS material (self-signed, openssl).
# Usage: generate-server-cert.sh INSTALL_ROOT [CERT_DIR]
#   INSTALL_ROOT — e.g. /usr/lib/videodedupserver (used for ownership when videodedup user exists)
#   CERT_DIR     — optional; default INSTALL_ROOT/cert
# Writes VideoDedup.pfx, VideoDedup.crt, VideoDedup.crt.thumbprint.txt
# Prints the PFX password on stdout (one line) when a new cert is created.
# Idempotent: exits 0 without output if VideoDedup.pfx already exists.
set -eu

INSTALL_ROOT="${1:?INSTALL_ROOT required}"
CERT_DIR="${2:-${INSTALL_ROOT}/cert}"
PFX="${CERT_DIR}/VideoDedup.pfx"
CRT="${CERT_DIR}/VideoDedup.crt"
THUMB="${CRT}.thumbprint.txt"

if [ -f "${PFX}" ]; then
  if [ -f "${CERT_DIR}/.pfx-password" ]; then
    cat "${CERT_DIR}/.pfx-password"
  fi
  exit 0
fi

if ! command -v openssl >/dev/null 2>&1; then
  echo "generate-server-cert.sh: openssl not found" >&2
  exit 1
fi

mkdir -p "${CERT_DIR}"
TMP="$(mktemp -d)"
trap 'rm -rf "${TMP}"' EXIT

PASS="$(openssl rand -base64 32 | tr -d '\n')"
KEY="${TMP}/key.pem"
CERT_PEM="${TMP}/cert.pem"

HOST_SHORT="$(hostname 2>/dev/null || echo localhost)"
HOST_FQDN="$(hostname -f 2>/dev/null || true)"
[ -n "${HOST_FQDN}" ] || HOST_FQDN="${HOST_SHORT}"

# OpenSSL 3 (Fedora, Ubuntu 24+) rejects legacy req.cnf + x509_extensions; use req -x509 -addext.
SAN="DNS:localhost,DNS:${HOST_SHORT},IP:127.0.0.1,IP:::1"
if [ -n "${HOST_FQDN}" ] && [ "${HOST_FQDN}" != "${HOST_SHORT}" ]; then
  SAN="${SAN},DNS:${HOST_FQDN}"
fi

_ips="${TMP}/ips.txt"
: > "${_ips}"
for _ip in $(hostname -I 2>/dev/null || true); do
  case "${_ip}" in
    ''|127.*|::1*) continue ;;
  esac
  echo "${_ip}" >> "${_ips}"
done
if command -v ip >/dev/null 2>&1; then
  ip -4 -o addr show scope global 2>/dev/null | awk '{print $4}' | cut -d/ -f1 >> "${_ips}" || true
fi
while read -r _ip; do
  [ -n "${_ip}" ] || continue
  case "${_ip}" in
    127.*) continue ;;
  esac
  SAN="${SAN},IP:${_ip}"
done < "${_ips}"

if ! openssl req -x509 -newkey "rsa:2048" -nodes \
  -keyout "${KEY}" -out "${CERT_PEM}" \
  -days 3650 -sha256 \
  -subj "/CN=VideoDedupServer/O=Sebastian Becker/C=DE" \
  -addext "basicConstraints=CA:FALSE" \
  -addext "keyUsage=digitalSignature,keyEncipherment" \
  -addext "extendedKeyUsage=serverAuth" \
  -addext "subjectAltName=${SAN}" 2>/dev/null; then
  # OpenSSL 1.1.x without -addext: fall back to config-file CSR flow.
  REQ="${TMP}/req.cnf"
  CSR="${TMP}/req.csr"
  cat > "${REQ}" <<EOF
[req]
distinguished_name = req_dn
prompt = no
req_extensions = v3_req
[req_dn]
CN = VideoDedupServer
O = Sebastian Becker
C = DE
[v3_req]
basicConstraints = CA:FALSE
keyUsage = digitalSignature, keyEncipherment
extendedKeyUsage = serverAuth
subjectAltName = DNS:localhost,DNS:${HOST_SHORT},IP:127.0.0.1,IP:::1
EOF
  openssl genrsa -out "${KEY}" 2048
  openssl req -new -key "${KEY}" -out "${CSR}" -config "${REQ}"
  openssl x509 -req -in "${CSR}" -signkey "${KEY}" -out "${CERT_PEM}" \
    -days 3650 -sha256 -extensions v3_req -extfile "${REQ}"
fi

openssl pkcs12 -export -out "${PFX}" -inkey "${KEY}" -in "${CERT_PEM}" \
  -passout "pass:${PASS}" -name VideoDedupServer

cp "${CERT_PEM}" "${CRT}"

THUMBPRINT="$(openssl x509 -in "${CRT}" -noout -fingerprint -sha1 | sed 's/.*=//' | tr -d ':' | tr '[:lower:]' '[:upper:]')"
printf '%s\n' "${THUMBPRINT}" > "${THUMB}"
printf '%s\n' "${PASS}" > "${CERT_DIR}/.pfx-password"
chmod 0640 "${CERT_DIR}/.pfx-password" 2>/dev/null || true

if getent passwd videodedup >/dev/null 2>&1; then
  chown root:videodedup "${CERT_DIR}/.pfx-password" 2>/dev/null || true
  chown root:videodedup "${CERT_DIR}" 2>/dev/null || true
  chown root:videodedup "${PFX}" 2>/dev/null || true
  chmod 0750 "${CERT_DIR}" 2>/dev/null || true
  chmod 0640 "${PFX}" 2>/dev/null || true
  chmod 0644 "${CRT}" "${THUMB}" 2>/dev/null || true
else
  chmod 0750 "${CERT_DIR}" 2>/dev/null || true
  chmod 0640 "${PFX}" 2>/dev/null || true
  chmod 0644 "${CRT}" "${THUMB}" 2>/dev/null || true
fi

if [ -d /usr/local/share/ca-certificates ] && command -v update-ca-certificates >/dev/null 2>&1; then
  cp "${CRT}" /usr/local/share/ca-certificates/videodedupserver.crt 2>/dev/null || true
  update-ca-certificates >/dev/null 2>&1 || true
fi

printf '%s\n' "${PASS}"
