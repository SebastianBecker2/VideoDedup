#!/usr/bin/env bash
# Used by docker_grpc_firewall.py / docker-grpc-deep-smoke. Installs videodedupserver (deb/rpm/staged/pacman/snap/flatpak), applies a strict
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

vd_run_sh() {
  local script="$1"
  shift
  if grep -q $'\r' "${script}" 2>/dev/null; then
    sed 's/\r$//' "${script}" | sh -s "$@"
  else
    sh "${script}" "$@"
  fi
}

vd_ensure_openssl() {
  command -v openssl >/dev/null 2>&1 && return 0
  echo "E2E: installing openssl for TLS certificate generation ..." >&2
  if command -v apt-get >/dev/null 2>&1; then
    export DEBIAN_FRONTEND=noninteractive
    apt-get update -qq 2>/dev/null || true
    apt-get install -y -qq openssl 2>/dev/null || true
  elif command -v dnf >/dev/null 2>&1; then
    dnf -y -q install openssl >/dev/null 2>&1 || true
  elif command -v zypper >/dev/null 2>&1; then
    zypper --non-interactive install -y openssl >/dev/null 2>&1 || true
  elif command -v pacman >/dev/null 2>&1; then
    pacman -Sy --noconfirm --needed openssl >/dev/null 2>&1 || true
  fi
  command -v openssl >/dev/null 2>&1
}

vd_setup_tls() {
  local install_root="${1:-/usr/lib/videodedupserver}"
  local cert_dir="${2:-${install_root}/cert}"
  local cert_setup="${install_root}/cert-setup"
  local pfx_path="${cert_dir}/VideoDedup.pfx"
  [[ -x "${cert_setup}/generate-server-cert.sh" ]] || {
    echo "E2E: cert-setup missing under ${install_root} (rebuild .deb after TLS packaging changes)" >&2
    return 1
  }
  vd_ensure_openssl || {
    echo "E2E: openssl required to generate VideoDedup TLS certificate" >&2
    return 1
  }
  local pass gen_rc=0
  pass="$(vd_run_sh "${cert_setup}/generate-server-cert.sh" "${install_root}" "${cert_dir}")" || gen_rc=$?
  if [[ "${gen_rc}" -ne 0 ]]; then
    echo "E2E: generate-server-cert.sh failed (exit ${gen_rc})" >&2
    return 1
  fi
  if [[ -n "${pass:-}" ]] && [[ -f "${pfx_path}" ]] && [[ -x "${cert_setup}/write-tls-env.sh" ]]; then
    vd_run_sh "${cert_setup}/write-tls-env.sh" "${pfx_path}" "${pass}" /etc/videodedupserver/tls.env
  fi
  [[ -f "${pfx_path}" ]]
}

vd_require_tls_cert() {
  local pfx_path="${1:-/usr/lib/videodedupserver/cert/VideoDedup.pfx}"
  if [[ ! -f "${pfx_path}" ]]; then
    echo "E2E: missing TLS certificate ${pfx_path}" >&2
    exit 1
  fi
}

