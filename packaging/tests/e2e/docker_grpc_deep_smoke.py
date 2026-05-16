#!/usr/bin/env python3
"""
Install videodedupserver from a .deb inside Docker, publish gRPC on a host port, run VideoDedupGrpcSmoke
and (when fixtures exist) VideoDedupGrpcComparisonSmoke + VideoDedupGrpcDedupSmoke.
When fixtures exist: duplicate left/right into *_copy_dedup.mp4 on the **host** under
``packaging/tests/fixtures/grpc-smoke`` **before** ``docker run`` (fixtures are bind-mounted read-write so
DedupSmoke can delete a duplicate file). ``finally`` removes the two copy files on the host. Implemented in Python so Windows/Git
Bash MSYS path rewriting does not affect docker/dotnet children.

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
import time
from concurrent.futures import ThreadPoolExecutor
from pathlib import Path

from docker_e2e_common import (
    deb_package_has_tls_support,
    docker_host_path,
    docker_ok,
    extract_server_cert,
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

if str(ROOT) not in sys.path:
    sys.path.insert(0, str(ROOT))

from packaging.ci.deb_docker_build import build_deb_via_docker  # noqa: E402


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
        die("bash is required to run packaging/tools/build-deb.sh on the host.")
    for script in ("packaging/tools/stage.sh", "packaging/tools/build-deb.sh"):
        sp = ROOT / script
        if sp.exists() and os.name != "nt":
            try:
                sp.chmod(sp.stat().st_mode | 0o111)
            except OSError:
                pass
    run([sys.executable, str(ROOT / "packaging/tools/stage.py"), "--arch", arch], cwd=ROOT, env=env)
    run([bash, str(ROOT / "packaging/tools/build-deb.sh"), "--arch", arch], cwd=ROOT, env=env)


def ensure_deb_package(arch: str, explicit: Path | None) -> Path:
    if explicit is not None:
        if not explicit.is_file():
            die(f"Not a file: {explicit}")
        deb = explicit.resolve()
        if not deb_package_has_tls_support(deb):
            die(
                f".deb lacks TLS cert-setup ({deb}). Rebuild after pulling TLS packaging changes:\n"
                f"  python3 packaging/tools/stage.py --arch {arch}\n"
                f"  packaging/tools/build-deb.sh --arch {arch}"
            )
        return deb

    pat = str(ROOT / "packaging/out" / arch / "deb" / "*.deb")
    found = latest_artifact(pat)
    if found is not None and not deb_package_has_tls_support(found):
        print(
            f"Existing package lacks TLS cert-setup ({found}); rebuilding ...",
            file=sys.stderr,
        )
        found = None
    if found is not None:
        print(f"Using existing package: {found}")
        return found

    print(f"No .deb under {ROOT / 'packaging/out' / arch / 'deb'} - building ...", file=sys.stderr)
    if docker_ok():
        build_deb_via_docker(ROOT, arch)
    elif host_can_build_deb():
        build_deb_on_host(arch)
    else:
        die(
            "Cannot build .deb: start Docker for an in-container build, or install "
            "dotnet/git/python3/fakeroot/dpkg-deb (and bash) for a host build.\n"
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


def grpc_smoke_base_image_has_openssl(tag: str) -> bool:
    r = run_capture(
        ["docker", "run", "--rm", tag, "sh", "-c", "command -v openssl"],
    )
    return r.returncode == 0


def ensure_grpc_smoke_base_image(tag: str) -> None:
    dockerfile = ROOT / "packaging/docker/Dockerfile.grpc-smoke-base"
    rebuild = os.environ.get("VD_REBUILD_GRPC_SMOKE_BASE", "0") == "1"
    ins = run_capture(["docker", "image", "inspect", tag])
    if not rebuild and ins.returncode == 0:
        if not grpc_smoke_base_image_has_openssl(tag):
            print(
                f"Image {tag} lacks openssl; rebuilding grpc-smoke-base (or set VD_REBUILD_GRPC_SMOKE_BASE=1) ...",
                file=sys.stderr,
            )
            rebuild = True
        else:
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


def prepare_dedup_fixture_copies_on_host(fixtures_host: Path) -> None:
    """Create *_copy_dedup.mp4 next to left/right before the server container starts."""
    left = fixtures_host / "left.mp4"
    right = fixtures_host / "right.mp4"
    if not left.is_file() or not right.is_file():
        die(f"prepare_dedup_fixture_copies_on_host: missing {left} or {right}")
    lc = fixtures_host / "left_copy_dedup.mp4"
    rc = fixtures_host / "right_copy_dedup.mp4"
    shutil.copy2(left, lc)
    shutil.copy2(right, rc)
    print(
        f"E2E: wrote host dedup fixture copies ({lc.name}, {rc.name}) before server start.",
        flush=True,
    )


def cleanup_dedup_fixture_copies_on_host(fixtures_host: Path) -> None:
    for name in ("left_copy_dedup.mp4", "right_copy_dedup.mp4"):
        p = fixtures_host / name
        if not p.is_file():
            continue
        try:
            p.unlink()
        except OSError as e:
            print(f"warning: could not remove {p}: {e}", file=sys.stderr)


def main() -> None:
    p = argparse.ArgumentParser(
        description="Docker gRPC deep smoke (.deb + VideoDedupGrpcSmoke; optional comparison + dedup smokes with fixtures).",
    )
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

    fixtures_host = ROOT / "packaging/tests/fixtures/grpc-smoke"
    fixtures_server = "/tmp/vd-fixtures/grpc-smoke"
    left_mp4 = fixtures_host / "left.mp4"
    right_mp4 = fixtures_host / "right.mp4"
    have_fixtures = left_mp4.is_file() and right_mp4.is_file()

    comp_abs: Path | None = None
    dedup_abs: Path | None = None
    if have_fixtures:
        print(f"Publishing VideoDedupGrpcComparisonSmoke to {ROOT / 'packaging/out' / arch / 'e2e-comparison-smoke'} ...")
        publish_smoke_project(
            "VideoDedupGrpcComparisonSmoke/VideoDedupGrpcComparisonSmoke.csproj",
            "e2e-comparison-smoke",
            arch,
            rid,
        )
        comp_abs = (ROOT / "packaging/out" / arch / "e2e-comparison-smoke").resolve()
        print(f"Publishing VideoDedupGrpcDedupSmoke to {ROOT / 'packaging/out' / arch / 'e2e-dedup-smoke'} ...")
        publish_smoke_project("VideoDedupGrpcDedupSmoke/VideoDedupGrpcDedupSmoke.csproj", "e2e-dedup-smoke", arch, rid)
        dedup_abs = (ROOT / "packaging/out" / arch / "e2e-dedup-smoke").resolve()
    else:
        print(
            f"Note: grpc-smoke fixtures not found under {fixtures_host}; comparison and dedup smokes skipped.",
            file=sys.stderr,
        )

    entry = ROOT / "packaging/tests/e2e/server-entrypoint.sh"
    entry_vol = docker_host_path(entry)
    pkg_vol = docker_host_path(pkg_abs)
    smoke_vol = docker_host_path(smoke_abs)

    compare_left = ""
    compare_right = ""

    mounts = [
        "-v",
        f"{entry_vol}:/entrypoint.sh:ro",
        "-v",
        f"{pkg_vol}:/tmp/videodedupserver.deb:ro",
    ]
    if have_fixtures:
        mounts += ["-v", f"{docker_host_path(fixtures_host)}:{fixtures_server}:rw"]
        compare_left = f"{fixtures_server}/left.mp4"
        compare_right = f"{fixtures_server}/right.mp4"
        print(f"Using gRPC fixtures at {fixtures_server} (in container).")

    if have_fixtures:
        prepare_dedup_fixture_copies_on_host(fixtures_host)

    try:
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

        cert_host_dir = ROOT / "packaging/out" / arch / "e2e-server-cert"
        try:
            pinned_cert = extract_server_cert(container, cert_host_dir, package_format="deb")
        except RuntimeError as ex:
            docker_logs(container)
            die(str(ex), 1)
        print(f"E2E: extracted server TLS cert to {pinned_cert}", flush=True)

        grpc_url = f"https://127.0.0.1:{port}"
        cert_vol = docker_host_path(pinned_cert.parent)
        pinned_in_container = "/smoke-cert/VideoDedup.crt"
        pinned_on_host = docker_host_path(pinned_cert)

        def log_compare_env(name: str) -> None:
            print(f"E2E [{name}] VIDEODEDUP_SMOKE_COMPARE_LEFT={compare_left or '<unset>'}", file=sys.stderr)
            print(f"E2E [{name}] VIDEODEDUP_SMOKE_COMPARE_RIGHT={compare_right or '<unset>'}", file=sys.stderr)

        use_host_dotnet = bool(which("dotnet")) and (
            os.environ.get("VD_SMOKE_USE_HOST_DOTNET", "").strip() == "1"
            or os.environ.get("VD_SMOKE_IN_DOCKER", "").strip() == "0"
        )

        def run_grpc_smoke_code(url: str) -> int:
            log_compare_env("VideoDedupGrpcSmoke")
            env = os.environ.copy()
            env["VIDEODEDUP_SMOKE_COMPARE_LEFT"] = compare_left
            env["VIDEODEDUP_SMOKE_COMPARE_RIGHT"] = compare_right
            env["VIDEODEDUP_SMOKE_PINNED_CERT"] = pinned_on_host
            if use_host_dotnet:
                smoke_dll = docker_host_path(smoke_abs / "VideoDedupGrpcSmoke.dll")
                r = subprocess.run(["dotnet", smoke_dll, url], env=env)
                return r.returncode
            print(
                "Running VideoDedupGrpcSmoke in mcr.microsoft.com/dotnet/runtime:8.0 ...",
                file=sys.stderr,
            )
            smoke_url = url.replace("127.0.0.1", "host.docker.internal").replace("localhost", "host.docker.internal")
            env_docker = os.environ.copy()
            if os.name == "nt":
                env_docker.setdefault("MSYS2_ARG_CONV_EXCL", "*")
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
                    "-e",
                    f"VIDEODEDUP_SMOKE_PINNED_CERT={pinned_in_container}",
                    "-v",
                    f"{smoke_vol}:/smoke:ro",
                    "-v",
                    f"{cert_vol}:/smoke-cert:ro",
                    "mcr.microsoft.com/dotnet/runtime:8.0",
                    "dotnet",
                    "/smoke/VideoDedupGrpcSmoke.dll",
                    smoke_url,
                ],
                env=env_docker,
            )
            return r.returncode

        def run_comparison_smoke_code(url: str, comparison_abs: Path) -> int:
            log_compare_env("VideoDedupGrpcComparisonSmoke")
            env = os.environ.copy()
            env["VIDEODEDUP_SMOKE_COMPARE_LEFT"] = compare_left
            env["VIDEODEDUP_SMOKE_COMPARE_RIGHT"] = compare_right
            env["VIDEODEDUP_SMOKE_PINNED_CERT"] = pinned_on_host
            if use_host_dotnet:
                dll = docker_host_path(comparison_abs / "VideoDedupGrpcComparisonSmoke.dll")
                r = subprocess.run(["dotnet", dll, url], env=env)
                return r.returncode
            print(
                "Running VideoDedupGrpcComparisonSmoke in mcr.microsoft.com/dotnet/runtime:8.0 ...",
                file=sys.stderr,
            )
            smoke_url = url.replace("127.0.0.1", "host.docker.internal").replace("localhost", "host.docker.internal")
            env_docker = os.environ.copy()
            if os.name == "nt":
                env_docker.setdefault("MSYS2_ARG_CONV_EXCL", "*")
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
                    "-e",
                    f"VIDEODEDUP_SMOKE_PINNED_CERT={pinned_in_container}",
                    "-v",
                    f"{docker_host_path(comparison_abs.resolve())}:/comparison-smoke:ro",
                    "-v",
                    f"{cert_vol}:/smoke-cert:ro",
                    "mcr.microsoft.com/dotnet/runtime:8.0",
                    "dotnet",
                    "/comparison-smoke/VideoDedupGrpcComparisonSmoke.dll",
                    smoke_url,
                ],
                env=env_docker,
            )
            return r.returncode

        def run_dedup_smoke_code(url: str, fixture_dir: str, dedup_root: Path) -> int:
            print(f"E2E [VideoDedupGrpcDedupSmoke] VIDEODEDUP_SMOKE_FIXTURE_DIR={fixture_dir}", file=sys.stderr)
            env = os.environ.copy()
            env["VIDEODEDUP_SMOKE_FIXTURE_DIR"] = fixture_dir
            env["VIDEODEDUP_SMOKE_PINNED_CERT"] = pinned_on_host
            if use_host_dotnet:
                dll = docker_host_path(dedup_root / "VideoDedupGrpcDedupSmoke.dll")
                r = subprocess.run(["dotnet", dll, url], env=env)
                return r.returncode
            print(
                "Running VideoDedupGrpcDedupSmoke in mcr.microsoft.com/dotnet/runtime:8.0 ...",
                file=sys.stderr,
            )
            dedup_v = docker_host_path(dedup_root.resolve())
            smoke_url = url.replace("127.0.0.1", "host.docker.internal").replace("localhost", "host.docker.internal")
            env_docker = os.environ.copy()
            env_docker["VIDEODEDUP_SMOKE_FIXTURE_DIR"] = fixture_dir
            if os.name == "nt":
                env_docker.setdefault("MSYS2_ARG_CONV_EXCL", "*")
            r = subprocess.run(
                [
                    "docker",
                    "run",
                    "--rm",
                    "--add-host=host.docker.internal:host-gateway",
                    "-e",
                    f"VIDEODEDUP_SMOKE_FIXTURE_DIR={fixture_dir}",
                    "-e",
                    f"VIDEODEDUP_SMOKE_PINNED_CERT={pinned_in_container}",
                    "-v",
                    f"{dedup_v}:/dedup-smoke:ro",
                    "-v",
                    f"{cert_vol}:/smoke-cert:ro",
                    "mcr.microsoft.com/dotnet/runtime:8.0",
                    "dotnet",
                    "/dedup-smoke/VideoDedupGrpcDedupSmoke.dll",
                    smoke_url,
                ],
                env=env_docker,
            )
            return r.returncode

        if compare_left and compare_right and comp_abs is not None and dedup_abs is not None:
            print(f"Running VideoDedupGrpcSmoke + VideoDedupGrpcComparisonSmoke in parallel ({grpc_url}) ...", flush=True)
            failures: list[tuple[str, int]] = []
            with ThreadPoolExecutor(max_workers=2) as ex:
                f_grpc = ex.submit(run_grpc_smoke_code, grpc_url)
                f_comp = ex.submit(run_comparison_smoke_code, grpc_url, comp_abs)
                code_g = f_grpc.result()
                code_c = f_comp.result()
            if code_g != 0:
                failures.append(("VideoDedupGrpcSmoke", code_g))
            if code_c != 0:
                failures.append(("VideoDedupGrpcComparisonSmoke", code_c))
            if failures:
                for label, code in failures:
                    print(f"FAILED: {label} exit={code}", file=sys.stderr, flush=True)
                print("--- server container logs ---", file=sys.stderr)
                docker_logs(container)
                die("parallel gRPC smoke failed", 1)

            print(f"Running VideoDedupGrpcDedupSmoke ({grpc_url}) ...", flush=True)
            code_d = run_dedup_smoke_code(grpc_url, fixtures_server, dedup_abs)
            if code_d != 0:
                print("--- server container logs (dedup smoke failed) ---", file=sys.stderr)
                docker_logs(container)
                die(f"VideoDedupGrpcDedupSmoke exited with code {code_d}", code_d)
        else:
            print(f"Running gRPC smoke only against {grpc_url} ...", flush=True)
            code = run_grpc_smoke_code(grpc_url)
            if code != 0:
                print("--- server container logs ---", file=sys.stderr)
                docker_logs(container)
                die(f"VideoDedupGrpcSmoke exited with code {code}", code)

        print(f"OK: Docker gRPC smoke passed ({server_image}, {pkg_abs}).")
    finally:
        if have_fixtures:
            cleanup_dedup_fixture_copies_on_host(fixtures_host)


if __name__ == "__main__":
    main()
