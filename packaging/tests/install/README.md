# `.deb` install smoke test (Docker)

Installs the built **`videodedupserver`** package inside a **Debian** container, checks
layout, and runs **`VideoDedupService`** briefly as user **`videodedup`** (no systemd in
the container; `postinst` `systemctl` calls are expected to no-op).

## Requirements

- Docker (Linux engine). From **WSL2** at the repo: paths under `/mnt/...` work.
- A **`.deb`** already built, e.g. `packaging/out/amd64/deb/*.deb`.

## Usage

From repository root:

```bash
chmod +x packaging/tests/install/*.sh

# Newest amd64 .deb under packaging/out/
./packaging/tests/install/docker-install-deb.sh

# Explicit package
./packaging/tests/install/docker-install-deb.sh /path/to/videodedupserver_*_amd64.deb

# arm64 output directory (if you build for arm64)
./packaging/tests/install/docker-install-deb.sh --arch arm64
```

## CI

The **Linux packaging** workflow runs this after the `.deb` is produced so regressions in
`control`, `postinst`, or payload layout fail the job.
