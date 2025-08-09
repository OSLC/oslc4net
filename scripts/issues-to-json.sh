#!/usr/bin/env bash

set -eEuo pipefail
# set -x

trap 'caller 1' ERR

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
OUT_DIR="${SCRIPT_DIR}/../tmp"
mkdir -p "$OUT_DIR"

echo "Exporting open issues to JSON..."

command -v gh >/dev/null 2>&1 || { echo "gh CLI is required"; exit 127; }
command -v jq >/dev/null 2>&1 || { echo "jq is required"; exit 127; }
# Ensure authenticated (public repo should work anonymously, but fail fast if misconfigured)
if ! gh auth status >/dev/null 2>&1; then
  echo "Warning: 'gh' not authenticated; proceeding (public repo)."
fi

TMP_FILE="${OUT_DIR}/open-issues.json.tmp"
gh issue list -R OSLC/oslc4net --state open --limit 1000 --json number,title,body,url,createdAt,updatedAt |
  jq 'map({number, title, description: .body, url, created_at: .createdAt, updated_at: .UpdatedAt // .updatedAt})' \
    >"$TMP_FILE"
mv -f "$TMP_FILE" "${OUT_DIR}/open-issues.json"


echo -e "\n✅ Done"
