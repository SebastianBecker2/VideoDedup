# Local Linux packaging

Run on Linux (or WSL2 with Docker optional for RPM). From the repository root:

```bash
chmod +x packaging/tools/*.sh packaging/common/generate-metadata.sh
./packaging/tools/stage.sh --arch amd64
./packaging/tools/build-deb.sh --arch amd64
```

Build everything that your host can satisfy:

```bash
./packaging/tools/build-all.sh --arch amd64 --formats deb
```

## Metadata

Edit static fields in [project.meta.json](../project.meta.json). Version and changelog lines are generated from git by `generate-metadata.sh`.

## Outputs

- Staged publish: `packaging/.stage/<arch>/server/`
- Metadata: `packaging/out/metadata.json`
- Packages: `packaging/out/<arch>/deb/`, `rpm/`, `snap/`, `flatpak/`

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
```

`--firewall` is one of `nft`, `iptables`, `ufw`, `firewalld`. `--format staged` uses `packaging/.stage/<arch>/server/` (no `.deb`/`.rpm`). openSUSE RPM runs try `zypper` first and fall back to the staged tree if the Fedora-built RPM cannot be installed.

CI runs a sparse matrix of these combinations in `.github/workflows/linux-packaging.yml` (job `e2e-grpc-firewall`).
