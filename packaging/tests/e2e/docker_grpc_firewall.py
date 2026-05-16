#!/usr/bin/env python3
"""
E2E: Linux container installs videodedupserver (deb/rpm/staged/pacman/snap/flatpak), firewall, service;
VideoDedupGrpcSmoke runs in a sibling Docker container on the same user-defined network (avoids Windows
Docker Desktop host port quirks). Opt in to host dotnet with VD_SMOKE_USE_HOST_DOTNET=1 or legacy
VD_SMOKE_IN_DOCKER=0 when dotnet is on PATH.
"""

from __future__ import annotations

import argparse
import atexit
import os
import random
import secrets
import shutil
import subprocess
import sys
import time
from pathlib import Path

from docker_e2e_common import (
    deb_package_has_tls_support,
    docker_host_path,
    docker_ok,
    extract_server_cert,
    file_has_crlf,
    latest_artifact,
    mktemp_dir_under,
    publish_dotnet_project,
    repo_root_from_e2e_dir,
    rpm_package_has_tls_support,
    run,
    run_capture,
    which,
)

E2E_DIR = Path(__file__).resolve().parent
ROOT = repo_root_from_e2e_dir(E2E_DIR)

if str(ROOT) not in sys.path:
    sys.path.insert(0, str(ROOT))

from packaging.ci.deb_docker_build import build_deb_via_docker  # noqa: E402
from packaging.ci.rpm_docker_build import build_rpm_via_docker  # noqa: E402

HELP_EPILOG = """
Preset images debian:bookworm-slim and fedora:40 are rebuilt locally as cached bases unless --srv-image is set.
VD_FIREWALL_SMOKE_DEBIAN_IMAGE / VD_FIREWALL_SMOKE_FEDORA_IMAGE: override tags.
VD_REBUILD_FIREWALL_SMOKE_DEBIAN_BASE=1 / VD_REBUILD_FIREWALL_SMOKE_FEDORA_BASE=1: force docker build.
On GitHub Actions bases are off by default (set VD_USE_FIREWALL_SMOKE_BASE=1 to enable). VD_SKIP_FIREWALL_SMOKE_BASE=1 disables locally.
VD_SMOKE_USE_HOST_DOTNET=1: run VideoDedupGrpcSmoke with host dotnet (faster on Linux; can fail on Windows Docker Desktop).
VD_SMOKE_IN_DOCKER=0: legacy alias for host dotnet when dotnet is on PATH.
"""


def die(msg: str, code: int = 1) -> None:
    print(msg, file=sys.stderr)
    raise SystemExit(code)


def allocate_e2e_user_bridge(net: str) -> None:
    """
    Create an IPv4+IPv6 user-defined bridge. Random /24 and /64 avoid overlap when several
    docker_grpc_firewall.py processes run concurrently (e.g. packaging/tests/run_all_linux_host.py -j).
    """
    last_err = ""
    for _ in range(16):
        b1 = secrets.randbelow(256)
        b2 = secrets.randbelow(256)
        v4_subnet = f"10.{b1}.{b2}.0/24"
        v4_gw = f"10.{b1}.{b2}.1"
        a = secrets.randbelow(65536)
        b = secrets.randbelow(65536)
        c = secrets.randbelow(65536)
        v6_subnet = f"fd00:{a:04x}:{b:04x}:{c:04x}::/64"
        r = subprocess.run(
            [
                "docker",
                "network",
                "create",
                "--driver",
                "bridge",
                "--subnet",
                v4_subnet,
                "--gateway",
                v4_gw,
                "--ipv6",
                "--subnet",
                v6_subnet,
                net,
            ],
            capture_output=True,
        )
        if r.returncode == 0:
            return
        last_err = (r.stderr or r.stdout or b"").decode("utf-8", errors="replace").strip()

    die(
        f'Failed to create dual-stack Docker network "{net}" after retries.\n'
        f"Last docker message:\n{last_err or '(empty)'}\n\n"
        "Typical causes: IPv6 not enabled for Docker user-defined networks (see packaging/common/docs/local-build.md), "
        "or subnet overlap if many E2E networks already exist (docker network prune)."
    )


def rid_for_arch(arch: str) -> str:
    if arch == "amd64":
        return "linux-x64"
    if arch == "arm64":
        return "linux-arm64"
    die(f"Unsupported --arch {arch}")


