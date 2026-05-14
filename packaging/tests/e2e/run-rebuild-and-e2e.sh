#!/usr/bin/env sh
# Thin shim: implementation is run_rebuild_and_e2e.py
SCRIPT_DIR="$(CDPATH= cd -- "$(dirname "$0")" && pwd)"
exec python3 "$SCRIPT_DIR/run_rebuild_and_e2e.py" "$@"
