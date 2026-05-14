#!/usr/bin/env sh
# Thin shim: implementation is packaging/tools/generate_metadata.py
SCRIPT_DIR="$(CDPATH= cd -- "$(dirname "$0")" && pwd)"
exec python3 "$SCRIPT_DIR/../tools/generate_metadata.py" "$@"
