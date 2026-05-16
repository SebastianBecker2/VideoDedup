#!/usr/bin/env python3
"""
Install-smoke tests via Docker (Python port of packaging/tests/install/docker-install-*.sh).
No host bash required. Nested docker: set VD_DOCKER_BIND_SRC like the shell scripts.
"""

from __future__ import annotations

import argparse
import glob
import os
import shutil
import subprocess
import sys
import tempfile
from pathlib import Path

_REPO_ROOT = Path(__file__).resolve().parents[2]
if str(_REPO_ROOT) not in sys.path:
    sys.path.insert(0, str(_REPO_ROOT))

from packaging.ci.docker_paths import docker_host_path, docker_ok, repo_path_for_docker_bind


def repo_root() -> Path:
    return _REPO_ROOT


def die(msg: str, code: int = 1) -> None:
    print(msg, file=sys.stderr)
    raise SystemExit(code)


def _bash_stdin(script: str) -> str:
    return script.replace("\r\n", "\n").replace("\r", "\n")


def latest_glob(pattern: str) -> Path | None:
    paths = [Path(p) for p in glob.glob(pattern)]
    if not paths:
        return None
    paths.sort(key=lambda p: p.stat().st_mtime, reverse=True)
    return paths[0]


def resolve_artifact(kind: str, arch: str, explicit: str | None) -> Path:
    root = repo_root()
    if explicit:
        p = Path(explicit)
        if not p.is_file():
            die(f"Not a file: {p}")
        return p.resolve()

    if kind == "deb":
        found = latest_glob(str(root / "packaging/out" / arch / "deb" / "*.deb"))
        hint = "packaging/tools/build-deb.sh or Docker Dockerfile.build-deb"
    elif kind == "rpm":
        found = latest_glob(str(root / "packaging/out" / arch / "rpm" / "*" / "*.rpm"))
        hint = "packaging/tools/build-rpm.sh"
    elif kind == "pacman":
        found = latest_glob(str(root / "packaging/out" / arch / "pacman" / "*.pkg.tar.zst"))
        hint = "packaging/tools/build-pacman.sh"
    elif kind == "flatpak":
        found = latest_glob(str(root / "packaging/out" / arch / "flatpak" / "*.flatpak"))
        hint = "packaging/tools/build-flatpak.sh"
    else:
        die(f"Unknown kind {kind}")
    if found is None:
        die(f"No {kind} artifact for --arch {arch}; build one first ({hint}).")
    return found


def vol_path(root: Path, abs_path: Path) -> str:
    p = repo_path_for_docker_bind(root, abs_path)
    return docker_host_path(p)


def run_deb(arch: str, deb_path: Path) -> None:
    root = repo_root()
    print(f"Using {deb_path}")
    deb_vol = vol_path(root, deb_path)
    script = r"""set -eu
export DEBIAN_FRONTEND=noninteractive
apt-get update -qq
apt-get install -y -qq /tmp/videodedupserver.deb

dpkg -l videodedupserver | grep -q ^ii

test -x /usr/lib/videodedupserver/VideoDedupService
test -f /usr/lib/videodedupserver/appsettings.json
grep -q "51726" /usr/lib/videodedupserver/appsettings.json
test -f /usr/lib/videodedupserver/firewall/open-port-nftables.sh
test -f /usr/lib/videodedupserver/firewall/configure-firewall-interactive.sh
test -f /usr/share/doc/videodedupserver/README.firewall
id videodedup >/dev/null

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

echo "deb install smoke OK"
"""
    r = subprocess.run(
        [
            "docker",
            "run",
            "--rm",
            "-v",
            f"{deb_vol}:/tmp/videodedupserver.deb:ro",
            "debian:bookworm-slim",
            "bash",
            "-s",
        ],
        input=_bash_stdin(script).encode("utf-8"),
    )
    if r.returncode != 0:
        die("Docker .deb install test failed.", r.returncode)
    print("Docker .deb install test passed.")


def run_rpm(arch: str, rpm_path: Path) -> None:
    root = repo_root()
    inner = (root / "packaging/tests/install/rpm-install-inner-smoke.sh").resolve()
    print(f"Using {rpm_path}")
    rpm_vol = vol_path(root, rpm_path)
    inner_vol = vol_path(root, inner)
    env = os.environ.copy()
    env.setdefault("MSYS2_ARG_CONV_EXCL", "*")
    r = subprocess.run(
        [
            "docker",
            "run",
            "--rm",
            "-v",
            f"{rpm_vol}:/tmp/videodedupserver.rpm:ro",
            "-v",
            f"{inner_vol}:/inner-smoke.sh:ro",
            "fedora:40",
            "bash",
            "/inner-smoke.sh",
        ],
        env=env,
    )
    if r.returncode != 0:
        die("Docker .rpm install test failed.", r.returncode)
    print("Docker .rpm install test passed.")