def resolve_distro_and_image(
    distro: str,
    srv_image: str,
    fmt: str,
) -> tuple[str, str]:
    d = distro.strip()
    img = srv_image.strip()

    if img and not d:
        inferred = {
            "debian:bookworm-slim": "debian",
            "ubuntu:24.04": "ubuntu",
            "fedora:40": "fedora",
            "rockylinux/rockylinux:9": "rocky",
            "opensuse/tumbleweed": "opensuse",
            "archlinux:latest": "arch",
            "manjarolinux/base": "manjaro",
        }.get(img, "custom")
        d = inferred

    if not d:
        d = {
            "deb": "debian",
            "rpm": "fedora",
            "staged": "arch",
            "pacman": "arch",
            "snap": "ubuntu",
            "flatpak": "fedora",
        }[fmt]

    if not img:
        preset = {
            "debian": "debian:bookworm-slim",
            "ubuntu": "ubuntu:24.04",
            "fedora": "fedora:40",
            "rocky": "rockylinux/rockylinux:9",
            "opensuse": "opensuse/tumbleweed",
            "arch": "archlinux:latest",
            "manjaro": "manjarolinux/base",
        }
        if d == "custom":
            die("Set --srv-image when using DISTRO=custom")
        if d not in preset:
            die(f"Unknown --distro {d}")
        img = preset[d]

    return d, img


def ffmpeg_lib_root_hint(distro: str, arch: str) -> str | None:
    """
    Native libav directory for FFmpeg.AutoGen (matches docker_grpc_deep_smoke on Debian/Ubuntu).
    Preset avoids relying on server-entrypoint.sh compgen-only detection on minimal images.
    """
    d = distro.strip().lower()
    if d in ("debian", "ubuntu"):
        return "/usr/lib/aarch64-linux-gnu" if arch == "arm64" else "/usr/lib/x86_64-linux-gnu"
    if d in ("fedora", "rocky", "opensuse"):
        return "/usr/lib64"
    if d in ("arch", "manjaro"):
        return "/usr/lib"
    return None


def validate_combo(fmt: str, distro: str, firewall: str) -> None:
    if distro == "custom":
        return
    if fmt == "staged" and distro not in ("arch", "manjaro"):
        die(f"For --format staged use --distro arch or manjaro (got {distro})")
    if fmt == "deb" and distro not in ("debian", "ubuntu"):
        die(f"For --format deb use --distro debian or ubuntu (got {distro})")
    if fmt == "rpm" and distro not in ("fedora", "rocky", "opensuse"):
        die(f"For --format rpm use --distro fedora, rocky, or opensuse (got {distro})")
    if fmt == "pacman" and distro not in ("arch", "manjaro"):
        die(f"For --format pacman use --distro arch or manjaro (got {distro})")
    if fmt == "snap" and distro not in ("debian", "ubuntu"):
        die(f"For --format snap use --distro debian or ubuntu (got {distro})")
    if fmt == "flatpak" and distro != "fedora":
        die(f"For --format flatpak use --distro fedora (got {distro})")
    if firewall == "ufw" and distro not in ("debian", "ubuntu", "manjaro"):
        die(f"ufw E2E is supported on debian, ubuntu, or manjaro (got {distro})")


def ensure_staged_tree(arch: str, stage_dir: Path) -> None:
    marker = stage_dir / "cert-setup" / "generate-server-cert.sh"
    needs_restage = not marker.is_file() or file_has_crlf(marker)
    if needs_restage:
        print(f"Re-staging server payload ({stage_dir}) ...", file=sys.stderr)
        run([sys.executable, str(ROOT / "packaging/tools/stage.py"), "--arch", arch], cwd=ROOT)
    if not marker.is_file():
        die(f"Staged cert-setup missing at {marker} — run: python3 packaging/tools/stage.py --arch {arch}")


def build_deb_on_host(arch: str) -> None:
    bash = shutil.which("bash")
    if not bash:
        die("bash required to build .deb on host")
    run([sys.executable, str(ROOT / "packaging/tools/stage.py"), "--arch", arch], cwd=ROOT)
    run([bash, str(ROOT / "packaging/tools/build-deb.sh"), "--arch", arch], cwd=ROOT)


def build_rpm_on_host(arch: str) -> None:
    bash = shutil.which("bash")
    if not bash:
        die("bash required to build .rpm on host")
    run([sys.executable, str(ROOT / "packaging/tools/stage.py"), "--arch", arch], cwd=ROOT)
    run([bash, str(ROOT / "packaging/tools/build-rpm.sh"), "--arch", arch], cwd=ROOT)


