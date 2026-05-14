"""Resolve repo root for packaging/tests host-side Python scripts (any depth under packaging/tests)."""

from __future__ import annotations

from pathlib import Path


def repo_root_from_script(script_file: Path) -> Path:
    """Walk up until packaging/tools/stage.sh exists (VideoDedup repo root)."""
    d = script_file.resolve().parent
    while d != d.parent:
        if (d / "packaging" / "tools" / "stage.sh").is_file() or (d / "packaging" / "tools" / "stage.py").is_file():
            return d
        d = d.parent
    raise RuntimeError(f"Could not find repo root above {script_file}")
