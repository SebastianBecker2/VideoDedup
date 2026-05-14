#!/usr/bin/env python3
"""
Emit packaging/out/metadata.json from project.meta.json + git.
Python port of packaging/common/generate-metadata.sh (same output).
"""

from __future__ import annotations

import json
import subprocess
from datetime import datetime, timezone
from pathlib import Path


def repo_root_from_here() -> Path:
    # packaging/tools/generate_metadata.py -> repo root
    return Path(__file__).resolve().parent.parent.parent


def run_git(root: Path, *args: str) -> subprocess.CompletedProcess[str]:
    return subprocess.run(
        ["git", "-C", str(root), *args],
        capture_output=True,
        text=True,
    )


def debian_changelog_line(epoch: int, version: str, git_sha: str) -> str:
    dt = datetime.fromtimestamp(epoch, tz=timezone.utc)
    date_s = dt.strftime("%a, %d %b %Y %H:%M:%S +0000")
    return f"{date_s}  {version}  Automated entry from git {git_sha}\n".rstrip("\n")


def main() -> int:
    root = repo_root_from_here()
    out_dir = root / "packaging" / "out"
    out_dir.mkdir(parents=True, exist_ok=True)

    r = run_git(root, "rev-parse", "--short", "HEAD")
    git_sha = r.stdout.strip() if r.returncode == 0 else "unknown"

    r = run_git(root, "describe", "--tags", "--always", "--dirty")
    git_tag = r.stdout.strip() if r.returncode == 0 else ""
    if not git_tag:
        git_tag = f"0.0.0+{git_sha}"

    version = git_tag.removeprefix("v")
    if not version or not version[0].isdigit():
        version = f"0.0.0+{version}"

    r = run_git(root, "log", "-1", "--format=%ct")
    try:
        source_date_epoch = int(r.stdout.strip()) if r.returncode == 0 and r.stdout.strip() else 0
    except ValueError:
        source_date_epoch = 0

    changelog_line = debian_changelog_line(source_date_epoch, version, git_sha)

    static_path = root / "packaging" / "common" / "project.meta.json"
    out_path = out_dir / "metadata.json"
    data = json.loads(static_path.read_text(encoding="utf-8"))
    data["version"] = version
    data["git_sha"] = git_sha
    data["git_tag"] = git_tag
    data["source_date_epoch"] = source_date_epoch
    data["changelog_debian"] = changelog_line
    out_path.write_text(json.dumps(data, indent=2, ensure_ascii=False) + "\n", encoding="utf-8")
    print(f"Wrote {out_path}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