def ensure_deb_package(arch: str, deb: Path) -> Path:
    if deb_package_has_tls_support(deb):
        return deb
    print(f"Existing .deb lacks TLS cert-setup ({deb}); rebuilding ...", file=sys.stderr)
    if docker_ok():
        build_deb_via_docker(ROOT, arch)
    else:
        build_deb_on_host(arch)
    pat = str(ROOT / "packaging/out" / arch / "deb" / "*.deb")
    found = latest_artifact(pat)
    if found is None or not deb_package_has_tls_support(found):
        die(f"Rebuilt .deb still lacks TLS cert-setup under packaging/out/{arch}/deb/")
    print(f"Built package: {found}")
    return found


def ensure_rpm_package(arch: str, rpm: Path) -> Path:
    if rpm_package_has_tls_support(rpm):
        return rpm
    print(f"Existing .rpm lacks TLS cert-setup ({rpm}); rebuilding ...", file=sys.stderr)
    if docker_ok():
        build_rpm_via_docker(ROOT, arch)
    else:
        build_rpm_on_host(arch)
    pat = str(ROOT / "packaging/out" / arch / "rpm" / "*" / "*.rpm")
    found = latest_artifact(pat)
    if found is None or not rpm_package_has_tls_support(found):
        die(f"Rebuilt .rpm still lacks TLS cert-setup under packaging/out/{arch}/rpm/")
    print(f"Built package: {found}")
    return found


def pick_package_glob(fmt: str, arch: str) -> tuple[str, str]:
    base = ROOT / "packaging/out" / arch
    if fmt == "deb":
        return str(base / "deb" / "*.deb"), "packaging/tools/build-deb.sh"
    if fmt == "rpm":
        return str(base / "rpm" / "*" / "*.rpm"), "packaging/tools/build-rpm.sh"
    if fmt == "pacman":
        return str(base / "pacman" / "*.pkg.tar.zst"), "packaging/tools/build-pacman.sh"
    if fmt == "snap":
        return str(base / "snap" / "*.snap"), "packaging/tools/build-snap.sh"
    if fmt == "flatpak":
        return str(base / "flatpak" / "*.flatpak"), "packaging/tools/build-flatpak.sh"
    die(f"internal: no candidate rule for {fmt}")


def ensure_firewall_smoke_debian_base(tag: str) -> None:
    dockerfile = ROOT / "packaging/docker/Dockerfile.firewall-smoke-debian-bookworm-slim"
    rebuild = os.environ.get("VD_REBUILD_FIREWALL_SMOKE_DEBIAN_BASE", "0") == "1"
    if not rebuild and run_capture(["docker", "image", "inspect", tag]).returncode == 0:
        return
    print(f"Building firewall-smoke Debian base {tag} ...")
    out_parent = ROOT / "packaging/out"
    try:
        ctx = mktemp_dir_under(out_parent, ".firewall-smoke-debian-ctx.")
    except OSError:
        ctx = out_parent / f".firewall-smoke-debian-ctx.{os.getpid()}"
        shutil.rmtree(ctx, ignore_errors=True)
        ctx.mkdir(parents=True)
    try:
        shutil.copy2(dockerfile, ctx / "Dockerfile")
        run(["docker", "build", "-t", tag, str(ctx)], env={**os.environ, "DOCKER_BUILDKIT": "1"})
    finally:
        shutil.rmtree(ctx, ignore_errors=True)


def ensure_firewall_smoke_fedora_base(tag: str) -> None:
    dockerfile = ROOT / "packaging/docker/Dockerfile.firewall-smoke-fedora-40"
    rebuild = os.environ.get("VD_REBUILD_FIREWALL_SMOKE_FEDORA_BASE", "0") == "1"
    if not rebuild and run_capture(["docker", "image", "inspect", tag]).returncode == 0:
        return
    print(f"Building firewall-smoke Fedora base {tag} ...")
    out_parent = ROOT / "packaging/out"
    try:
        ctx = mktemp_dir_under(out_parent, ".firewall-smoke-fedora-ctx.")
    except OSError:
        ctx = out_parent / f".firewall-smoke-fedora-ctx.{os.getpid()}"
        shutil.rmtree(ctx, ignore_errors=True)
        ctx.mkdir(parents=True)
    try:
        shutil.copy2(dockerfile, ctx / "Dockerfile")
        run(["docker", "build", "-t", tag, str(ctx)], env={**os.environ, "DOCKER_BUILDKIT": "1"})
    finally:
        shutil.rmtree(ctx, ignore_errors=True)


