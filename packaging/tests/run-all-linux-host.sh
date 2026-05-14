#!/usr/bin/env sh
# Thin shim: implementation is run_all_linux_host.py
SCRIPT_DIR="$(CDPATH= cd -- "$(dirname "$0")" && pwd)"
exec python3 "$SCRIPT_DIR/run_all_linux_host.py" "$@"
