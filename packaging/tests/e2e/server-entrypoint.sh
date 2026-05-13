#!/usr/bin/env bash
# Used by docker-grpc-firewall.sh. Installs videodedupserver (deb/rpm/staged/pacman/snap/flatpak), applies a strict
# firewall (nft | iptables | ufw | firewalld), runs VideoDedupService.
#
# Env:
#   VD_PACKAGE_FORMAT   deb | rpm | staged | pacman | snap | flatpak (default deb)
#   VD_FIREWALL         nft | iptables | ufw | firewalld (default nft)
#   VD_INSTALL_TOOL     auto | dnf | zypper — for rpm only (default auto: use zypper if /usr/bin/zypper exists)
#
# Mounts:
#   deb:   /tmp/videodedupserver.deb
#   rpm:   /tmp/videodedupserver.rpm  (+ optional /opt/videodedup-staged for zypper fallback)
#   staged: /opt/videodedup-staged (read-only tree from packaging/.stage/<arch>/server)
#   pacman: /tmp/videodedupserver.pkg.tar.zst
#   snap:   /tmp/videodedupserver.snap (unsquashfs payload; Docker-friendly)
#   flatpak: /tmp/videodedupserver.flatpak
set -eu

FMT="${VD_PACKAGE_FORMAT:-deb}"
FW="${VD_FIREWALL:-nft}"
INSTALL_TOOL="${VD_INSTALL_TOOL:-auto}"

case "${FMT}" in deb|rpm|staged|pacman|snap|flatpak) ;; *)
  echo "VD_PACKAGE_FORMAT must be deb, rpm, staged, pacman, snap, or flatpak, got ${FMT}" >&2
  exit 1
;; esac

case "${FW}" in nft|iptables|ufw|firewalld) ;; *)
  echo "VD_FIREWALL must be nft, iptables, ufw, or firewalld, got ${FW}" >&2
  exit 1
;; esac

firewall_pkgs_apt() {
  case "${FW}" in
    nft) printf '%s' "nftables" ;;
    iptables) printf '%s' "iptables" ;;
    ufw) printf '%s' "ufw" ;;
    firewalld) printf '%s' "firewalld dbus" ;;
  esac
}

firewall_pkgs_dnf() {
  case "${FW}" in
    nft) printf '%s' "nftables" ;;
    iptables) printf '%s' "iptables" ;;
    ufw) printf '%s' "" ;;
    firewalld) printf '%s' "firewalld dbus-daemon" ;;
  esac
}

firewall_pkgs_zypper() {
  case "${FW}" in
    nft) printf '%s' "nftables" ;;
    iptables) printf '%s' "iptables" ;;
    ufw) printf '%s' "" ;;
    firewalld) printf '%s' "firewalld dbus-1" ;;
  esac
}

firewall_pkgs_pacman() {
  case "${FW}" in
    nft) printf '%s' "nftables" ;;
    iptables) printf '%s' "iptables" ;;
    ufw) printf '%s' "ufw" ;;
    firewalld) printf '%s' "firewalld dbus" ;;
  esac
}

ensure_videodedup_user() {
  if ! getent passwd videodedup >/dev/null 2>&1; then
    if command -v useradd >/dev/null 2>&1; then
      useradd --system --user-group \
        --home-dir /var/lib/videodedupserver --create-home \
        --shell /sbin/nologin videodedup 2>/dev/null \
        || useradd --system --user-group \
          --home-dir /var/lib/videodedupserver --shell /usr/sbin/nologin videodedup
    elif command -v adduser >/dev/null 2>&1; then
      adduser --system --group --home /var/lib/videodedupserver --no-create-home \
        --shell /usr/sbin/nologin videodedup
    else
      echo "cannot create videodedup user" >&2
      exit 1
    fi
  fi
  install -d -o videodedup -g videodedup -m 0750 /var/lib/videodedupserver
  install -d -o videodedup -g videodedup -m 0750 /var/log/videodedupserver
}

