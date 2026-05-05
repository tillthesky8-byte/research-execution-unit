#!/usr/bin/env zsh

# Usage: tools/commit-tool.sh [-a] [-n|--dry-run]
#   -a : stage all changes before committing
#   -n|--dry-run : show the generated commit message without committing

set -euo pipefail

DRY_RUN=0
STAGE_ALL=0

while [[ $# -gt 0 ]]; do
  case "$1" in
    -n|--dry-run) DRY_RUN=1; shift ;;
    -a) STAGE_ALL=1; shift ;;
    -h|--help)
      sed -n '1,120p' "$0"
      exit 0
      ;;
    *) echo "Unknown arg: $1"; exit 1 ;;
  esac
done

if [ $STAGE_ALL -eq 1 ]; then
  git add -A
fi

STATUS=$(git status --porcelain)
if [ -z "$STATUS" ]; then
  echo "No changes to commit." >&2
  exit 0
fi

map_scope() {
  local path="$1"
  if [[ "$path" == src/REU.App/* ]]; then
    echo "app"
  elif [[ "$path" == src/REU.Pipeline/* ]]; then
    echo "pipeline"
  elif [[ "$path" == src/REU.Simulator/* ]]; then
    echo "simulator"
  elif [[ "$path" == src/REU.Modules/* ]]; then
    echo "modules"
  elif [[ "$path" == src/REU.Contracts/* ]]; then
    echo "contracts"
  elif [[ "$path" == tests/* ]]; then
    echo "tests"
  elif [[ "$path" == .github/* ]]; then
    echo "ci"
  else
    echo "repo"
  fi
}

typeset -A scopes
new_files=0
deleted_files=0
modified_files=0
renamed_files=0

changed_files=()

while IFS= read -r line; do
  status=${line:0:2}
  file=${line:3}
  case "$status" in
    "A "*) new_files=$((new_files+1)) ;;
    " D"*|"D "*) deleted_files=$((deleted_files+1)) ;;
    "R "*) renamed_files=$((renamed_files+1)) ;;
    * ) modified_files=$((modified_files+1)) ;;
  esac
  # For scope, prefer first few path segments
  scope=$(map_scope "$file")
  scopes[$scope]=1
  changed_files+=("$status $file")
done <<< "$STATUS"

# Decide commit type
if [ $new_files -gt 0 ] && [ $modified_files -eq 0 ] && [ $deleted_files -eq 0 ]; then
  type="feat"
elif [ $deleted_files -gt 0 ] && [ $modified_files -eq 0 ]; then
  type="chore"
elif [ $modified_files -gt 0 ] && [ $new_files -eq 0 ]; then
  type="fix"
else
  type="feat"
fi

# Build scope string
scope_keys=()
for k in "${!scopes[@]}"; do
  scope_keys+=("$k")
done
if [ ${#scope_keys[@]} -eq 1 ]; then
  scope="(${scope_keys[0]})"
else
  scope=""
fi

# Build short summary
count_changed=$(echo "$STATUS" | wc -l | tr -d ' ')
if [ ${#changed_files[@]} -le 3 ]; then
  subj_items=()
  for item in "${changed_files[@]}"; do
    subj_items+=("${item#* }")
  done
  summary=$(IFS=", "; echo "${subj_items[*]}")
  summary=$(echo "$summary" | sed 's#/+#/#g')
  subject="$type$scope: update $summary"
else
  subject="$type$scope: update $count_changed files"
fi

# Build body with file list
body="Changes:\n"
for item in "${changed_files[@]}"; do
  body+="- $item\n"
done

body+="\nGenerated-by: commit-tool (tools/commit-tool.sh)\n"

echo "---\nSubject:\n$subject\n---\n"
echo -e "$body"

if [ $DRY_RUN -eq 1 ]; then
  echo "(dry-run) not committing" >&2
  exit 0
fi

# Commit
git commit -m "$subject" -m "$body"
echo "Committed: $subject"
