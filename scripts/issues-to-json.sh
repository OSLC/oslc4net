#!/usr/bin/env bash

set -eEuo pipefail
# set -x

trap 'caller 1' ERR

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
OUT_DIR="${SCRIPT_DIR}/../tmp"
mkdir -p "$OUT_DIR"

echo "Exporting open issues to JSON..."

gh issue list -R OSLC/oslc4net --state open --limit 1000 --json number,title,body,url,createdAt,updatedAt |
  jq 'map({number, title, description: .body, url, created_at: .createdAt, updated_at: .updatedAt})' \
    >"${OUT_DIR}/open-issues.json"

echo -e "\n✅ Done"