# Drop privileges for videodedup (LinuxHostBootstrap rejects root). Fedora :40 often ships without
# runuser/su until util-linux is installed; try absolute paths and setpriv(1) as well.
vd_exec_as_videodedup() {
  if [[ -x /usr/bin/runuser ]]; then
    /usr/bin/runuser -u videodedup -- "$@"
    return
  fi
  if command -v runuser >/dev/null 2>&1; then
    runuser -u videodedup -- "$@"
    return
  fi
  if [[ -x /usr/bin/su ]]; then
    /usr/bin/su videodedup -c "exec $(printf '%q ' "$@")"
    return
  fi
  if command -v su >/dev/null 2>&1; then
    su videodedup -c "exec $(printf '%q ' "$@")"
    return
  fi
  if command -v setpriv >/dev/null 2>&1; then
    setpriv --reuid="$(id -u videodedup)" --regid="$(id -g videodedup)" --clear-groups -- "$@"
    return
  fi
  echo "vd_exec_as_videodedup: need runuser, su, or setpriv (e.g. dnf install util-linux)" >&2
  exit 1
}

install_tree_from_staged() {
  local src="${1:-/opt/videodedup-staged}"
  [[ -f "${src}/VideoDedupService" ]] || {
    echo "staged tree missing VideoDedupService at ${src}" >&2
    exit 1
  }
  ensure_videodedup_user
  mkdir -p /usr/lib/videodedupserver
  cp -a "${src}/." /usr/lib/videodedupserver/
  chmod 0755 /usr/lib/videodedupserver/VideoDedupService
}

install_deb() {
  export DEBIAN_FRONTEND=noninteractive
  local fw_pkg
  fw_pkg="$(firewall_pkgs_apt)"

  # packaging/docker/Dockerfile.grpc-smoke-base or Dockerfile.firewall-smoke-debian-bookworm-slim:
  # deps match .deb; skip apt-get update + bulk install.
  if [[ -f /etc/videodedup-grpc-smoke-base ]] || [[ -f /etc/videodedup-firewall-smoke-deb-base ]]; then
    if dpkg -i /tmp/videodedupserver.deb; then
      return 0
    fi
    echo "videodedup-grpc-smoke-base: dpkg -i failed; falling back to apt-get (slow path)" >&2
  fi

  apt-get update -qq
  if [[ -n "${fw_pkg}" ]]; then
    # shellcheck disable=SC2086
    apt-get install -y -qq iproute2 ${fw_pkg} /tmp/videodedupserver.deb >/dev/null
  else
    apt-get install -y -qq iproute2 /tmp/videodedupserver.deb >/dev/null
  fi
}

ensure_dnf_el_ffmpeg() {
  [[ -f /etc/os-release ]] || return 0
  # shellcheck disable=SC1091
  . /etc/os-release
  case "${ID:-}" in
    rocky|almalinux|rhel|centos) ;;
    *) return 0 ;;
  esac
  dnf -y -q install dnf-plugins-core >/dev/null 2>&1 || true
  dnf config-manager --set-enabled crb 2>/dev/null \
    || dnf config-manager --set-enabled powertools 2>/dev/null \
    || true
  dnf -y -q install epel-release >/dev/null 2>&1 || true
  dnf -y -q install ffmpeg-free >/dev/null 2>&1 \
    || dnf -y -q install ffmpeg >/dev/null 2>&1 \
    || true
}

install_rpm_dnf() {
  ensure_dnf_el_ffmpeg
  if [[ -f /etc/videodedup-firewall-smoke-fedora-base ]]; then
    dnf -y -q install --setopt=tsflags=nodocs /tmp/videodedupserver.rpm
    return
  fi
  local fw_pkg
  fw_pkg="$(firewall_pkgs_dnf)"
  if [[ -n "${fw_pkg}" ]]; then
    # shellcheck disable=SC2086
    dnf -y -q install iproute util-linux ${fw_pkg} >/dev/null
  else
    dnf -y -q install iproute util-linux >/dev/null
  fi
  dnf -y -q install --setopt=tsflags=nodocs /tmp/videodedupserver.rpm
}

