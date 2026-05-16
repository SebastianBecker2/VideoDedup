# Placeholders @VERSION@, @RPMARCH@, @HOMEPAGE@, @REPO@, @MAINTAINER@, @CHANGELOG_DATE@
# are substituted by packaging/tools/build-rpm.sh

%global __strip /bin/true
%global _build_id_links none
%global _missing_build_ids_terminate_build 0

Name:           videodedupserver
Version:        @VERSION@
Release:        1%{?dist}
Summary:        VideoDedup gRPC deduplication server
License:        MIT
URL:            @HOMEPAGE@
Packager:       Sebastian Becker <mail@sbecker.de.com>
Group:          Applications/System
Source0:        %{name}-%{version}.tar.gz
BuildArch:      @RPMARCH@
# Fedora base repos ship ffmpeg-free; full "ffmpeg" is often from RPM Fusion.
Requires:       (ffmpeg or ffmpeg-free)
Requires:       systemd
Requires(pre):  shadow-utils
Requires:       ca-certificates
Requires:       openssl-libs
Requires:       openssl

%description
VideoDedup gRPC server: scans storage for near-duplicate videos and exposes
match results to the VideoDedup desktop client. Listens on TCP 51726 by
default (HTTPS gRPC with install-time self-signed certificate). Ships a systemd unit; firewall rules are left to
the administrator (see %{_docdir}/%{name}/README.firewall).

%prep
%setup -q

%build
:

%install
rm -rf "%{buildroot}"
mkdir -p "%{buildroot}/usr/lib/videodedupserver"
cp -a . "%{buildroot}/usr/lib/videodedupserver/"
find "%{buildroot}/usr/lib/videodedupserver" -maxdepth 1 -type f ! -name 'VideoDedupService' -exec chmod 0644 {} +
chmod 0755 "%{buildroot}/usr/lib/videodedupserver/VideoDedupService"
mkdir -p "%{buildroot}/usr/lib/systemd/system"
install -m 0644 "@REPO@/packaging/common/systemd/videodedupserver.service" \
  "%{buildroot}/usr/lib/systemd/system/videodedupserver.service"
mkdir -p "%{buildroot}/etc/videodedupserver"
install -m 0644 "@REPO@/packaging/common/env/videodedupserver.env" \
  "%{buildroot}/etc/videodedupserver/env"
mkdir -p "%{buildroot}/usr/share/doc/%{name}"
sed 's/\r$//' "@REPO@/LICENSE" > "%{buildroot}/usr/share/doc/%{name}/LICENSE"
chmod 0644 "%{buildroot}/usr/share/doc/%{name}/LICENSE"
sed 's/\r$//' "@REPO@/packaging/common/firewall/README.firewall" > "%{buildroot}/usr/share/doc/%{name}/README.firewall"
chmod 0644 "%{buildroot}/usr/share/doc/%{name}/README.firewall"
mkdir -p "%{buildroot}/usr/lib/%{name}/firewall"
sed 's/\r$//' "@REPO@/packaging/common/firewall/open-port-ufw.sh" > "%{buildroot}/usr/lib/%{name}/firewall/open-port-ufw.sh"
sed 's/\r$//' "@REPO@/packaging/common/firewall/open-port-firewalld.sh" > "%{buildroot}/usr/lib/%{name}/firewall/open-port-firewalld.sh"
sed 's/\r$//' "@REPO@/packaging/common/firewall/open-port-iptables.sh" > "%{buildroot}/usr/lib/%{name}/firewall/open-port-iptables.sh"
sed 's/\r$//' "@REPO@/packaging/common/firewall/open-port-nftables.sh" > "%{buildroot}/usr/lib/%{name}/firewall/open-port-nftables.sh"
sed 's/\r$//' "@REPO@/packaging/common/firewall/configure-firewall-interactive.sh" > "%{buildroot}/usr/lib/%{name}/firewall/configure-firewall-interactive.sh"
chmod 0755 "%{buildroot}/usr/lib/%{name}/firewall/open-port-ufw.sh" \
  "%{buildroot}/usr/lib/%{name}/firewall/open-port-firewalld.sh" \
  "%{buildroot}/usr/lib/%{name}/firewall/open-port-iptables.sh" \
  "%{buildroot}/usr/lib/%{name}/firewall/open-port-nftables.sh" \
  "%{buildroot}/usr/lib/%{name}/firewall/configure-firewall-interactive.sh"
mkdir -p "%{buildroot}/usr/lib/%{name}/cert-setup"
for s in generate-server-cert.sh remove-server-cert.sh write-tls-env.sh; do
  sed 's/\r$//' "@REPO@/packaging/common/scripts/${s}" > "%{buildroot}/usr/lib/%{name}/cert-setup/${s}"
  chmod 0755 "%{buildroot}/usr/lib/%{name}/cert-setup/${s}"
done
mkdir -p "%{buildroot}/var/lib/videodedupserver"
mkdir -p "%{buildroot}/var/log/videodedupserver"

%post
getent passwd videodedup >/dev/null || useradd --system --user-group \
  --home-dir /var/lib/videodedupserver --create-home \
  --shell /sbin/nologin videodedup || true
chown -R videodedup:videodedup /var/lib/videodedupserver /var/log/videodedupserver || true
chmod 0750 /var/lib/videodedupserver /var/log/videodedupserver || true
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
if [ "$1" -eq 1 ]; then
  /bin/systemctl daemon-reload || :
  /bin/systemctl enable videodedupserver.service || :
  /bin/systemctl start videodedupserver.service || :
  cat <<'FWMSG' >&2

====================================================================
videodedupserver — open firewall for gRPC (TCP 51726)
====================================================================
This package does not add host firewall rules. Allow TCP 51726 for HTTPS gRPC clients.
Import server certificate from: /usr/lib/videodedupserver/cert/VideoDedup.crt

  sudo /usr/lib/videodedupserver/firewall/configure-firewall-interactive.sh
  sudo /usr/lib/videodedupserver/firewall/open-port-firewalld.sh
  sudo /usr/lib/videodedupserver/firewall/open-port-nftables.sh

nftables persistence:  sudo /usr/lib/videodedupserver/firewall/open-port-nftables.sh --persist

  /usr/share/doc/videodedupserver/README.firewall
====================================================================

FWMSG
fi

%preun
if [ "$1" -eq 0 ]; then
  /bin/systemctl stop videodedupserver.service || :
  /bin/systemctl disable videodedupserver.service || :
  INSTALL_ROOT="/usr/lib/videodedupserver"
  if [ -x "${INSTALL_ROOT}/cert-setup/remove-server-cert.sh" ]; then
    "${INSTALL_ROOT}/cert-setup/remove-server-cert.sh" "${INSTALL_ROOT}" || :
  fi
fi

%postun
if [ "$1" -ge 1 ]; then
  /bin/systemctl try-restart videodedupserver.service || :
fi
/bin/systemctl daemon-reload || :

%files
%license /usr/share/doc/videodedupserver/LICENSE
/usr/share/doc/videodedupserver/README.firewall
/usr/lib/videodedupserver
/usr/lib/systemd/system/videodedupserver.service
%config(noreplace) %attr(0644,root,root) /etc/videodedupserver/env
# Owned root in the payload so RPM does not add user()/group() deps (dnf cannot satisfy those before %%post).
%dir %attr(0750,root,root) /var/lib/videodedupserver
%dir %attr(0750,root,root) /var/log/videodedupserver

%changelog
* @CHANGELOG_DATE@ @MAINTAINER@ - @VERSION@-1
- Packaged release (see git metadata)
