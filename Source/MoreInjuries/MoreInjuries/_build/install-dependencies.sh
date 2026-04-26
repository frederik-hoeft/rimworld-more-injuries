#!/usr/bin/env bash

# Abort on any error, unset variable, or failed pipeline
set -euo pipefail

# Resolve script directory. Use cd + pwd -P (POSIX, no realpath dependency)
# so this works on macOS where realpath is either missing or lives at
# /opt/homebrew/bin/realpath rather than /usr/bin/realpath.
script_dir="$(cd -- "$(dirname "${BASH_SOURCE[0]}")" && pwd -P)"
# Resolve project root (4 levels up from script directory)
mod_root="$(cd -- "${script_dir}/../../../.." && pwd -P)"

# Read steam_root from hostconfig.json
steam_root="$(jq -r '.steam_root' "${script_dir}/hostconfig.json")"

deps_json="${script_dir}/dependencies.json"
dest_dir="${script_dir}/dependencies"

# Expand {vars} placeholders using values passed in env to sed
expand_placeholders() {
  local s="$1"
  # NOTE: assumes no literal '|' in paths; typical filesystem paths are fine.
  # Bare `sed` rather than /usr/bin/sed so PATH-overridden GNU sed (gsed
  # symlink, brew) is honoured on macOS where the system sed is BSD.
  printf '%s' "$s" | sed -e "s|{steam_root}|${steam_root}|g" \
                         -e "s|{workshop_root}|${workshop_root}|g" \
                         -e "s|{name}|${name}|g"
}

# Load and expand workshop_root template
workshop_root_tpl="$(jq -r '.workshop_root' "$deps_json")"
# For workshop_root expansion, name isn't meaningful, but keep it defined
name=""
workshop_root="$(printf '%s' "$workshop_root_tpl" | sed -e "s|{steam_root}|${steam_root}|g")"

# Ensure destination directory exists
mkdir -p -- "$dest_dir"

# Iterate dependencies
count="$(jq -r '.dependencies | length' "$deps_json")"
for ((i=0; i<count; i++)); do
  name="$(jq -r ".dependencies[$i].name" "$deps_json")"
  path_tpl="$(jq -r ".dependencies[$i].path" "$deps_json")"

  src_path="$(expand_placeholders "$path_tpl")"
  dest_path="${dest_dir}/${name}"

  cp -- "$src_path" "$dest_path"
done

echo "Dependencies copied to dependencies folder"