install_rpm_zypper() {
  local fw_pkg
  fw_pkg="$(firewall_pkgs_zypper)"
  zypper --non-interactive refresh >/dev/null
  if [[ -n "${fw_pkg}" ]]; then
    # shellcheck disable=SC2086
    zypper --non-interactive install -y iproute2 ffmpeg fontconfig ${fw_pkg} >/dev/null \
      || zypper --non-interactive install -y iproute ffmpeg fontconfig ${fw_pkg} >/dev/null
  else
    zypper --non-interactive install -y iproute2 ffmpeg fontconfig >/dev/null \
      || zypper --non-interactive install -y iproute ffmpeg fontconfig >/dev/null
  fi
  if zypper --non-interactive install -y --allow-unsigned-rpm /tmp/videodedupserver.rpm >/dev/null; then
    return 0
  fi
  if [[ -f /opt/videodedup-staged/VideoDedupService ]]; then
    echo "zypper: RPM install failed; using staged tree from /opt/videodedup-staged" >&2
    install_tree_from_staged /opt/videodedup-staged
    return 0
  fi
  echo "zypper: could not install RPM and no staged fallback" >&2
  return 1
}

install_rpm() {
  local tool="${INSTALL_TOOL}"
  if [[ "${tool}" == auto ]]; then
    tool=dnf
    if [[ -f /etc/os-release ]] && grep -qiE 'suse|opensuse' /etc/os-release; then
      tool=zypper
    fi
  fi
  case "${tool}" in
    zypper) install_rpm_zypper ;;
    dnf) install_rpm_dnf ;;
    *) echo "VD_INSTALL_TOOL must be auto, dnf, or zypper" >&2; exit 1 ;;
  esac
}

install_pacman_pkg() {
  local fw_pkg
  fw_pkg="$(firewall_pkgs_pacman)"
  if [[ -f /etc/os-release ]]; then
    if grep -qi manjaro /etc/os-release; then
      pacman-key --init 2>/dev/null || true
      pacman-key --populate manjaro archlinux 2>/dev/null || true
    elif grep -qi '^ID=arch' /etc/os-release || grep -qi archlinux /etc/os-release; then
      pacman-key --init 2>/dev/null || true
      pacman-key --populate archlinux 2>/dev/null || true
    fi
  fi
  pacman -Sy --noconfirm --needed manjaro-keyring 2>/dev/null || true
  pacman -Sy --noconfirm --needed archlinux-keyring 2>/dev/null || true
  if [[ -n "${fw_pkg}" ]]; then
    # shellcheck disable=SC2086
    pacman -Sy --noconfirm --needed iproute2 ${fw_pkg} >/dev/null
  else
    pacman -Sy --noconfirm --needed iproute2 >/dev/null
  fi
  pacman -U --noconfirm /tmp/videodedupserver.pkg.tar.zst
}

install_snap_unsquash() {
  export DEBIAN_FRONTEND=noninteractive
  local fw_pkg
  fw_pkg="$(firewall_pkgs_apt)"
  if [[ ! -f /etc/videodedup-firewall-smoke-deb-base ]]; then
    apt-get update -qq
    # shellcheck disable=SC2086
    apt-get install -y -qq iproute2 squashfs-tools ${fw_pkg} >/dev/null
  fi
  rm -rf /tmp/vd-snap
  unsquashfs -f -d /tmp/vd-snap /tmp/videodedupserver.snap
  [[ -x /tmp/vd-snap/usr/lib/videodedupserver/VideoDedupService ]] || {
    echo "snap squashfs missing usr/lib/videodedupserver/VideoDedupService" >&2
    ls -la /tmp/vd-snap >&2 || true
    exit 1
  }
  ensure_videodedup_user
}

install_flatpak_bundle() {
  ensure_dnf_el_ffmpeg
  if [[ -f /etc/videodedup-firewall-smoke-fedora-base ]]; then
    flatpak install -y --noninteractive --bundle /tmp/videodedupserver.flatpak
    ensure_videodedup_user
    return
  fi
  local fw_pkg
  fw_pkg="$(firewall_pkgs_dnf)"
  if [[ -n "${fw_pkg}" ]]; then
    # shellcheck disable=SC2086
    dnf -y -q install iproute flatpak util-linux ${fw_pkg} >/dev/null
  else
    dnf -y -q install iproute flatpak util-linux >/dev/null
  fi
  flatpak remote-add --if-not-exists flathub https://flathub.org/repo/flathub.flatpakrepo 2>/dev/null || true
  flatpak install -y --noninteractive flathub org.freedesktop.Platform//24.08
  flatpak install -y --noninteractive --bundle /tmp/videodedupserver.flatpak
  ensure_videodedup_user
}

