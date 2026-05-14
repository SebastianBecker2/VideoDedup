#!/usr/bin/env sh
# Thin shim: implementation is docker_grpc_firewall.py (avoids MSYS/Git Bash when invoking docker/dotnet).
SCRIPT_DIR="$(CDPATH= cd -- "$(dirname "$0")" && pwd)"
exec python3 "$SCRIPT_DIR/docker_grpc_firewall.py" "$@"
