#!/usr/bin/env python3
"""
Install videodedupserver from a .deb inside Docker, publish gRPC on a host port, run VideoDedupGrpcSmoke
and (when fixtures exist) VideoDedupGrpcComparisonSmoke. Implemented in Python so Windows/Git Bash MSYS
path rewriting does not affect docker/dotnet children.

See argparse help for options and env vars (VD_GRPC_SMOKE_IMAGE, VD_UBUNTU_SMOKE_HOST_PORT, etc.).
"""

from __future__ import annotations

import argparse
import atexit
import os
import random
import shutil
import subprocess
import sys
import tempfile
import time
from pathlib import Path

from docker_e2e_common import (
    docker_host_path,
    docker_ok,
    latest_artifact,
    mktemp_dir_under,
    publish_dotnet_project,
    repo_root_from_e2e_dir,
    run,
    run_capture,
    which,
)

E2E_DIR = Path(__file__).resolve().parent
ROOT = repo_root_from_e2e_dir(E2E_DIR)


def die(msg: str, code: int = 1) -> None:
    print(msg, file=sys.stderr)
    raise SystemExit(code)


def arch_default() -> str:
    import platform

    m = platform.machine().lower()
    if m in ("x86_64", "amd64"):
        return "amd64"
    if m in ("aarch64", "arm64"):
        return "arm64"
    return "amd64"


def rid_for_arch(arch: str) -> str:
    if arch == "amd64":
        return "linux-x64"
    if arch == "arm64":
        return "linux-arm64"
    die(f"Unsupported --arch {arch} (use amd64 or arm64)")


def host_can_build_deb() -> bool:
    return all(which(x) for x in ("dotnet", "git", "python3", "fakeroot", "dpkg-deb"))


def build_deb_on_host(arch: str) -> None:
    print(f"Building .deb on host (stage + build-deb, arch={arch}) ...")
    env = os.environ.copy()
    r = run_capture(["git", "-C", str(ROOT), "log", "-1", "--format=%ct"])
    env["SOURCE_DATE_EPOCH"] = (r.stdout.strip() or "0") if r.returncode == 0 else "0"
    for p in list((ROOT / "packaging/tools").glob("*.sh")) + [ROOT / "packaging/common/generate-metadata.sh"]:
        if p.is_file() and os.name != "nt":
            try:
                p.chmod(p.stat().st_mode | 0o111)
            except OSError:
                pass
    bash = which("bash")
    if not bash:
        die("bash is required to run packaging/tools/stage.sh on the host.")
    for script in ("packaging/tools/stage.sh", "packaging/tools/build-deb.sh"):
        sp = ROOT / script
        if sp.exists() and os.name != "nt":
            try:
                sp.chmod(sp.stat().st_mode | 0o111)
            except OSError:
                pass
    run([bash, str(ROOT / "packaging/tools/stage.sh"), "--arch", arch], cwd=ROOT, env=env)
    run([bash, str(ROOT / "packaging/tools/build-deb.sh"), "--arch", arch], cwd=ROOT, env=env)


def build_deb_in_docker(arch: str) -> None:
    print(f"Building .deb in Docker ({ROOT / 'packaging/docker/Dockerfile.build-deb'}, arch={arch}) ...")
    out_tmp = tempfile.mkdtemp(prefix="vd-deb-docker.", dir=str(ROOT / "packaging/out"))
    out_tmp_path = Path(out_tmp)
    try:
        r = subprocess.run(
            [
                "docker",
                "build",
                "-f",
                str(ROOT / "packaging/docker/Dockerfile.build-deb"),
                "--build-arg",
                f"ARCH={arch}",
                "--target",
                "artifacts",
                "--output",
                f"type=local,dest={out_tmp_path}",
                str(ROOT),
            ],
            env={**os.environ, "DOCKER_BUILDKIT": "1"},
        )
        if r.returncode != 0:
            die("Docker build of .deb failed.")
        debs = list(out_tmp_path.glob("*.deb"))
        if len(debs) != 1:
            die("Docker build produced no (or multiple) .deb in output directory.")
        deb_out = ROOT / "packaging/out" / arch / "deb"
        deb_out.mkdir(parents=True, exist_ok=True)
        dest = deb_out / debs[0].name
        shutil.copy2(debs[0], dest)
        print(f"Wrote {dest}")
    finally:
        shutil.rmtree(out_tmp_path, ignore_errors=True)