install_staged_pacman() {
  local fw_pkg
  fw_pkg="$(firewall_pkgs_pacman)"
  if [[ -f /etc/os-release ]]; then
    if grep -qi manjaro /etc/os-release; then
      pacman-key --init 2>/dev/null || true
      pacman-key --populate manjaro archlinux 2>/dev/null || true
    elif grep -qi '^ID=arch' /etc/os-release || grep -qi archlinux /etc/os-release; then
      pacman-key --init 2>/dev/null || true
      pacman-key --populate archlinux 2>/dev/null || true
    fi
  fi
  # Arch/Manjaro: refresh keys and install deps (self-contained binary still needs ffmpeg, fontconfig, etc.)
  pacman -Sy --noconfirm --needed manjaro-keyring 2>/dev/null || true
  pacman -Sy --noconfirm --needed archlinux-keyring 2>/dev/null || true
  if [[ -n "${fw_pkg}" ]]; then
    # shellcheck disable=SC2086
    pacman -Sy --noconfirm --needed iproute2 ffmpeg fontconfig util-linux ${fw_pkg} >/dev/null
  else
    pacman -Sy --noconfirm --needed iproute2 ffmpeg fontconfig util-linux >/dev/null
  fi
  install_tree_from_staged /opt/videodedup-staged
}

apply_firewall_nft() {
  nft flush ruleset
  nft add table inet filter
  nft add chain inet filter input '{ type filter hook input priority filter; policy drop; }'
  nft add rule inet filter input ct state established,related accept
  # IPv6 neighbor discovery (ICMPv6) must be permitted or peers cannot resolve the link layer (Host unreachable).
  nft add rule inet filter input meta l4proto icmpv6 icmpv6 type { nd-neighbor-solicit, nd-neighbor-advert, nd-router-advert } accept
  nft add rule inet filter input tcp dport 51726 accept
}

apply_firewall_iptables() {
  iptables -F
  iptables -X
  iptables -t nat -F 2>/dev/null || true
  iptables -P FORWARD DROP
  iptables -P OUTPUT ACCEPT
  iptables -P INPUT DROP
  iptables -A INPUT -m state --state ESTABLISHED,RELATED -j ACCEPT
  iptables -A INPUT -p tcp --dport 51726 -j ACCEPT
  # Docker / trimmed kernels often have no ip6tables filter module (e.g. Rocky on GHA). Keep strict IPv4; skip IPv6 rules.
  if ! command -v ip6tables >/dev/null 2>&1; then
    echo "ip6tables not found; using IPv4-only iptables rules in this environment" >&2
    return 0
  fi
  if ! ip6tables -t filter -L >/dev/null 2>&1; then
    echo "ip6tables filter table unavailable; using IPv4-only iptables rules (IPv6 not firewalled here)" >&2
    return 0
  fi
  ip6tables -F
  ip6tables -X 2>/dev/null || true
  ip6tables -P FORWARD DROP
  ip6tables -P OUTPUT ACCEPT
  ip6tables -P INPUT DROP
  ip6tables -A INPUT -m state --state ESTABLISHED,RELATED -j ACCEPT
  ip6tables -A INPUT -p ipv6-icmp --icmpv6-type neighbor-solicitation -j ACCEPT
  ip6tables -A INPUT -p ipv6-icmp --icmpv6-type neighbor-advertisement -j ACCEPT
  ip6tables -A INPUT -p ipv6-icmp --icmpv6-type router-advertisement -j ACCEPT
  ip6tables -A INPUT -p tcp --dport 51726 -j ACCEPT
}

apply_firewall_ufw() {
  ufw --force reset
  ufw default deny incoming
  ufw default allow outgoing
  ufw allow 51726/tcp
  ufw --force enable
}

