#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
ARCH="amd64"

# shellcheck source=../common/packaging-python.sh disable=SC1091
source "${ROOT}/packaging/common/packaging-python.sh"
resolve_packaging_python || exit 1

while [[ $# -gt 0 ]]; do
  case "$1" in
    --arch) ARCH="$2"; shift 2 ;;
    *) echo "Unknown arg: $1" >&2; exit 1 ;;
  esac
done

META="${ROOT}/packaging/out/metadata.json"
STAGE="${ROOT}/packaging/.stage/${ARCH}/server"
DEB_WORK="${ROOT}/packaging/out/deb-work/${ARCH}"
WORK="${DEB_WORK}"
OUT="${ROOT}/packaging/out/${ARCH}/deb"

if [[ ! -f "${META}" ]]; then
  "${ROOT}/packaging/common/generate-metadata.sh"
fi

if [[ ! -d "${STAGE}" ]]; then
  echo "Missing staged payload ${STAGE}; run packaging/tools/stage.sh first" >&2
  exit 1
fi

VD_META_JSON="${META}"
if command -v cygpath >/dev/null 2>&1; then
  VD_META_JSON="$(cygpath -w "${META}")"
fi
export VD_META_JSON

PKG="$("${PACKAGING_PYTHON[@]}" -c "import json, os; print(json.load(open(os.environ['VD_META_JSON'], encoding='utf-8'))['package_name'])")"
VER_RAW="$("${PACKAGING_PYTHON[@]}" -c "import json, os; print(json.load(open(os.environ['VD_META_JSON'], encoding='utf-8'))['version'])")"
# Debian version: allow ~ for prerelease-ish segments
VER_DEB="${VER_RAW//+/\~}"

rm -rf "${WORK}"
mkdir -p "${WORK}/DEBIAN" \
  "${WORK}/usr/lib/videodedupserver" \
  "${WORK}/usr/lib/systemd/system" \
  "${WORK}/etc/videodedupserver" \
  "${WORK}/var/lib/videodedupserver" \
  "${WORK}/var/log/videodedupserver"

cp -a "${STAGE}/." "${WORK}/usr/lib/videodedupserver/"
# Lintian: bundled SQLite .so from NuGet should not ship unstripped debug payloads.
if command -v strip >/dev/null 2>&1; then
  _sqlite="${WORK}/usr/lib/videodedupserver/libe_sqlite3.so"
  if [[ -f "${_sqlite}" ]]; then
    strip --strip-unneeded "${_sqlite}" 2>/dev/null || true
  fi
fi
install -m 0644 "${ROOT}/packaging/common/systemd/videodedupserver.service" \
  "${WORK}/usr/lib/systemd/system/videodedupserver.service"
install -m 0644 "${ROOT}/packaging/common/env/videodedupserver.env" \
  "${WORK}/etc/videodedupserver/env"

FW="${ROOT}/packaging/common/firewall"
mkdir -p "${WORK}/usr/share/doc/${PKG}" "${WORK}/usr/lib/${PKG}/firewall"
# Strip CR so scripts run on Linux even if checked out with CRLF on Windows.
sed 's/\r$//' "${FW}/README.firewall" > "${WORK}/usr/share/doc/${PKG}/README.firewall"
chmod 0644 "${WORK}/usr/share/doc/${PKG}/README.firewall"
for s in open-port-ufw.sh open-port-firewalld.sh open-port-iptables.sh open-port-nftables.sh \
  configure-firewall-interactive.sh; do
  sed 's/\r$//' "${FW}/${s}" > "${WORK}/usr/lib/${PKG}/firewall/${s}"
  chmod 0755 "${WORK}/usr/lib/${PKG}/firewall/${s}"
done

CERT_SCRIPTS="${ROOT}/packaging/common/scripts"
mkdir -p "${WORK}/usr/lib/${PKG}/cert-setup"
for s in generate-server-cert.sh remove-server-cert.sh write-tls-env.sh; do
  sed 's/\r$//' "${CERT_SCRIPTS}/${s}" > "${WORK}/usr/lib/${PKG}/cert-setup/${s}"
  chmod 0755 "${WORK}/usr/lib/${PKG}/cert-setup/${s}"
done

mkdir -p "${WORK}/usr/share/lintian/overrides"
install -m 0644 "${ROOT}/packaging/common/debian/copyright" "${WORK}/usr/share/doc/${PKG}/copyright"
install -m 0644 "${ROOT}/packaging/common/debian/lintian-overrides.videodedupserver" \
  "${WORK}/usr/share/lintian/overrides/${PKG}"

SOURCE_DATE_EPOCH="${SOURCE_DATE_EPOCH:-$(git -C "${ROOT}" log -1 --format=%ct 2>/dev/null || echo 0)}"
export SOURCE_DATE_EPOCH