def run_pacman(arch: str, pkg_path: Path) -> None:
    root = repo_root()
    print(f"Using {pkg_path}")
    pkg_vol = vol_path(root, pkg_path)
    script = r"""set -eu
pacman-key --init 2>/dev/null || true
pacman-key --populate archlinux 2>/dev/null || true
pacman -Sy --noconfirm archlinux-keyring
pacman -Sy --noconfirm --needed iproute2
pacman -U --noconfirm /tmp/videodedupserver.pkg.tar.zst

test -x /usr/lib/videodedupserver/VideoDedupService
test -f /usr/lib/videodedupserver/appsettings.json
grep -q "51726" /usr/lib/videodedupserver/appsettings.json
test -f /usr/lib/videodedupserver/firewall/open-port-nftables.sh
id videodedup >/dev/null

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

echo "pacman install smoke OK"
"""
    r = subprocess.run(
        [
            "docker",
            "run",
            "--rm",
            "-v",
            f"{pkg_vol}:/tmp/videodedupserver.pkg.tar.zst:ro",
            "archlinux:latest",
            "bash",
            "-s",
        ],
        input=_bash_stdin(script).encode("utf-8"),
    )
    if r.returncode != 0:
        die("Docker pacman install test failed.", r.returncode)
    print("Docker pacman install test passed.")


def ensure_flatpak_smoke_base_image(root: Path, tag: str) -> None:
    if tag == "fedora:40":
        return
    dockerfile = root / "packaging/docker/Dockerfile.flatpak-smoke-fedora"
    env = os.environ.copy()
    env.setdefault("MSYS2_ARG_CONV_EXCL", "*")
    rebuild = os.environ.get("VD_REBUILD_FLATPAK_SMOKE_BASE", "0") == "1"
    insp = subprocess.run(["docker", "image", "inspect", tag], capture_output=True, env=env)
    if not rebuild and insp.returncode == 0:
        return
    print(f"Building flatpak-smoke base image {tag} ...")
    root / "packaging/out"
    (root / "packaging/out").mkdir(parents=True, exist_ok=True)
    ctx = tempfile.mkdtemp(prefix=".flatpak-smoke-base-ctx.", dir=str(root / "packaging/out"))
    try:
        shutil.copy2(dockerfile, Path(ctx) / "Dockerfile")
        b = subprocess.run(
            ["docker", "build", "-t", tag, ctx],
            env={**env, "DOCKER_BUILDKIT": "1"},
        )
        if b.returncode != 0:
            die("docker build of flatpak-smoke base failed.")
    finally:
        shutil.rmtree(ctx, ignore_errors=True)


def run_flatpak(arch: str, fb_path: Path) -> None:
    root = repo_root()
    tag = os.environ.get("VD_FLATPAK_SMOKE_BASE_IMAGE", "videodedup/flatpak-smoke-fedora:40")
    ensure_flatpak_smoke_base_image(root, tag)
    print(f"Using {fb_path} (image={tag})")
    fb_vol = vol_path(root, fb_path)
    env = os.environ.copy()
    env.setdefault("MSYS2_ARG_CONV_EXCL", "*")
    script = r"""set -eu
if [[ ! -f /etc/videodedup-flatpak-smoke-base ]]; then
  dnf -y -q install flatpak
  flatpak remote-add --if-not-exists flathub https://flathub.org/repo/flathub.flatpakrepo
  flatpak install -y --noninteractive flathub org.freedesktop.Platform//24.08
fi
flatpak install -y --noninteractive --bundle /tmp/videodedupserver.flatpak

install -d -m 0755 -o root -g root /tmp/vd-xdg
export XDG_RUNTIME_DIR=/tmp/vd-xdg
timeout 25s flatpak run io.github.sebastianbecker2.videodedup.server 2>&1 | tee /tmp/vd-smoke.log

if ! grep -qE "Now listening|Application started" /tmp/vd-smoke.log; then
  echo "--- smoke log ---" >&2
  cat /tmp/vd-smoke.log >&2
  exit 1
fi

echo "flatpak install smoke OK"
"""
    r = subprocess.run(
        [
            "docker",
            "run",
            "--rm",
            "-v",
            f"{fb_vol}:/tmp/videodedupserver.flatpak:ro",
            tag,
            "bash",
            "-s",
        ],
        input=_bash_stdin(script).encode("utf-8"),
        env=env,
    )
    if r.returncode != 0:
        die("Docker flatpak install test failed.", r.returncode)
    print("Docker flatpak install test passed.")


def main() -> int:
    ap = argparse.ArgumentParser(description="Docker install smoke tests (deb/rpm/pacman/flatpak).")
    sub = ap.add_subparsers(dest="cmd", required=True)

    for name, helpt in (
        ("deb", "Smoke-test .deb in debian:bookworm-slim"),
        ("rpm", "Smoke-test .rpm in fedora:40"),
        ("pacman", "Smoke-test .pkg.tar.zst in archlinux:latest"),
        ("flatpak", "Smoke-test .flatpak bundle"),
    ):
        p = sub.add_parser(name, help=helpt)
        p.add_argument("--arch", default="amd64", choices=("amd64", "arm64"))
        p.add_argument("path", nargs="?", help="Explicit package path")

    args = ap.parse_args()
    if not docker_ok():
        die("docker not found or daemon not reachable.")

    root = repo_root()
    os.chdir(root)

    path_arg: str | None = getattr(args, "path", None)
    arch = args.arch

    if args.cmd == "deb":
        run_deb(arch, resolve_artifact("deb", arch, path_arg))
    elif args.cmd == "rpm":
        run_rpm(arch, resolve_artifact("rpm", arch, path_arg))
    elif args.cmd == "pacman":
        run_pacman(arch, resolve_artifact("pacman", arch, path_arg))
    elif args.cmd == "flatpak":
        run_flatpak(arch, resolve_artifact("flatpak", arch, path_arg))
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
