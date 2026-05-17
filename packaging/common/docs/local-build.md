# Local Linux packaging

## Full build + tests in Docker (recommended on Windows)

Uses **Docker only** (no WSL): Ubuntu in a container installs .NET 8 and the Docker CLI (for nested images). **Flatpak** is **skipped by default** here because `flatpak-builder` often fails on Docker Desktop and similar setups; GitHub Actions still builds and tests Flatpak. Pass **`--include-flatpak`** or set **`VD_INCLUDE_FLATPAK=1`** to match CI. When enabled, the container also installs `flatpak` / `flatpak-builder` and Flathub runtimes. The **snap** is built in a second, nested **`ghcr.io/canonical/snapcraft:8_core24`** container (plain Ubuntu is not systemd PID 1, so `snap install snapcraft` cannot run there). Your repo is bind-mounted at `/src`; the host Docker socket is used for Arch `makepkg`, the snapcraft image, and install smoke tests.

From the repository root (Git Bash, Linux shell, or `bash` on macOS):

```bash
chmod +x packaging/tools/run-full-linux-build-docker.sh packaging/tools/run-full-linux-build-docker-inner.sh
./packaging/tools/run-full-linux-build-docker.sh --arch amd64
```

Options: `--image ubuntu:24.04`, **`--include-flatpak`**. Env: `DOCKER_IMAGE`, `ARCH`, optional `SNAPCRAFT_IMAGE`, optional **`VD_INCLUDE_FLATPAK=1`** (same effect as `--include-flatpak`). The outer run uses **`--privileged --network host --pid=host`** so nested **flatpak-builder** (bubblewrap, when enabled) and the snapcraft image can run; omitting these often breaks Flatpak builds on Docker Desktop.

**Git Bash (Windows):** the script sets **`MSYS2_ARG_CONV_EXCL='*'`** so MSYS does not rewrite **`/var/run/docker.sock`**, **`-w /src`**, or **`bash -c '…/src/…'`** into **`C:\Program Files\Git\...`** (which makes Docker fail with “Access is denied” or “working directory … is invalid”). The repo bind mount uses **`cygpath -w`** for a Windows path Docker Desktop accepts.

**CRLF:** packaging **`*.sh`** must use **LF** (see [.gitattributes](../../../.gitattributes)). If you see **`set: pipefail: invalid option name`**, run `git add --renormalize packaging` (or convert line endings) and retry. The outer script also pipes the inner script through **`sed 's/\r$//'`** so a stray CRLF inner file still runs.

---

Run on Linux (or WSL2 with Docker optional for RPM/Pacman). From the repository root:

```bash
chmod +x packaging/tools/*.sh packaging/common/generate-metadata.sh
./packaging/tools/stage.sh --arch amd64
./packaging/tools/build-deb.sh --arch amd64
```

Build everything that your host can satisfy:

```bash
./packaging/tools/build-all.sh --arch amd64 --formats deb,rpm,pacman,snap,flatpak
```

**Pacman** (`.pkg.tar.zst`): on Arch, install `base-devel` and run `./packaging/tools/build-pacman.sh --arch amd64`. Elsewhere the script uses **Docker** (`archlinux` image) to run `makepkg`.