# Kestrel TLS from /etc/videodedupserver/tls.env (runuser does not inherit root shell exports).
vd_tls_env_args() {
  VD_TLS_ENV_ARGS=()
  [[ -f /etc/videodedupserver/tls.env ]] || return 0
  local line key val
  while IFS= read -r line || [[ -n "${line}" ]]; do
    line="${line//$'\r'/}"
    case "${line}" in
      ''|\#*) continue ;;
    esac
    key="${line%%=*}"
    val="${line#*=}"
    [[ -n "${key}" ]] || continue
    # Strip optional double quotes (write-tls-env.sh).
    if [[ "${val}" == \"*\" && "${val}" == *\" ]]; then
      val="${val:1:${#val}-2}"
      val="${val//\\\"/\"}"
      val="${val//\\\\/\\}"
    fi
    VD_TLS_ENV_ARGS+=("${key}=${val}")
  done < /etc/videodedupserver/tls.env
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
    dnf -y -q install iproute util-linux procps-ng ${fw_pkg} >/dev/null
  else
    dnf -y -q install iproute util-linux procps-ng >/dev/null
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
    apt-get install -y -qq iproute2 squashfs-tools openssl ${fw_pkg} >/dev/null
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

# deb/rpm postinst runs `systemctl start videodedupserver` when systemctl succeeds (common in privileged
# test images). This entrypoint then starts VideoDedupService manually as videodedup — stop the packaged
# instance first so Kestrel can bind [::]:51726.
stop_packaged_videodedup_if_running() {
  if command -v systemctl >/dev/null 2>&1; then
    systemctl stop videodedupserver.service 2>/dev/null || true
    systemctl disable videodedupserver.service 2>/dev/null || true
  fi
  if command -v pkill >/dev/null 2>&1; then
    pkill -TERM -x VideoDedupService 2>/dev/null || true
    sleep 1
    pkill -KILL -x VideoDedupService 2>/dev/null || true
  fi
}
stop_packaged_videodedup_if_running

apply_firewall

# Kestrel in this package is configured to listen on `[::]:51726`.
# If the kernel has `net.ipv6.bindv6only=1`, the IPv6 socket will not accept
# IPv4 connections (even though E2E expects IPv4 smoke to work).
# In a privileged test container we can force dual-stack semantics.
if command -v sysctl >/dev/null 2>&1; then
  sysctl -w net.ipv6.bindv6only=0 >/dev/null 2>&1 || true
fi

install -d -o videodedup -g videodedup /var/lib/videodedupserver

# Smoke mounts comparison / dedup fixtures here. The service runs as user videodedup; the bind mount
# is often owned by root or the host checkout UID (e.g. GitHub Actions), so "other" cannot remove
# directory entries — ResolveDuplicate (delete file) fails with "access denied" even when the volume is rw.
# Grant write on directories only so unlink/create names under /tmp/vd-fixtures works without chmod'ing files.
if [[ -d /tmp/vd-fixtures ]]; then
  chmod -R a+rX /tmp/vd-fixtures 2>/dev/null || true
  find /tmp/vd-fixtures -type d -exec chmod o+w {} + 2>/dev/null || true
fi

# FFmpeg.AutoGen loads libav from this directory on Linux (FfmpegWrapper.TryConfigureLinuxNativeRootPath).
# docker_grpc_deep_smoke and docker_grpc_firewall preset it per distro; otherwise auto-detect below.
detect_ffmpeg_lib_root() {
  if [[ -n "${VIDEODEDUP_FFMPEG_LIB_ROOT:-}" ]]; then
    echo "E2E: VIDEODEDUP_FFMPEG_LIB_ROOT=${VIDEODEDUP_FFMPEG_LIB_ROOT} (preset)" >&2
    return 0
  fi
  local deb_march
  case "$(uname -m 2>/dev/null)" in
    aarch64 | arm64) deb_march=aarch64-linux-gnu ;;
    *) deb_march=x86_64-linux-gnu ;;
  esac
  if [[ -d "/usr/lib/${deb_march}" ]] && compgen -G "/usr/lib/${deb_march}/libavcodec.so*" &>/dev/null; then
    export VIDEODEDUP_FFMPEG_LIB_ROOT="/usr/lib/${deb_march}"
    echo "E2E: VIDEODEDUP_FFMPEG_LIB_ROOT=${VIDEODEDUP_FFMPEG_LIB_ROOT} (multiarch)" >&2
    return 0
  fi
  for d in /usr/lib64 /usr/lib; do
    if [[ -d "${d}" ]] && compgen -G "${d}/libavcodec.so*" &>/dev/null; then
      export VIDEODEDUP_FFMPEG_LIB_ROOT="${d}"
      echo "E2E: VIDEODEDUP_FFMPEG_LIB_ROOT=${VIDEODEDUP_FFMPEG_LIB_ROOT}" >&2
      return 0
    fi
  done
  echo "E2E: WARNING: could not find libavcodec under /usr/lib*; FFmpeg.AutoGen may fail (StartVideoComparison)" >&2
}
detect_ffmpeg_lib_root

if [[ "${FMT}" != flatpak && "${FMT}" != snap ]]; then
  vd_setup_tls /usr/lib/videodedupserver || exit 1
  vd_require_tls_cert /usr/lib/videodedupserver/cert/VideoDedup.pfx
fi

if [[ "${FMT}" == flatpak ]]; then
  install -d -m 0755 /var/lib/videodedupserver
  _u="$(id -u videodedup)"
  install -d -m 0700 -o videodedup -g videodedup "/run/user/${_u}"
  _fp_app_id="io.github.sebastianbecker2.videodedup.server"
  _fp_data="/var/lib/videodedupserver/.var/app/${_fp_app_id}/data"
  _fp_install=""
  if _fp_root="$(flatpak info --show-location "${_fp_app_id}" 2>/dev/null)" && [[ -n "${_fp_root}" ]]; then
    _fp_install="${_fp_root}/lib/videodedupserver"
  fi
  if [[ -n "${_fp_install}" && -x "${_fp_install}/cert-setup/generate-server-cert.sh" ]]; then
    install -d -o videodedup -g videodedup "${_fp_data}/cert"
    vd_ensure_openssl || {
      echo "E2E: openssl required for flatpak TLS certificate" >&2
      exit 1
    }
    vd_setup_tls "${_fp_install}" "${_fp_data}/cert" || exit 1
    vd_require_tls_cert "${_fp_data}/cert/VideoDedup.pfx"
  else
    echo "E2E: flatpak app install path missing cert-setup (rebuild flatpak bundle)" >&2
    exit 1
  fi
  # Packaged binary refuses UID 0 (LinuxHostBootstrap); flatpak must not run as root.
  vd_exec_as_videodedup env -u ASPNETCORE_URLS \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1 \
    VIDEODEDUP_APP_DATA=/var/lib/videodedupserver \
    XDG_RUNTIME_DIR="/run/user/${_u}" \
    flatpak run io.github.sebastianbecker2.videodedup.server &
elif [[ "${FMT}" == snap ]]; then
  export SNAP=/tmp/vd-snap
  export SNAP_COMMON=/tmp/vd-snap-common
  install -d -o videodedup -g videodedup /tmp/vd-snap-common/cert /tmp/vd-snap-common/data
  vd_ensure_openssl || {
    echo "E2E: openssl required for snap TLS certificate" >&2
    exit 1
  }
  vd_setup_tls "${SNAP}/usr/lib/videodedupserver" "${SNAP_COMMON}/cert" || exit 1
  vd_require_tls_cert "${SNAP_COMMON}/cert/VideoDedup.pfx"
  _vd_bin="${SNAP}/usr/lib/videodedupserver/videodedup-server-launch.sh"
  [[ -x "${_vd_bin}" ]] || {
    echo "E2E: missing snap launch wrapper ${_vd_bin}" >&2
    exit 1
  }
  if [[ -n "${VIDEODEDUP_FFMPEG_LIB_ROOT:-}" ]]; then
    vd_exec_as_videodedup env \
      SNAP="${SNAP}" SNAP_COMMON="${SNAP_COMMON}" \
      ASPNETCORE_ENVIRONMENT=Production \
      DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1 \
      VIDEODEDUP_APP_DATA=/var/lib/videodedupserver \
      VIDEODEDUP_FFMPEG_LIB_ROOT="${VIDEODEDUP_FFMPEG_LIB_ROOT}" \
      "${_vd_bin}" &
  else
    vd_exec_as_videodedup env \
      SNAP="${SNAP}" SNAP_COMMON="${SNAP_COMMON}" \
      ASPNETCORE_ENVIRONMENT=Production \
      DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1 \
      VIDEODEDUP_APP_DATA=/var/lib/videodedupserver \
      "${_vd_bin}" &
  fi
else
  _vd_bin=/usr/lib/videodedupserver/VideoDedupService
  vd_tls_env_args
  if [[ -n "${VIDEODEDUP_FFMPEG_LIB_ROOT:-}" ]]; then
    vd_exec_as_videodedup env \
      "${VD_TLS_ENV_ARGS[@]}" \
      ASPNETCORE_ENVIRONMENT=Production \
      DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1 \
      VIDEODEDUP_APP_DATA=/var/lib/videodedupserver \
      VIDEODEDUP_FFMPEG_LIB_ROOT="${VIDEODEDUP_FFMPEG_LIB_ROOT}" \
      "${_vd_bin}" &
  else
    vd_exec_as_videodedup env \
      "${VD_TLS_ENV_ARGS[@]}" \
      ASPNETCORE_ENVIRONMENT=Production \
      DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1 \
      VIDEODEDUP_APP_DATA=/var/lib/videodedupserver \
      "${_vd_bin}" &
  fi
fi

_ready_wait=90
if [[ "${FMT}" == flatpak || "${FMT}" == snap ]]; then
  _ready_wait=180
fi
for _ in $(seq 1 "${_ready_wait}"); do
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
