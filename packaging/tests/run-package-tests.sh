#!/usr/bin/env sh
# Thin shim: implementation is run_package_tests.py
SCRIPT_DIR="$(CDPATH= cd -- "$(dirname "$0")" && pwd)"
exec python3 "$SCRIPT_DIR/run_package_tests.py" "$@"
