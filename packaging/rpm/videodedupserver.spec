# Placeholders @VERSION@, @RPMARCH@, @HOMEPAGE@, @DESCRIPTION@, @REPO@,
# @MAINTAINER@, @CHANGELOG_DATE@ are substituted by packaging/tools/build-rpm.sh

%global __strip /bin/true
%global _build_id_links none
%global _missing_build_ids_terminate_build 0

Name:           videodedupserver
Version:        @VERSION@
Release:        1%{?dist}
Summary:        VideoDedup gRPC deduplication server
License:        MIT
URL:            @HOMEPAGE@
Source0:        %{name}-%{version}.tar.gz
BuildArch:      @RPMARCH@
# Fedora base repos ship ffmpeg-free; full "ffmpeg" is often from RPM Fusion.
Requires:       (ffmpeg or ffmpeg-free)
Requires:       systemd
Requires(pre):  shadow-utils
Requires:       ca-certificates
Requires:       openssl-libs

%description
@DESCRIPTION@

%prep
%setup -q

%build
:

%install
rm -rf %{buildroot}
mkdir -p %{buildroot}/usr/lib/videodedupserver
cp -a . %{buildroot}/usr/lib/videodedupserver/
mkdir -p %{buildroot}/usr/lib/systemd/system
install -m 0644 @REPO@/packaging/common/systemd/videodedupserver.service \
  %{buildroot}/usr/lib/systemd/system/videodedupserver.service
mkdir -p %{buildroot}/etc/videodedupserver
install -m 0644 @REPO@/packaging/common/env/videodedupserver.env \
  %{buildroot}/etc/videodedupserver/env
mkdir -p %{buildroot}/usr/share/doc/%{name}
install -m 0644 @REPO@/LICENSE %{buildroot}/usr/share/doc/%{name}/LICENSE
sed 's/\r$//' @REPO@/packaging/common/firewall/README.firewall > %{buildroot}/usr/share/doc/%{name}/README.firewall
chmod 0644 %{buildroot}/usr/share/doc/%{name}/README.firewall
mkdir -p %{buildroot}/usr/lib/%{name}/firewall
sed 's/\r$//' @REPO@/packaging/common/firewall/open-port-ufw.sh > %{buildroot}/usr/lib/%{name}/firewall/open-port-ufw.sh
sed 's/\r$//' @REPO@/packaging/common/firewall/open-port-firewalld.sh > %{buildroot}/usr/lib/%{name}/firewall/open-port-firewalld.sh
sed 's/\r$//' @REPO@/packaging/common/firewall/open-port-iptables.sh > %{buildroot}/usr/lib/%{name}/firewall/open-port-iptables.sh
sed 's/\r$//' @REPO@/packaging/common/firewall/open-port-nftables.sh > %{buildroot}/usr/lib/%{name}/firewall/open-port-nftables.sh
sed 's/\r$//' @REPO@/packaging/common/firewall/configure-firewall-interactive.sh > %{buildroot}/usr/lib/%{name}/firewall/configure-firewall-interactive.sh
chmod 0755 %{buildroot}/usr/lib/%{name}/firewall/open-port-ufw.sh \
  %{buildroot}/usr/lib/%{name}/firewall/open-port-firewalld.sh \
  %{buildroot}/usr/lib/%{name}/firewall/open-port-iptables.sh \
  %{buildroot}/usr/lib/%{name}/firewall/open-port-nftables.sh \
  %{buildroot}/usr/lib/%{name}/firewall/configure-firewall-interactive.sh
mkdir -p %{buildroot}/var/lib/videodedupserver
mkdir -p %{buildroot}/var/log/videodedupserver

%post
getent passwd videodedup >/dev/null || useradd --system --user-group \
  --home-dir /var/lib/videodedupserver --create-home \
  --shell /sbin/nologin videodedup || true
chown -R videodedup:videodedup /var/lib/videodedupserver /var/log/videodedupserver || true
chmod 0750 /var/lib/videodedupserver /var/log/videodedupserver || true
if [ "$1" -eq 1 ]; then
  /bin/systemctl daemon-reload || :
  /bin/systemctl enable videodedupserver.service || :
  /bin/systemctl start videodedupserver.service || :
  cat <<'FWMSG' >&2

====================================================================
videodedupserver — open firewall for gRPC (TCP 51726)
====================================================================
This package does not add host firewall rules. Allow TCP 51726 for clients.

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