**Snap**: install [`snapcraft`](https://snapcraft.io/docs/snapcraft-overview) (e.g. `snap install snapcraft --classic`). CI passes `--require-snapcraft` so missing `snapcraft` fails the job; locally `build-snap.sh` skips if not installed.

**Flatpak**: install `flatpak`, `flatpak-builder`, and Flathub **Platform**, **Sdk**, and **dotnet8** SDK extension (see `packaging/tools/build-flatpak.sh`). CI uses `--require-flatpak-builder`.

## Metadata

Edit static fields in [project.meta.json](../project.meta.json). Version and changelog lines are generated from git by `generate-metadata.sh`.

## Outputs

- Staged publish: `packaging/.stage/<arch>/server/`
- Metadata: `packaging/out/metadata.json`
- Packages: `packaging/out/<arch>/deb/`, `rpm/`, `pacman/` (`.pkg.tar.zst`), `snap/`, `flatpak/`

## Build and test scripts (orchestration)

This is a **call map** for the main entrypoints. Thin `*.sh` wrappers under `packaging/tests/` often `exec` the Python listed next to them so Git Bash / POSIX can invoke one command.

### CI and full local pipeline

| Entrypoint | Purpose | Main callees |
|------------|---------|----------------|
| [`packaging/ci/linux_build_and_test.py`](../../ci/linux_build_and_test.py) | **Linux packaging job** (mirrors [linux-packaging.yml](../../../.github/workflows/linux-packaging.yml) job `linux-packages`). Phases: `packages` → `lint` → `smoke`. | **`packages`:** `packaging/tools/stage.py`, then `build-deb.sh`, `build-rpm.sh`, `build-pacman.sh`, `build-flatpak.sh` (`--require-flatpak-builder`), `build-snap.sh` (`--require-snapcraft`), `write-checksums.sh` (all via bash). **`lint`:** `packaging/tests/run_package_tests.py`. **`smoke`:** `packaging/ci/docker_firewall_run_all.py --integration`, then `docker_install_smoke.py` for `deb`, `rpm`, `pacman`, `flatpak`. |
| [linux-packaging.yml](../../../.github/workflows/linux-packaging.yml) | Workflow: `linux-packages` runs `linux_build_and_test.py`; **`e2e-grpc-firewall`** restores artifacts and runs **`docker_grpc_firewall.py`** once per **matrix** row (parallel jobs). **`e2e-grpc-deep-smoke`** restores the same artifact and runs **`docker_grpc_deep_smoke.py`** once per arch (.deb in container: **VideoDedupGrpcSmoke**; when LFS fixtures exist: parallel **VideoDedupGrpcComparisonSmoke** + sequential **VideoDedupGrpcDedupSmoke** after preparing `*_copy_dedup.mp4` on the host before the fixture bind mount). | As above + matrix of [`docker_grpc_firewall.py`](../../tests/e2e/docker_grpc_firewall.py) + [`docker_grpc_deep_smoke.py`](../../tests/e2e/docker_grpc_deep_smoke.py). |
| [`packaging/tests/run_all_linux_host.py`](../../tests/run_all_linux_host.py) | Same **idea** as smoke + lint for developers on Windows (no full `linux_build_and_test` package phase). Expects packages already built. | `run_package_tests.py` → `docker_firewall_run_all.py --integration` → `docker_install_smoke.py deb` / `rpm` → **`docker_grpc_deep_smoke.py`** → **`docker_grpc_firewall.py`** for each fixed distro/format/firewall row (matches CI `e2e-grpc-firewall` matrix; no snap/flatpak rows). **`-j N`:** concurrent firewall E2E only (default `1` = sequential). **Git LFS** on `packaging/tests/fixtures/grpc-smoke/*.mp4` enables comparison + dedup deep smokes. |

On GitHub Actions, the **`linux-packages`** job uploads **separate artifacts** per format (`linux-deb-*`, `linux-rpm-*`, `linux-pacman-*`, `linux-snap-*`, `linux-flatpak-*`, `linux-metadata-*`). E2E jobs download them with `merge-multiple: false` (one subdirectory per artifact) and reassemble `packaging/out/` via `packaging/ci/restore_linux_package_artifacts.sh`. **`e2e-grpc-firewall`** uses a **matrix**, so each combination is a **separate job** scheduled **in parallel** (subject to org concurrency). Locally, **`python3 packaging/tests/run_all_linux_host.py -j 8`** (example) overlaps the **firewall** matrix after a **single** `VideoDedupGrpcSmoke` publish so workers do not race on `packaging/out/<arch>/e2e-smoke/`.

### Full build inside Docker (outer + inner)

[`packaging/tools/run-full-linux-build-docker.sh`](../../tools/run-full-linux-build-docker.sh) starts an Ubuntu worker with the repo mounted; [`run-full-linux-build-docker-inner.sh`](../../tools/run-full-linux-build-docker-inner.sh) runs inside it:

- `stage.sh`, `build-deb.sh`, `build-rpm.sh`, `build-pacman.sh`, optional `build-flatpak.sh`, snap via nested **snapcraft** image, `write-checksums.sh`
- `python3 packaging/tests/run_package_tests.py`
- `./packaging/tests/firewall/docker-run-all.sh --integration` → **`packaging/ci/docker_firewall_run_all.py`**
- `./packaging/tests/install/docker-install-*.sh` → **`packaging/ci/docker_install_smoke.py`** (`deb`, `rpm`, `pacman`, optional `flatpak`)

The inner script does **not** run `docker_grpc_firewall.py` or `docker_grpc_deep_smoke.py`; use **`run_all_linux_host.py`** or CI for those. **`run_all_linux_host`** runs deep smoke **before** the firewall matrix.

### Individual drivers (detail)

- **`run_package_tests.py`** — Package lint when tools exist: `packaging/tests/firewall/validate_static.py`; optional `lintian` on latest `.deb`, `rpmlint` on latest `.rpm`, `review-tools.snap-review` on snaps, `namcap` on pacman packages.
- **`docker_firewall_run_all.py`** — Ports `docker-run-all.sh`: for each integration scenario, `docker run … bash -s` with stdin scripts that exercise shipped firewall helpers under `packaging/common/firewall/` (mounted read-only). See [tests/firewall/README.md](../../tests/firewall/README.md).
- **`docker_install_smoke.py`** — Ports `docker-install-*.sh`: picks latest artifact under `packaging/out/<arch>/…`, bind-mounts repo + package, installs in a throwaway container and sanity-checks the service layout/start.
- **`docker_grpc_firewall.py`** — End-to-end: creates a dual-stack user bridge, runs a **server** container with [`server-entrypoint.sh`](../../tests/e2e/server-entrypoint.sh) (install package, firewall, start `VideoDedupService`), publishes and runs **`VideoDedupGrpcSmoke`** over IPv4 and IPv6. Env and options: script docstring / `--help`.
- **`docker_grpc_deep_smoke.py`** — Single **deb** container with [`server-entrypoint.sh`](../../tests/e2e/server-entrypoint.sh): install generates TLS (`VideoDedup.crt`); smokes use **HTTPS** with `VIDEODEDUP_SMOKE_PINNED_CERT`. When **`packaging/tests/fixtures/grpc-smoke/`** has both mp4s (Git LFS), copies them on the host to `*_copy_dedup.mp4` before starting the container, bind-mounts that dir read-write (so **VideoDedupGrpcDedupSmoke** can delete a resolved duplicate), runs **VideoDedupGrpcComparisonSmoke** in parallel with GrpcSmoke, then **VideoDedupGrpcDedupSmoke**, then removes the host copies. Invoked by **`run_all_linux_host.py`** and CI **`e2e-grpc-deep-smoke`**.

## Reproducible builds

Export a fixed timestamp before packaging:

```bash
export SOURCE_DATE_EPOCH="$(git log -1 --format=%ct)"
```

## Roles

- **DEB/RPM**: host systemd service as user `videodedup`.
- **Snap**: `daemon: simple` under Snap (often `classic` confinement for broad folder access).
- **Flatpak**: user-run binary only; not a system daemon (see manifest comments).

## Firewall

The `.deb` / `.rpm` install docs and helper scripts under `/usr/share/doc/videodedupserver/README.firewall` and `/usr/lib/videodedupserver/firewall/` (see [firewall/README.firewall](../firewall/README.firewall) in the source tree). Packages do not open TCP 51726 automatically.

Automated checks (static + optional Docker integration): [tests/firewall/README.md](../../tests/firewall/README.md).

`.deb` install smoke test (apt + short server run in Docker): [tests/install/README.md](../../tests/install/README.md).

## gRPC E2E (Docker, multi-distro / firewall)

The E2E script creates a **dual-stack** user-defined bridge and runs the gRPC smoke client against the server’s explicit **IPv4** and **IPv6** addresses. Your Docker daemon must support IPv6 on custom networks (otherwise `docker network create --ipv6 …` fails or the server gets no `GlobalIPv6Address`). See the [Docker IPv6 documentation](https://docs.docker.com/engine/daemon/ipv6/) (e.g. `"ipv6": true` and a `fixed-cidr-v6` in `daemon.json` on Docker Desktop or Linux).

After building packages and staging (`./packaging/tools/stage.sh`), run the gRPC firewall E2E driver ([`docker_grpc_firewall.py`](../../tests/e2e/docker_grpc_firewall.py)) from the repo root (or the thin shim `./packaging/tests/e2e/docker-grpc-firewall.sh`, which `exec`s the same script):

```bash
python3 packaging/tests/e2e/docker_grpc_firewall.py --arch amd64 --distro debian --format deb --firewall nft
python3 packaging/tests/e2e/docker_grpc_firewall.py --arch amd64 --distro ubuntu --format deb --firewall ufw
python3 packaging/tests/e2e/docker_grpc_firewall.py --arch amd64 --distro fedora --format rpm --firewall firewalld
python3 packaging/tests/e2e/docker_grpc_firewall.py --arch amd64 --distro rocky --format rpm --firewall iptables
python3 packaging/tests/e2e/docker_grpc_firewall.py --arch amd64 --distro opensuse --format rpm --firewall nft
python3 packaging/tests/e2e/docker_grpc_firewall.py --arch amd64 --distro arch --format staged --firewall iptables
python3 packaging/tests/e2e/docker_grpc_firewall.py --arch amd64 --distro manjaro --format staged --firewall nft
python3 packaging/tests/e2e/docker_grpc_firewall.py --arch amd64 --distro arch --format pacman --firewall iptables
python3 packaging/tests/e2e/docker_grpc_firewall.py --arch amd64 --distro ubuntu --format snap --firewall nft
python3 packaging/tests/e2e/docker_grpc_firewall.py --arch amd64 --distro fedora --format flatpak --firewall nft
```

`--firewall` is one of `nft`, `iptables`, `ufw`, `firewalld`. `--format staged` uses `packaging/.stage/<arch>/server/` (no `.deb`/`.rpm`). **`pacman`** needs a built `.pkg.tar.zst` under `packaging/out/<arch>/pacman/`. **`snap`** mounts a `.snap` and tests the payload via **unsquashfs** inside the container (Docker-friendly). **`flatpak`** needs a `.flatpak` bundle and uses `flatpak run` on Fedora. openSUSE RPM runs try `zypper` first and fall back to the staged tree if the Fedora-built RPM cannot be installed.

CI runs a sparse matrix of these combinations in `.github/workflows/linux-packaging.yml` (job `e2e-grpc-firewall`). Job **`e2e-grpc-deep-smoke`** runs [`docker_grpc_deep_smoke.py`](../../tests/e2e/docker_grpc_deep_smoke.py) against the built `.deb` (Git LFS checkout for full comparison + dedup coverage).