def e2e_srv_ip(net: str, srv: str, field: str) -> str:
    template = "{{ (index .NetworkSettings.Networks \"" + net + "\")." + field + " }}"
    r = run_capture(["docker", "inspect", "-f", template, srv])
    return r.stdout.strip() if r.returncode == 0 else ""


def main() -> None:
    p = argparse.ArgumentParser(
        description="Docker gRPC + firewall E2E.",
        formatter_class=argparse.RawDescriptionHelpFormatter,
        epilog=HELP_EPILOG,
    )
    p.add_argument("--arch", default="amd64", choices=("amd64", "arm64"))
    p.add_argument("--format", default="deb", choices=("deb", "rpm", "staged", "pacman", "snap", "flatpak"))
    p.add_argument("--distro", default="", help="debian|ubuntu|fedora|rocky|opensuse|arch|manjaro")
    p.add_argument("--srv-image", default="", dest="srv_image", metavar="IMAGE")
    p.add_argument("--firewall", default="nft", choices=("nft", "iptables", "ufw", "firewalld"))
    p.add_argument("--smoke-dir", default="", dest="smoke_dir", metavar="DIR")
    p.add_argument("pkg", nargs="?", default="", help="path/to/package (optional for staged or auto-pick)")
    args = p.parse_args()

    arch = args.arch
    fmt = args.format
    firewall = args.firewall
    srv_image_user_set = bool(args.srv_image.strip())
    smoke_dir_user_set = bool(args.smoke_dir.strip())

    distro, srv_image = resolve_distro_and_image(args.distro, args.srv_image, fmt)
    validate_combo(fmt, distro, firewall)

    rid = rid_for_arch(arch)
    stage_dir = ROOT / "packaging/.stage" / arch / "server"

    if distro == "opensuse" and fmt == "rpm":
        if not (stage_dir / "VideoDedupService").is_file():
            die(
                f"openSUSE RPM E2E mounts staged fallback at {stage_dir} - run: ./packaging/tools/stage.sh --arch {arch}"
            )

    if fmt == "staged":
        if not (stage_dir / "VideoDedupService").is_file():
            die(f"Staged server missing at {stage_dir} - run: ./packaging/tools/stage.sh --arch {arch}")
        ensure_staged_tree(arch, stage_dir)
        pkg_abs: Path | None = None
    else:
        pkg_arg = args.pkg.strip()
        if not pkg_arg:
            gpat, hint = pick_package_glob(fmt, arch)
            found = latest_artifact(gpat)
            if found is None:
                die(f"No {fmt} artifact under packaging/out/{arch}/ - build one first (e.g. {hint})")
            pkg_path = found
        else:
            pkg_path = Path(pkg_arg)
        if not pkg_path.is_file():
            die(f"Not a file: {pkg_path}")
        pkg_abs = pkg_path.resolve()
        if fmt == "deb":
            pkg_abs = ensure_deb_package(arch, pkg_abs)
        elif fmt == "rpm":
            pkg_abs = ensure_rpm_package(arch, pkg_abs)

    smoke_dir = Path(args.smoke_dir) if smoke_dir_user_set else ROOT / "packaging/out" / arch / "e2e-smoke"
    smoke_dir = smoke_dir.resolve()
    if not smoke_dir_user_set or not (smoke_dir / "VideoDedupGrpcSmoke.dll").is_file():
        print(f"Publishing VideoDedupGrpcSmoke to {smoke_dir} ...")
        publish_dotnet_project(ROOT, "VideoDedupGrpcSmoke/VideoDedupGrpcSmoke.csproj", smoke_dir, rid)

    if not which("docker"):
        die("docker not found")
    if not docker_ok():
        die("Docker daemon not reachable")

    debian_tag = os.environ.get("VD_FIREWALL_SMOKE_DEBIAN_IMAGE", "videodedup/firewall-smoke-debian:bookworm")
    fedora_tag = os.environ.get("VD_FIREWALL_SMOKE_FEDORA_IMAGE", "videodedup/firewall-smoke-fedora:40")

    if not srv_image_user_set:
        skip_base = os.environ.get("VD_SKIP_FIREWALL_SMOKE_BASE", "0") == "1" or (
            bool(os.environ.get("GITHUB_ACTIONS")) and os.environ.get("VD_USE_FIREWALL_SMOKE_BASE", "0") != "1"
        )
        if not skip_base:
            if srv_image == "debian:bookworm-slim":
                ensure_firewall_smoke_debian_base(debian_tag)
                srv_image = debian_tag
            elif srv_image == "fedora:40":
                ensure_firewall_smoke_fedora_base(fedora_tag)
                srv_image = fedora_tag

    net = f"videodedup-e2e-{os.getpid()}-{random.randint(0, 99999)}"
    srv = f"videodedup-e2e-srv-{os.getpid()}-{random.randint(0, 99999)}"
    smoke_abs = smoke_dir.resolve()

    compare_left = ""
    compare_right = ""

    def cleanup() -> None:
        subprocess.run(["docker", "rm", "-f", srv], capture_output=True)
        subprocess.run(["docker", "network", "rm", net], capture_output=True)

    atexit.register(cleanup)

    allocate_e2e_user_bridge(net)

    entry = ROOT / "packaging/tests/e2e/server-entrypoint.sh"
    entry_vol = docker_host_path(entry)
    smoke_vol = docker_host_path(smoke_abs)

    docker_env = ["-e", f"VD_PACKAGE_FORMAT={fmt}", "-e", f"VD_FIREWALL={firewall}"]
    _ffmpeg_hint = ffmpeg_lib_root_hint(distro, arch)
    if _ffmpeg_hint:
        docker_env += ["-e", f"VIDEODEDUP_FFMPEG_LIB_ROOT={_ffmpeg_hint}"]
        print(
            f"E2E: VIDEODEDUP_FFMPEG_LIB_ROOT={_ffmpeg_hint} (preset for distro={distro})",
            file=sys.stderr,
        )
    mounts = ["-v", f"{entry_vol}:/entrypoint.sh:ro"]

    fixtures_host = ROOT / "packaging/tests/fixtures/grpc-smoke"
    fixtures_server = "/tmp/vd-fixtures/grpc-smoke"
    left_mp4 = fixtures_host / "left.mp4"
    right_mp4 = fixtures_host / "right.mp4"
    if left_mp4.is_file() and right_mp4.is_file():
        mounts += ["-v", f"{docker_host_path(fixtures_host)}:{fixtures_server}:ro"]
        compare_left = f"{fixtures_server}/left.mp4"
        compare_right = f"{fixtures_server}/right.mp4"
    else:
        print("E2E: grpc-smoke fixtures missing; comparison RPCs will run with non-existent paths.", file=sys.stderr)

    if fmt == "deb" and pkg_abs is not None:
        mounts += ["-v", f"{docker_host_path(pkg_abs)}:/tmp/videodedupserver.deb:ro"]
    elif fmt == "rpm" and pkg_abs is not None:
        mounts += ["-v", f"{docker_host_path(pkg_abs)}:/tmp/videodedupserver.rpm:ro"]
        if distro == "opensuse":
            mounts += ["-v", f"{docker_host_path(stage_dir)}:/opt/videodedup-staged:ro"]
    elif fmt == "staged":
        mounts += ["-v", f"{docker_host_path(stage_dir)}:/opt/videodedup-staged:ro"]
    elif fmt == "pacman" and pkg_abs is not None:
        mounts += ["-v", f"{docker_host_path(pkg_abs)}:/tmp/videodedupserver.pkg.tar.zst:ro"]
    elif fmt == "snap" and pkg_abs is not None:
        mounts += ["-v", f"{docker_host_path(pkg_abs)}:/tmp/videodedupserver.snap:ro"]
    elif fmt == "flatpak" and pkg_abs is not None:
        mounts += ["-v", f"{docker_host_path(pkg_abs)}:/tmp/videodedupserver.flatpak:ro"]

    print(f"Starting server container ({srv}, image={srv_image}, format={fmt}, firewall={firewall}) ...")
    run(
        ["docker", "run", "-d", "--name", srv, "--network", net, "--privileged"]
        + docker_env
        + mounts
        + [srv_image, "bash", "/entrypoint.sh"]
    )

    print(f"Waiting for {srv} gRPC ({fmt} install + {firewall} + service) ...")
    ready = False
    for _ in range(240):
        if subprocess.run(["docker", "exec", srv, "test", "-f", "/tmp/vd-ready"], capture_output=True).returncode == 0:
            ready = True
            break
        time.sleep(1)
    if not ready:
        print("server did not become ready (missing /tmp/vd-ready)", file=sys.stderr)
        subprocess.run(["docker", "logs", srv], stderr=subprocess.STDOUT)
        die("server did not become ready (missing /tmp/vd-ready)", 1)

    ipv4 = e2e_srv_ip(net, srv, "IPAddress")
    ipv6 = e2e_srv_ip(net, srv, "GlobalIPv6Address")
    if not ipv4:
        die(f"E2E: server {srv} has no IPv4 address on network {net}")
    if not ipv6:
        die(
            f"E2E: server {srv} has no GlobalIPv6Address on {net}.\n"
            "Docker must assign IPv6 to custom bridge networks (enable IPv6 in Docker; see packaging/common/docs/local-build.md)."
        )

    # Per-container cert dir so parallel -j workers do not overwrite each other's VideoDedup.crt.
    cert_host_dir = ROOT / "packaging/out" / arch / "e2e-server-cert" / srv
    cert_host_dir.mkdir(parents=True, exist_ok=True)
    try:
        pinned_cert = extract_server_cert(srv, cert_host_dir, package_format=fmt)
    except RuntimeError as ex:
        subprocess.run(["docker", "logs", srv], stderr=subprocess.STDOUT)
        die(str(ex), 1)
    print(f"E2E: extracted server TLS cert to {pinned_cert}", flush=True)
    cert_vol = docker_host_path(pinned_cert.parent)
    pinned_in_container = "/smoke-cert/VideoDedup.crt"
    pinned_on_host = docker_host_path(pinned_cert)

    def log_compare_env(name: str) -> None:
        print(f"E2E [{name}] VIDEODEDUP_SMOKE_COMPARE_LEFT={compare_left or '<unset>'}", file=sys.stderr)
        print(f"E2E [{name}] VIDEODEDUP_SMOKE_COMPARE_RIGHT={compare_right or '<unset>'}", file=sys.stderr)

    def run_smoke(url: str, label: str) -> bool:
        log_compare_env(f"VideoDedupGrpcSmoke ({label})")
        print(f"Running gRPC smoke client ({label}: {url}) ...")
        use_host_dotnet = bool(which("dotnet")) and (
            os.environ.get("VD_SMOKE_USE_HOST_DOTNET", "").strip() == "1"
            or os.environ.get("VD_SMOKE_IN_DOCKER", "").strip() == "0"
        )
        env = os.environ.copy()
        env["VIDEODEDUP_SMOKE_COMPARE_LEFT"] = compare_left
        env["VIDEODEDUP_SMOKE_COMPARE_RIGHT"] = compare_right
        env["VIDEODEDUP_SMOKE_PINNED_CERT"] = pinned_on_host

        if use_host_dotnet:
            smoke_dll = docker_host_path(smoke_abs / "VideoDedupGrpcSmoke.dll")
            r = subprocess.run(["dotnet", smoke_dll, url], env=env)
            return r.returncode == 0

        docker_cmd = [
            "docker",
            "run",
            "--rm",
            "--network",
            net,
            "-e",
            f"VIDEODEDUP_SMOKE_COMPARE_LEFT={compare_left}",
            "-e",
            f"VIDEODEDUP_SMOKE_COMPARE_RIGHT={compare_right}",
            "-e",
            f"VIDEODEDUP_SMOKE_PINNED_CERT={pinned_in_container}",
            "-v",
            f"{smoke_vol}:/smoke:ro",
            "-v",
            f"{cert_vol}:/smoke-cert:ro",
            "mcr.microsoft.com/dotnet/runtime:8.0",
            "dotnet",
            "/smoke/VideoDedupGrpcSmoke.dll",
            url,
        ]
        env_docker = os.environ.copy()
        if os.name == "nt":
            env_docker.setdefault("MSYS2_ARG_CONV_EXCL", "*")
        r = subprocess.run(docker_cmd, env=env_docker)
        return r.returncode == 0

    ipv4_ok = True
    if not run_smoke(f"https://{ipv4}:51726", "IPv4"):
        ipv4_ok = False
        print("--- server container logs (IPv4 smoke failed) ---", file=sys.stderr)
        subprocess.run(["docker", "logs", srv], stderr=subprocess.STDOUT)
    if not run_smoke(f"https://[{ipv6}]:51726", "IPv6"):
        print("--- server container logs (IPv6 smoke failed) ---", file=sys.stderr)
        subprocess.run(["docker", "logs", srv], stderr=subprocess.STDOUT)
        die("IPv6 smoke failed", 1)

    if not ipv4_ok:
        print(
            "E2E gRPC + firewall passed for IPv6, but IPv4 smoke failed (server likely IPv6-only on this host).",
            file=sys.stderr,
        )
    else:
        print(f"E2E gRPC + firewall passed ({srv_image}, {fmt}, {firewall}; IPv4 + IPv6).")


if __name__ == "__main__":
    main()
