#!/usr/bin/env bash
ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../../.." && pwd)"
exec python3 "${ROOT}/packaging/ci/docker_install_smoke.py" flatpak "$@"
