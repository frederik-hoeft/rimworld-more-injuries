#!/usr/bin/env bash

# Abort on any error, unset variable, or failed pipeline
set -euo pipefail

# Resolve script directory
script_dir="$(/usr/bin/realpath "$(/usr/bin/dirname "${BASH_SOURCE[0]}")")"
# Resolve project root (4 levels up from script directory)
mod_root="$(/usr/bin/realpath "${script_dir}/../../../..")"

# Read steam_root from hostconfig.json
steam_root="$(jq -r '.steam_root' "${script_dir}/hostconfig.json")"

deps_json="${script_dir}/dependencies.json"
dest_dir="${script_dir}/dependencies"

# Expand {vars} placeholders using values passed in env to sed
expand_placeholders() {
  local s="$1"
  # GNU sed: replace placeholders with current values
  # NOTE: assumes no literal '|' in paths; typical filesystem paths are fine.
  printf '%s' "$s" | /usr/bin/sed -e "s|{steam_root}|${steam_root}|g" \
                                  -e "s|{workshop_root}|${workshop_root}|g" \
                                  -e "s|{name}|${name}|g"
}

# Load and expand workshop_root template
workshop_root_tpl="$(jq -r '.workshop_root' "$deps_json")"
# For workshop_root expansion, name isn't meaningful, but keep it defined
name=""
workshop_root="$(printf '%s' "$workshop_root_tpl" | /usr/bin/sed -e "s|{steam_root}|${steam_root}|g")"

# Ensure destination directory exists
/usr/bin/mkdir -p "$dest_dir"

# Iterate dependencies
count="$(jq -r '.dependencies | length' "$deps_json")"
for ((i=0; i<count; i++)); do
  name="$(jq -r ".dependencies[$i].name" "$deps_json")"
  path_tpl="$(jq -r ".dependencies[$i].path" "$deps_json")"

  src_path="$(expand_placeholders "$path_tpl")"
  dest_path="${dest_dir}/${name}"

  /bin/cp "$src_path" "$dest_path"
done

echo "Dependencies copied to dependencies folder"