def ensure_deb_package(arch: str, explicit: Path | None) -> Path:
    if explicit is not None:
        if not explicit.is_file():
            die(f"Not a file: {explicit}")
        return explicit.resolve()

    pat = str(ROOT / "packaging/out" / arch / "deb" / "*.deb")
    found = latest_artifact(pat)
    if found is not None:
        print(f"Using existing package: {found}")
        return found

    print(f"No .deb under {ROOT / 'packaging/out' / arch / 'deb'} - building ...", file=sys.stderr)
    if host_can_build_deb():
        build_deb_on_host(arch)
    elif which("docker") and docker_ok():
        build_deb_in_docker(arch)
    else:
        die(
            "Cannot build .deb: install dotnet/git/python3/fakeroot/dpkg-deb, or install/start Docker.\n"
            "  Host: ./packaging/tools/build-deb-one-shot.sh --arch %s\n"
            "  Docker: DOCKER_BUILDKIT=1 docker build -f packaging/docker/Dockerfile.build-deb ..."
            % arch
        )

    found = latest_artifact(pat)
    if found is None:
        die(f"Build finished but no .deb found under packaging/out/{arch}/deb/")
    print(f"Built package: {found}")
    return found


def publish_smoke_project(rel_csproj: str, out_subdir: str, arch: str, rid: str) -> None:
    out_dir = ROOT / "packaging/out" / arch / out_subdir
    publish_dotnet_project(ROOT, rel_csproj, out_dir, rid)


def ensure_grpc_smoke_base_image(tag: str) -> None:
    dockerfile = ROOT / "packaging/docker/Dockerfile.grpc-smoke-base"
    rebuild = os.environ.get("VD_REBUILD_GRPC_SMOKE_BASE", "0") == "1"
    ins = run_capture(["docker", "image", "inspect", tag])
    if not rebuild and ins.returncode == 0:
        return
    print(f"Building grpc-smoke base image {tag} ...")
    out_parent = ROOT / "packaging/out"
    out_parent.mkdir(parents=True, exist_ok=True)
    try:
        ctx = mktemp_dir_under(out_parent, ".grpc-smoke-base-ctx.")
    except OSError:
        ctx = out_parent / f".grpc-smoke-base-ctx.{os.getpid()}"
        shutil.rmtree(ctx, ignore_errors=True)
        ctx.mkdir(parents=True)
    try:
        shutil.copy2(dockerfile, ctx / "Dockerfile")
        run(["docker", "build", "-t", tag, str(ctx)], env={**os.environ, "DOCKER_BUILDKIT": "1"})
    finally:
        shutil.rmtree(ctx, ignore_errors=True)


def docker_logs(container: str) -> None:
    subprocess.run(["docker", "logs", container], stderr=subprocess.STDOUT)


