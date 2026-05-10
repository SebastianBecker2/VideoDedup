# Install smoke tests (Docker)

## `.deb` (Debian)

Installs **`videodedupserver`** in a **Debian** container, checks layout, and runs **`VideoDedupService`** briefly as **`videodedup`** (no PID 1 systemd; `postinst` `systemctl` calls may no-op).

**Requirements:** Docker; a built `.deb` under `packaging/out/<arch>/deb/`.

```bash
chmod +x packaging/tests/install/*.sh
./packaging/tests/install/docker-install-deb.sh
./packaging/tests/install/docker-install-deb.sh /path/to/videodedupserver_*_amd64.deb
./packaging/tests/install/docker-install-deb.sh --arch arm64
```

## Pacman (Arch)

Uses **`archlinux`** and **`pacman -U`** on a `.pkg.tar.zst` from `packaging/out/<arch>/pacman/`.

```bash
./packaging/tests/install/docker-install-pacman.sh
./packaging/tests/install/docker-install-pacman.sh --arch arm64
```

## Flatpak (Fedora)

Uses **`fedora:40`**, installs **`org.freedesktop.Platform//24.08`**, then the `.flatpak` bundle from `packaging/out/<arch>/flatpak/`.

```bash
./packaging/tests/install/docker-install-flatpak.sh
```

## RPM

See **`docker-install-rpm.sh`** (Fedora-based smoke test).

## CI

The **Linux packaging** workflow runs **deb**, **rpm**, **pacman**, and **flatpak** install smokes so packaging regressions fail the job.
