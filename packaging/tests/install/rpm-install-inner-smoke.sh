#!/usr/bin/env bash
# Consumed by docker-install-rpm.sh (mounted into fedora:40). Keep LF-only (see .gitattributes).
set -eu
# fedora:* images often set tsflags=nodocs, which skips /usr/share/doc/* except licenses.
# Clear tsflags so README.firewall matches a normal user install.
dnf -y -q install --setopt=tsflags= /tmp/videodedupserver.rpm >/dev/null

rpm -q videodedupserver >/dev/null

test -x /usr/lib/videodedupserver/VideoDedupService
test -f /usr/lib/videodedupserver/appsettings.json
grep -q "51726" /usr/lib/videodedupserver/appsettings.json
test -f /usr/lib/videodedupserver/firewall/open-port-nftables.sh
test -f /usr/lib/videodedupserver/firewall/configure-firewall-interactive.sh
test -f /usr/share/doc/videodedupserver/README.firewall
id videodedup >/dev/null

install -d -o videodedup -g videodedup /var/lib/videodedupserver

test -f /usr/lib/videodedupserver/cert/VideoDedup.pfx
test -f /etc/videodedupserver/tls.env

runuser -u videodedup -- bash -s <<'SMOKE'
set -eu
set -a
# shellcheck source=/dev/null
. /etc/videodedupserver/tls.env
set +a
export ASPNETCORE_ENVIRONMENT=Production
export DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1
export VIDEODEDUP_APP_DATA=/var/lib/videodedupserver
timeout 25s /usr/lib/videodedupserver/VideoDedupService 2>&1 | tee /tmp/vd-smoke.log
SMOKE

if ! grep -qE "Now listening|Application started" /tmp/vd-smoke.log; then
  echo "--- smoke log ---" >&2
  cat /tmp/vd-smoke.log >&2
  exit 1
fi

echo "rpm install smoke OK"
