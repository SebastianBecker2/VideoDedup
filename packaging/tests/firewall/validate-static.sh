#!/usr/bin/env sh
# Thin shim: implementation is validate_static.py
SCRIPT_DIR="$(CDPATH= cd -- "$(dirname "$0")" && pwd)"
exec python3 "$SCRIPT_DIR/validate_static.py" "$@"
