# Firewall helper scripts — automated checks

Scripts live in `packaging/common/firewall/` and are shipped under
`/usr/lib/videodedupserver/firewall/` in the `.deb`.

## Static validation (no Docker)

From repository root on Linux or WSL:

```bash
chmod +x packaging/tests/firewall/*.sh
python3 packaging/tests/firewall/validate_static.py
# or: ./packaging/tests/firewall/validate-static.sh  (thin shim)
```

Checks: `sh -n` on each script, no CRLF, README present.

## Docker integration tests

Requires **Docker** with the Linux daemon running (Docker Desktop on Windows, or
`docker` on Linux). Uses **`--privileged`** so netfilter-style tools can load
rules inside containers.

```bash
./packaging/tests/firewall/docker-run-all.sh --integration
```

This runs, in order:

| Check | Image | What it proves |
|-------|--------|----------------|
| nftables | `debian:bookworm-slim` | Live rule, then `--persist` writes `/etc/nftables.conf` containing `51726` (on-disk persistence of ruleset dump; not a reboot) |
| iptables | `debian:bookworm-slim` | Live rule, then `--persist` with `iptables-persistent` — `/etc/iptables/rules.v4` contains `51726` |
| ufw | `ubuntu:22.04` | `open-port-ufw.sh` leaves rules in `user.rules` (ufw’s normal persistent store) |
| firewalld | `fedora:40` | `firewall-offline-cmd` permanent port list (not the full `firewall-cmd` daemon path) |

None of these simulate a **full reboot**; they assert that the **saved config files** (or ufw’s rules file) contain the expected port after the persist path where applicable.

The **firewalld** script itself calls `firewall-cmd` with a **running** daemon;
the Docker step validates **offline** permanent semantics and that the script
still references the expected commands. On a real Fedora/RHEL host, run the
shipped script after `systemctl start firewalld`.

## CI

`Linux packaging` workflow can call `docker-run-all.sh --integration` on
`ubuntu-latest` (Docker is preinstalled). This is optional if integration
runtime is too slow; static checks are cheap and run locally without Docker.

## WSL

Same commands from a WSL shell at the repo path (`/mnt/.../VideoDedup`). If
Docker Desktop integration is broken, run integration tests from a Linux VM or
GitHub Actions instead.