def main() -> None:
    p = argparse.ArgumentParser(description="Docker gRPC deep smoke (deb + VideoDedupGrpcSmoke + comparison smoke).")
    p.add_argument("--arch", choices=("amd64", "arm64"), default="", help="default: from uname")
    p.add_argument("--image", default="", metavar="IMAGE", help="server image (overrides VD_GRPC_SMOKE_IMAGE / base)")
    p.add_argument("--deb", type=Path, default=None, metavar="PATH.deb", help="explicit .deb to install")
    p.add_argument("--host-port", default=os.environ.get("VD_UBUNTU_SMOKE_HOST_PORT", "51726"))
    args = p.parse_args()

    arch = args.arch or arch_default()
    rid = rid_for_arch(arch)

    try:
        port = int(args.host_port)
    except ValueError:
        die(f"Invalid host port: {args.host_port}")
    if not (1 <= port <= 65535):
        die(f"Invalid host port: {port}")

    if not which("docker"):
        die("docker not found")
    if not docker_ok():
        die("Docker daemon not reachable")

    opt_image = args.image.strip()
    grpc_base = os.environ.get("VD_GRPC_SMOKE_BASE_IMAGE", "videodedup/grpc-smoke-base:trixie")
    if opt_image:
        server_image = opt_image
    elif os.environ.get("VD_GRPC_SMOKE_IMAGE"):
        server_image = os.environ["VD_GRPC_SMOKE_IMAGE"].strip()
    else:
        ensure_grpc_smoke_base_image(grpc_base)
        server_image = grpc_base

    pkg_abs = ensure_deb_package(arch, args.deb)

    print(f"Publishing VideoDedupGrpcSmoke to {ROOT / 'packaging/out' / arch / 'e2e-smoke'} ...")
    publish_smoke_project("VideoDedupGrpcSmoke/VideoDedupGrpcSmoke.csproj", "e2e-smoke", arch, rid)
    smoke_abs = (ROOT / "packaging/out" / arch / "e2e-smoke").resolve()

    entry = ROOT / "packaging/tests/e2e/server-entrypoint.sh"
    entry_vol = docker_host_path(entry)
    pkg_vol = docker_host_path(pkg_abs)
    smoke_vol = docker_host_path(smoke_abs)

    fixtures_host = ROOT / "packaging/tests/fixtures/grpc-smoke"
    fixtures_server = "/tmp/vd-fixtures/grpc-smoke"
    compare_left = ""
    compare_right = ""

    mounts = [
        "-v",
        f"{entry_vol}:/entrypoint.sh:ro",
        "-v",
        f"{pkg_vol}:/tmp/videodedupserver.deb:ro",
    ]
    left_mp4 = fixtures_host / "left.mp4"
    right_mp4 = fixtures_host / "right.mp4"
    if left_mp4.is_file() and right_mp4.is_file():
        mounts += ["-v", f"{docker_host_path(fixtures_host)}:{fixtures_server}:ro"]
        compare_left = f"{fixtures_server}/left.mp4"
        compare_right = f"{fixtures_server}/right.mp4"
        print(f"Using gRPC comparison fixtures at {fixtures_server} (in container).")
    else:
        print(
            f"Note: grpc-smoke fixtures not found under {fixtures_host}; comparison deep smoke will be skipped.",
            file=sys.stderr,
        )

    container = f"videodedup-grpc-smoke-{os.getpid()}-{random.randint(0, 99999)}"

    def cleanup() -> None:
        subprocess.run(["docker", "rm", "-f", container], capture_output=True)

    atexit.register(cleanup)

    if arch == "arm64":
        ffmpeg_lib = "/usr/lib/aarch64-linux-gnu"
    else:
        ffmpeg_lib = "/usr/lib/x86_64-linux-gnu"

    print(f"Starting {container} (image={server_image}, published localhost:{port} -> container :51726) ...")
    run(
        [
            "docker",
            "run",
            "-d",
            "--name",
            container,
            "--privileged",
            "-p",
            f"{port}:51726",
            "-e",
            "VD_PACKAGE_FORMAT=deb",
            "-e",
            "VD_FIREWALL=nft",
            "-e",
            f"VIDEODEDUP_FFMPEG_LIB_ROOT={ffmpeg_lib}",
            *mounts,
            server_image,
            "bash",
            "/entrypoint.sh",
        ]
    )

    print("Waiting for gRPC readiness (/tmp/vd-ready in container) ...")
    ready = False
    for _ in range(180):
        r = subprocess.run(
            ["docker", "exec", container, "test", "-f", "/tmp/vd-ready"],
            capture_output=True,
        )
        if r.returncode == 0:
            ready = True
            break
        time.sleep(1)
    if not ready:
        docker_logs(container)
        die("Server did not become ready in time.", 1)

    grpc_url = f"http://127.0.0.1:{port}"

    def log_compare_env(name: str) -> None:
        print(f"E2E [{name}] VIDEODEDUP_SMOKE_COMPARE_LEFT={compare_left or '<unset>'}", file=sys.stderr)
        print(f"E2E [{name}] VIDEODEDUP_SMOKE_COMPARE_RIGHT={compare_right or '<unset>'}", file=sys.stderr)

    def run_smoke(url: str) -> None:
        log_compare_env("VideoDedupGrpcSmoke")
        in_docker = os.environ.get("VD_SMOKE_IN_DOCKER", "0") == "1"
        env = os.environ.copy()
        env["VIDEODEDUP_SMOKE_COMPARE_LEFT"] = compare_left
        env["VIDEODEDUP_SMOKE_COMPARE_RIGHT"] = compare_right
        if not in_docker and which("dotnet"):
            smoke_dll = docker_host_path(smoke_abs / "VideoDedupGrpcSmoke.dll")
            r = subprocess.run(["dotnet", smoke_dll, url], env=env)
            if r.returncode != 0:
                die(f"VideoDedupGrpcSmoke exited with code {r.returncode}", r.returncode)
            return
        print(
            "Running smoke inside mcr.microsoft.com/dotnet/runtime:8.0 "
            "(host has no dotnet; set VD_SMOKE_IN_DOCKER=1 to force) ...",
            file=sys.stderr,
        )
        smoke_url = url.replace("127.0.0.1", "host.docker.internal").replace("localhost", "host.docker.internal")
        r = subprocess.run(
            [
                "docker",
                "run",
                "--rm",
                "--add-host=host.docker.internal:host-gateway",
                "-e",
                f"VIDEODEDUP_SMOKE_COMPARE_LEFT={compare_left}",
                "-e",
                f"VIDEODEDUP_SMOKE_COMPARE_RIGHT={compare_right}",
                "-v",
                f"{smoke_vol}:/smoke:ro",
                "mcr.microsoft.com/dotnet/runtime:8.0",
                "dotnet",
                "/smoke/VideoDedupGrpcSmoke.dll",
                smoke_url,
            ]
        )
        if r.returncode != 0:
            die(f"VideoDedupGrpcSmoke (docker runtime) exited with code {r.returncode}", r.returncode)

    def run_comparison_smoke(url: str, comparison_abs: Path) -> None:
        log_compare_env("VideoDedupGrpcComparisonSmoke")
        in_docker = os.environ.get("VD_SMOKE_IN_DOCKER", "0") == "1"
        env = os.environ.copy()
        env["VIDEODEDUP_SMOKE_COMPARE_LEFT"] = compare_left
        env["VIDEODEDUP_SMOKE_COMPARE_RIGHT"] = compare_right
        if not in_docker and which("dotnet"):
            dll = docker_host_path(comparison_abs / "VideoDedupGrpcComparisonSmoke.dll")
            r = subprocess.run(["dotnet", dll, url], env=env)
            if r.returncode != 0:
                die(f"VideoDedupGrpcComparisonSmoke exited with code {r.returncode}", r.returncode)
            return
        print(
            "Running VideoComparison deep smoke inside mcr.microsoft.com/dotnet/runtime:8.0 ...",
            file=sys.stderr,
        )
        smoke_url = url.replace("127.0.0.1", "host.docker.internal").replace("localhost", "host.docker.internal")
        r = subprocess.run(
            [
                "docker",
                "run",
                "--rm",
                "--add-host=host.docker.internal:host-gateway",
                "-e",
                f"VIDEODEDUP_SMOKE_COMPARE_LEFT={compare_left}",
                "-e",
                f"VIDEODEDUP_SMOKE_COMPARE_RIGHT={compare_right}",
                "-v",
                f"{docker_host_path(comparison_abs.resolve())}:/comparison-smoke:ro",
                "mcr.microsoft.com/dotnet/runtime:8.0",
                "dotnet",
                "/comparison-smoke/VideoDedupGrpcComparisonSmoke.dll",
                smoke_url,
            ]
        )
        if r.returncode != 0:
            die(
                f"VideoDedupGrpcComparisonSmoke (docker runtime) exited with code {r.returncode}",
                r.returncode,
            )

    print(f"Running gRPC smoke against {grpc_url} ...")
    try:
        run_smoke(grpc_url)
    except SystemExit as e:
        if e.code not in (0, None):
            print("--- server container logs ---", file=sys.stderr)
            docker_logs(container)
        raise

    if compare_left and compare_right:
        print(f"Publishing VideoDedupGrpcComparisonSmoke to {ROOT / 'packaging/out' / arch / 'e2e-comparison-smoke'} ...")
        publish_smoke_project(
            "VideoDedupGrpcComparisonSmoke/VideoDedupGrpcComparisonSmoke.csproj",
            "e2e-comparison-smoke",
            arch,
            rid,
        )
        comp_abs = ROOT / "packaging/out" / arch / "e2e-comparison-smoke"
        print(f"Running gRPC comparison deep smoke (DIFFERENT, DUPLICATE, cancel) against {grpc_url} ...")
        try:
            run_comparison_smoke(grpc_url, comp_abs.resolve())
        except SystemExit as e:
            if e.code not in (0, None):
                print("--- server container logs (comparison deep smoke failed) ---", file=sys.stderr)
                docker_logs(container)
            raise
    else:
        print("Skipping VideoComparison deep smoke (grpc-smoke fixtures not mounted / paths unset).", file=sys.stderr)

    print(f"OK: Docker gRPC smoke passed ({server_image}, {pkg_abs}).")


if __name__ == "__main__":
    main()