export WORK VD_META_JSON VER_DEB ARCH PKG
"${PACKAGING_PYTHON[@]}" <<'PY'
import gzip
import json, os, pathlib, subprocess, textwrap

work = pathlib.Path(os.environ["WORK"])
meta = json.loads(pathlib.Path(os.environ["VD_META_JSON"]).read_text(encoding="utf-8"))
pkg = os.environ["PKG"]
ver = os.environ["VER_DEB"]
arch = os.environ["ARCH"]

syn = (meta.get("description_synopsis") or meta["description"]).strip()
syn = syn.replace("\n", " ")
if len(syn) > 90:
    syn = syn[:87] + "..."

detail = (meta.get("description_detail") or meta.get("description") or "").strip()
maint = " ".join(str(meta.get("maintainer", "")).split())
if not maint or maint.count("<") != 1 or maint.count(">") != 1:
    maint = "Sebastian Becker <mail@sbecker.de.com>"
home = meta["homepage"].strip()

desc_lines = ["Description: " + syn]
extended_nonempty = False
for para in detail.split("\n\n"):
    para = para.strip().replace("\n", " ")
    if not para:
        continue
    extended_nonempty = True
    for line in textwrap.wrap(
        para,
        width=76,
        break_long_words=False,
        break_on_hyphens=False,
    ):
        desc_lines.append(" " + line)
if not extended_nonempty:
    body = (meta.get("description") or syn).strip()
    for line in textwrap.wrap(
        body,
        width=76,
        break_long_words=False,
        break_on_hyphens=False,
    ):
        desc_lines.append(" " + line)

control = "\n".join(
    [
        f"Package: {pkg}",
        f"Version: {ver}-1",
        "Section: video",
        "Priority: optional",
        f"Architecture: {arch}",
        f"Maintainer: {maint}",
        "Depends: ffmpeg, adduser, systemd, ca-certificates, openssl, libc6 (>= 2.31), libssl3 | libssl3t64 | libssl1.1",
        f"Homepage: {home}",
        *desc_lines,
        "",
    ]
)
(work / "DEBIAN" / "control").write_text(control, encoding="utf-8")

stamp = subprocess.check_output(["date", "-R"], text=True).strip()
cl = (
    f"{pkg} ({ver}-1) unstable; urgency=medium\n\n"
    f"  * Package build from upstream {meta.get('git_tag', meta.get('version', '?'))}.\n\n"
    f" -- {maint}  {stamp}\n"
)
gz_path = work / "usr" / "share" / "doc" / pkg / "changelog.Debian.gz"
# Use SOURCE_DATE_EPOCH for gzip mtime (mtime=0 upsets some lintian versions).
_mtime = int(os.environ.get("SOURCE_DATE_EPOCH", "0") or "0")
with open(gz_path, "wb") as raw:
    with gzip.GzipFile(fileobj=raw, mode="wb", compresslevel=9, mtime=_mtime) as zf:
        zf.write(cl.encode("utf-8"))
PY

cat > "${WORK}/DEBIAN/conffiles" <<'EOF'
/etc/videodedupserver/env
EOF

cat > "${WORK}/DEBIAN/postinst" <<'EOF'
#!/bin/sh
set -e
mkdir -p /var/lib/videodedupserver /var/log/videodedupserver
if ! getent passwd videodedup >/dev/null 2>&1; then
  if command -v adduser >/dev/null 2>&1; then
    adduser --system --group --home /var/lib/videodedupserver --no-create-home --shell /usr/sbin/nologin videodedup
  else
    useradd --system --user-group --home-dir /var/lib/videodedupserver --shell /usr/sbin/nologin videodedup
  fi
fi
chown -R videodedup:videodedup /var/lib/videodedupserver /var/log/videodedupserver
chmod 0750 /var/lib/videodedupserver /var/log/videodedupserver

INSTALL_ROOT="/usr/lib/videodedupserver"
CERT_SETUP="${INSTALL_ROOT}/cert-setup"
PFX_PATH="${INSTALL_ROOT}/cert/VideoDedup.pfx"
if [ ! -x "${CERT_SETUP}/generate-server-cert.sh" ]; then
  echo "videodedupserver: missing ${CERT_SETUP}/generate-server-cert.sh (rebuild package)" >&2
  exit 1
fi
if ! command -v openssl >/dev/null 2>&1; then
  echo "videodedupserver: openssl not found; cannot create TLS certificate" >&2
  exit 1
fi
PASS="$("${CERT_SETUP}/generate-server-cert.sh" "${INSTALL_ROOT}")" || {
  echo "videodedupserver: generate-server-cert.sh failed" >&2
  exit 1
}
if [ -n "${PASS:-}" ] && [ -f "${PFX_PATH}" ] && [ -x "${CERT_SETUP}/write-tls-env.sh" ]; then
  "${CERT_SETUP}/write-tls-env.sh" "${PFX_PATH}" "${PASS}" /etc/videodedupserver/tls.env