apply_firewall_firewalld() {
  # Docker / trimmed kernels often reject nft fib (rpfilter) rules; firewalld then exits before firewall-cmd works.
  if [[ -f /etc/firewalld/firewalld.conf ]] && grep -q '^IPv6_rpfilter=' /etc/firewalld/firewalld.conf; then
    sed -i 's/^IPv6_rpfilter=.*/IPv6_rpfilter=no/' /etc/firewalld/firewalld.conf
  fi
  mkdir -p /run/dbus
  ln -sf /run/dbus /var/run/dbus 2>/dev/null || true
  dbus-uuidgen --ensure 2>/dev/null || true
  if command -v dbus-daemon >/dev/null 2>&1; then
    dbus-daemon --system --fork
  else
    echo "dbus-daemon not found; install dbus (firewalld needs a system bus)" >&2
    exit 1
  fi
  local _dbus_wait=0
  for _ in $(seq 1 40); do
    [[ -S /run/dbus/system_bus_socket ]] && break
    sleep 0.2
    _dbus_wait=$((_dbus_wait + 1))
  done
  if [[ ! -S /run/dbus/system_bus_socket ]]; then
    echo "system D-Bus socket did not appear" >&2
    exit 1
  fi
  export DBUS_SYSTEM_BUS_ADDRESS="unix:path=/run/dbus/system_bus_socket"
  /usr/sbin/firewalld --nofork &
  local _fpid=$!
  for _ in $(seq 1 45); do
    firewall-cmd --state >/dev/null 2>&1 && break
    sleep 1
  done
  firewall-cmd --state >/dev/null 2>&1 || {
    echo "firewalld did not become ready (firewall-cmd --state failed)" >&2
    exit 1
  }
  firewall-cmd --set-default-zone=drop
  firewall-cmd --permanent --add-port=51726/tcp
  firewall-cmd --reload
  disown "${_fpid}" 2>/dev/null || true
}

apply_firewall() {
  case "${FW}" in
    nft) apply_firewall_nft ;;
    iptables) apply_firewall_iptables ;;
    ufw) apply_firewall_ufw ;;
    firewalld) apply_firewall_firewalld ;;
  esac
}

case "${FMT}" in
  deb) install_deb ;;
  rpm) install_rpm ;;
  staged) install_staged_pacman ;;
  pacman) install_pacman_pkg ;;
  snap) install_snap_unsquash ;;
  flatpak) install_flatpak_bundle ;;
esac

apply_firewall

# Kestrel in this package is configured to listen on `[::]:51726`.
# If the kernel has `net.ipv6.bindv6only=1`, the IPv6 socket will not accept
# IPv4 connections (even though E2E expects IPv4 smoke to work).
# In a privileged test container we can force dual-stack semantics.
if command -v sysctl >/dev/null 2>&1; then
  sysctl -w net.ipv6.bindv6only=0 >/dev/null 2>&1 || true
fi

install -d -o videodedup -g videodedup /var/lib/videodedupserver

if [[ "${FMT}" == flatpak ]]; then
  install -d -m 0755 /var/lib/videodedupserver
  _u="$(id -u videodedup)"
  install -d -m 0700 -o videodedup -g videodedup "/run/user/${_u}"
  # Packaged binary refuses UID 0 (LinuxHostBootstrap); flatpak must not run as root.
  # Flatpak layout can miss Kestrel gRPC appsettings (defaults to localhost:5000); force h2c gRPC port.
  vd_exec_as_videodedup env -u ASPNETCORE_URLS \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1 \
    VIDEODEDUP_APP_DATA=/var/lib/videodedupserver \
    XDG_RUNTIME_DIR="/run/user/${_u}" \
    Kestrel__Endpoints__gRPC__Url='http://[::]:51726' \
    Kestrel__Endpoints__gRPC__Protocols=Http2 \
    flatpak run io.github.sebastianbecker2.videodedup.server &
else
  _vd_bin=/usr/lib/videodedupserver/VideoDedupService
  if [[ "${FMT}" == snap ]]; then
    _vd_bin=/tmp/vd-snap/usr/lib/videodedupserver/VideoDedupService
  fi
  vd_exec_as_videodedup env \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1 \
    VIDEODEDUP_APP_DATA=/var/lib/videodedupserver \
    "${_vd_bin}" &
fi

for _ in $(seq 1 90); do
  if ss -ltn | grep -qE ':51726\b'; then
    touch /tmp/vd-ready
    break
  fi
  sleep 1
done
if [[ ! -f /tmp/vd-ready ]]; then
  echo "server did not listen on 51726" >&2
  ss -ltnp >&2 || true
  exit 1
fi

tail -f /dev/null
