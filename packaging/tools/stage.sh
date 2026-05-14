#!/usr/bin/env sh
# Thin shim: implementation is stage.py (no bash required for staging).
SCRIPT_DIR="$(CDPATH= cd -- "$(dirname "$0")" && pwd)"
exec python3 "$SCRIPT_DIR/stage.py" "$@"