else
  echo "videodedupserver: TLS certificate was not created under ${INSTALL_ROOT}/cert" >&2
  exit 1
fi

if command -v deb-systemd-helper >/dev/null 2>&1; then
  deb-systemd-helper update-state videodedupserver.service || true
fi
systemctl daemon-reload || true
systemctl enable videodedupserver.service || true
systemctl start videodedupserver.service || true

if [ "${1:-}" = "configure" ]; then
  cat <<'FWMSG' >&2

====================================================================
videodedupserver — open firewall for gRPC (TCP 51726)
====================================================================
This package does not add host firewall rules. Remote clients need port
51726/tcp allowed (HTTPS gRPC). Server certificate for clients:
  /usr/lib/videodedupserver/cert/VideoDedup.crt

Interactive (detects ufw / firewalld / nftables / iptables, suggests one):
  sudo /usr/lib/videodedupserver/firewall/configure-firewall-interactive.sh

Or run a specific helper as root, e.g.:
  sudo /usr/lib/videodedupserver/firewall/open-port-ufw.sh
  sudo /usr/lib/videodedupserver/firewall/open-port-nftables.sh

Save nftables across reboots (optional; after the rule exists):
  sudo /usr/lib/videodedupserver/firewall/open-port-nftables.sh --persist
  (sudo drops env vars — use --persist, not "sudo PERSIST=1 ...".)

Full documentation:
  /usr/share/doc/videodedupserver/README.firewall
====================================================================

FWMSG
fi
EOF

cat > "${WORK}/DEBIAN/prerm" <<'EOF'
#!/bin/sh
set -e
case "$1" in
  remove|upgrade|deconfigure)
    systemctl stop videodedupserver.service || true
    systemctl disable videodedupserver.service || true
    ;;
esac
EOF

cat > "${WORK}/DEBIAN/postrm" <<'EOF'
#!/bin/sh
set -e
case "$1" in
  remove|purge)
    INSTALL_ROOT="/usr/lib/videodedupserver"
    if [ -x "${INSTALL_ROOT}/cert-setup/remove-server-cert.sh" ]; then
      "${INSTALL_ROOT}/cert-setup/remove-server-cert.sh" "${INSTALL_ROOT}" || true
    fi
    ;;
esac
EOF

chmod 0755 "${WORK}/DEBIAN/postinst" "${WORK}/DEBIAN/prerm" "${WORK}/DEBIAN/postrm"

mkdir -p "${OUT}"

DEB_NAME="${PKG}_${VER_DEB}-1_${ARCH}.deb"

# DrvFs (WSL /mnt/..., Git Bash /d/...) often keeps 777 on dirs; chmod is ignored and dpkg-deb fails.
# Copy to a native temp tree when needed, then normalize modes.
if command -v stat >/dev/null 2>&1; then
  _deb_m="$(stat -c '%a' "${WORK}/DEBIAN" 2>/dev/null || echo 755)"
  if [[ "${_deb_m}" == "777" ]] || [[ "${_deb_m}" -gt 775 ]]; then
    _vd_native="$(mktemp -d "${TMPDIR:-/tmp}/videodedup-debwork.XXXXXX")"
    cp -a "${WORK}/." "${_vd_native}/"
    WORK="${_vd_native}"
    _vd_cleanup_debwork() { rm -rf "${_vd_native}"; }
    trap _vd_cleanup_debwork EXIT
  fi
fi
find "${WORK}" -type d -exec chmod 0755 {} +
find "${WORK}" -type f -exec chmod 0644 {} +
chmod 0755 "${WORK}/DEBIAN/postinst" "${WORK}/DEBIAN/prerm" "${WORK}/DEBIAN/postrm"
chmod 0755 "${WORK}/usr/lib/videodedupserver/VideoDedupService"
chmod 0755 "${WORK}/usr/lib/${PKG}/cert-setup"/*.sh
chmod 0755 "${WORK}/usr/lib/${PKG}/firewall"/*.sh

if command -v fakeroot >/dev/null 2>&1; then
  fakeroot dpkg-deb --root-owner-group --build "${WORK}" "${OUT}/${DEB_NAME}"
else
  dpkg-deb --root-owner-group --build "${WORK}" "${OUT}/${DEB_NAME}"
fi

echo "Built ${OUT}/${DEB_NAME}"

# Drop intermediate tree (DrvFs path may still exist even when build used a temp copy).
rm -rf "${DEB_WORK}"
rmdir "${ROOT}/packaging/out/deb-work" 2>/dev/null || true
