# Local Linux packaging

## Full build + tests in Docker (recommended on Windows)

Uses **Docker only** (no WSL): Ubuntu in a container installs .NET 8, `flatpak` / `flatpak-builder` + Flathub runtimes, and the Docker CLI (for nested images). The **snap** is built in a second, nested **`ghcr.io/canonical/snapcraft:8_core24`** container (plain Ubuntu is not systemd PID 1, so `snap install snapcraft` cannot run there). Your repo is bind-mounted at `/src`; the host Docker socket is used for Arch `makepkg`, the snapcraft image, and install smoke tests.

From the repository root (Git Bash, Linux shell, or `bash` on macOS):

```bash
chmod +x packaging/tools/run-full-linux-build-docker.sh packaging/tools/run-full-linux-build-docker-inner.sh
./packaging/tools/run-full-linux-build-docker.sh --arch amd64
```

Options: `--image ubuntu:24.04`. Env: `DOCKER_IMAGE`, `ARCH`, optional `SNAPCRAFT_IMAGE`. The outer run uses **`--privileged --network host --pid=host`** so nested **flatpak-builder** (bubblewrap) and the snapcraft image can run; omitting these often breaks Flatpak builds on Docker Desktop.

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

After building packages and staging (`./packaging/tools/stage.sh`), run [tests/e2e/docker-grpc-firewall.sh](../../tests/e2e/docker-grpc-firewall.sh):

```bash
./packaging/tests/e2e/docker-grpc-firewall.sh --arch amd64 --distro debian --format deb --firewall nft
./packaging/tests/e2e/docker-grpc-firewall.sh --arch amd64 --distro ubuntu --format deb --firewall ufw
./packaging/tests/e2e/docker-grpc-firewall.sh --arch amd64 --distro fedora --format rpm --firewall firewalld
./packaging/tests/e2e/docker-grpc-firewall.sh --arch amd64 --distro rocky --format rpm --firewall iptables
./packaging/tests/e2e/docker-grpc-firewall.sh --arch amd64 --distro opensuse --format rpm --firewall nft
./packaging/tests/e2e/docker-grpc-firewall.sh --arch amd64 --distro arch --format staged --firewall iptables
./packaging/tests/e2e/docker-grpc-firewall.sh --arch amd64 --distro manjaro --format staged --firewall nft
./packaging/tests/e2e/docker-grpc-firewall.sh --arch amd64 --distro arch --format pacman --firewall iptables
./packaging/tests/e2e/docker-grpc-firewall.sh --arch amd64 --distro ubuntu --format snap --firewall nft
./packaging/tests/e2e/docker-grpc-firewall.sh --arch amd64 --distro fedora --format flatpak --firewall nft
```

`--firewall` is one of `nft`, `iptables`, `ufw`, `firewalld`. `--format staged` uses `packaging/.stage/<arch>/server/` (no `.deb`/`.rpm`). **`pacman`** needs a built `.pkg.tar.zst` under `packaging/out/<arch>/pacman/`. **`snap`** mounts a `.snap` and tests the payload via **unsquashfs** inside the container (Docker-friendly). **`flatpak`** needs a `.flatpak` bundle and uses `flatpak run` on Fedora. openSUSE RPM runs try `zypper` first and fall back to the staged tree if the Fedora-built RPM cannot be installed.

CI runs a sparse matrix of these combinations in `.github/workflows/linux-packaging.yml` (job `e2e-grpc-firewall`).
