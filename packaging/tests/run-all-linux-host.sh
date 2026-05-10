#!/usr/bin/env bash
# Run Linux packaging tests from a host with Docker + dotnet (e.g. Windows Git Bash).
# Expects: packaging already staged/built for amd64.
set -euo pipefail
ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
cd "${ROOT}"
ARCH="${1:-amd64}"

chmod +x packaging/tools/*.sh packaging/common/generate-metadata.sh packaging/tests/*.sh \
  packaging/tests/firewall/*.sh packaging/tests/install/*.sh packaging/tests/e2e/*.sh 2>/dev/null || true

echo "=== run-package-tests.sh ==="
./packaging/tests/run-package-tests.sh "${ARCH}"

echo "=== firewall docker-run-all.sh --integration ==="
./packaging/tests/firewall/docker-run-all.sh --integration

echo "=== docker-install-deb.sh ==="
./packaging/tests/install/docker-install-deb.sh --arch "${ARCH}"

echo "=== docker-install-rpm.sh ==="
./packaging/tests/install/docker-install-rpm.sh --arch "${ARCH}"

rows=(
  "debian deb nft"
  "debian deb iptables"
  "ubuntu deb ufw"
  "fedora rpm nft"
  "fedora rpm firewalld"
  "rocky rpm iptables"
  "opensuse rpm nft"
  "arch staged iptables"
  "manjaro staged nft"
)
for row in "${rows[@]}"; do
  # shellcheck disable=SC2086
  set -- ${row}
  d="$1"; f="$2"; w="$3"
  echo "=== e2e: ${d} ${f} ${w} ==="
  ./packaging/tests/e2e/docker-grpc-firewall.sh --arch "${ARCH}" --distro "${d}" --format "${f}" --firewall "${w}"
done

echo "ALL_LINUX_TESTS_OK"
