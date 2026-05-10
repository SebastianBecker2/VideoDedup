#!/usr/bin/env bash
# Resolve a working Python 3 for packaging scripts. On Windows, "python3" may be a Store stub
# that fails; fall back to the "py -3" launcher.
# shellcheck shell=bash
resolve_packaging_python() {
  if command -v python3 >/dev/null 2>&1 && python3 -c "import sys" 2>/dev/null; then
    PACKAGING_PYTHON=(python3)
    return 0
  fi
  if command -v py >/dev/null 2>&1 && py -3 -c "import sys" 2>/dev/null; then
    PACKAGING_PYTHON=(py -3)
    return 0
  fi
  echo "packaging: need Python 3 (python3 or Windows py -3)" >&2
  return 1
}
