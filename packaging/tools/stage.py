#!/usr/bin/env python3
"""
Stage Linux server payload under packaging/.stage/<arch>/server (dotnet publish + metadata).
Python port of packaging/tools/stage.sh — no bash required on the host.
"""

from __future__ import annotations

import argparse
import json
import os
import shutil
import subprocess
import sys
from pathlib import Path


def repo_root_from_here() -> Path:
    return Path(__file__).resolve().parent.parent.parent


def git_epoch(root: Path) -> int:
    r = subprocess.run(
        ["git", "-C", str(root), "log", "-1", "--format=%ct"],
        capture_output=True,
        text=True,
    )
    if r.returncode != 0 or not r.stdout.strip():
        return 0
    try:
        return int(r.stdout.strip())
    except ValueError:
        return 0


def strip_crlf_bytes(data: bytes) -> bytes:
    if b"\r\n" in data:
        return data.replace(b"\r\n", b"\n")
    if b"\r" in data:
        return data.replace(b"\r", b"")
    return data


def strip_crlf_text_files(stage: Path) -> None:
    for pattern in ("*.json", "*.dll.config", "**/*.sh"):
        for f in stage.glob(pattern):
            if not f.is_file():
                continue
            data = f.read_bytes()
            normalized = strip_crlf_bytes(data)
            if normalized != data:
                f.write_bytes(normalized)


def chmod_stage_tree(stage: Path) -> None:
    """Match stage.sh: top-level file modes; skip on Windows."""
    if os.name == "nt":
        return
    for p in stage.iterdir():
        if p.is_dir():
            try:
                p.chmod(0o755)
            except OSError:
                pass
        elif p.is_file():
            try:
                p.chmod(0o755 if p.name == "VideoDedupService" else 0o644)
            except OSError:
                pass


def patch_appsettings_grpc(stage: Path) -> None:
    path = stage / "appsettings.json"
    data = json.loads(path.read_text(encoding="utf-8"))
    grpc = data.setdefault("Kestrel", {}).setdefault("Endpoints", {}).setdefault("gRPC", {})
    grpc["Url"] = "https://[::]:51726"
    grpc["Protocols"] = "Http2"
    grpc["Certificate"] = {"Path": "cert/VideoDedup.pfx", "Password": ""}
    path.write_text(json.dumps(data, indent=2) + "\n", encoding="utf-8")


def main() -> int:
    p = argparse.ArgumentParser(description="Stage VideoDedupService for Linux packaging.")
    p.add_argument("--arch", default="amd64", choices=("amd64", "arm64"))
    args = p.parse_args()

    root = repo_root_from_here()
    arch = args.arch
    rid = "linux-x64" if arch == "amd64" else "linux-arm64"

    if "SOURCE_DATE_EPOCH" not in os.environ or not os.environ["SOURCE_DATE_EPOCH"].strip():
        os.environ["SOURCE_DATE_EPOCH"] = str(git_epoch(root))

    stage = root / "packaging" / ".stage" / arch / "server"
    if stage.exists():
        shutil.rmtree(stage)
    stage.mkdir(parents=True, exist_ok=True)

    csproj = root / "VideoDedupService" / "VideoDedupService.csproj"
    r = subprocess.run(
        [
            "dotnet",
            "publish",
            str(csproj),
            "-c",
            "Release",
            "-r",
            rid,
            "--self-contained",
            "true",
            "-p:PublishSingleFile=true",
            "-o",
            str(stage),
        ],
        cwd=root,
    )
    if r.returncode != 0:
        return r.returncode

    shutil.copy2(root / "VideoDedupService" / "appsettings.json", stage / "appsettings.json")
    dev = root / "VideoDedupService" / "appsettings.Development.json"
    if dev.is_file():
        shutil.copy2(dev, stage / "appsettings.Development.json")

    patch_appsettings_grpc(stage)
    cert_setup = stage / "cert-setup"
    cert_setup.mkdir(parents=True, exist_ok=True)
    for name in ("generate-server-cert.sh", "remove-server-cert.sh", "write-tls-env.sh"):
        src = root / "packaging/common/scripts" / name
        dest = cert_setup / name
        dest.write_bytes(strip_crlf_bytes(src.read_bytes()))
        if os.name != "nt":
            try:
                dest.chmod(0o755)
            except OSError:
                pass
    strip_crlf_text_files(stage)
    chmod_stage_tree(stage)

    gen = root / "packaging" / "tools" / "generate_metadata.py"
    r = subprocess.run([sys.executable, str(gen)], cwd=root)
    if r.returncode != 0:
        return r.returncode

    print(f"Staged server payload: {stage}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
