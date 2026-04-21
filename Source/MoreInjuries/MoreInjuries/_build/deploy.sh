#!/usr/bin/env bash
set -euo pipefail
shopt -s nullglob

# IMPORTANT: change to Release for stable deployments
configuration="Release"
project_name="MoreInjuries"
game_version="1.6"

# conditional compilation flags (mapped to: -p:<flag>=enable)
mod_feature_flags=( "ModBadHygiene" )

# Resolve script directory (where this script lives)
script_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
# Resolve mod root (4 levels up from script directory)
mod_root="$(cd "${script_dir}/../../../.." && pwd)"
project_path="${script_dir}/../${project_name}.csproj"

log_message() {
  local msg="$1"
  printf '[%s build %s]: %s\n' \
    "$project_name" \
    "$(date '+%Y-%m-%d %H:%M:%S')" \
    "$msg"
}

# Read the hostconfig file (located next to this script)
log_message "Reading host configuration file..."
steam_root="$(jq -r '.steam_root' "${script_dir}/hostconfig.json")"
if [[ "$steam_root" == "G:/SteamLibrary" ]]
then
  echo "Error: You must update the 'steam_root' value in hostconfig.json."
  exit 1
fi
upload_dir="${steam_root}/steamapps/common/RimWorld/RimWorldMac.app/Mods/${project_name}"

# Build mod flag properties
mod_flag_properties=()
for flag in "${mod_feature_flags[@]:-}"; do
  mod_flag_properties+=( "-p:${flag}=enable" )
done

# build and publish the project
log_message "Building and publishing ${project_name} v${game_version}..."
dotnet clean "${project_path}"
dotnet restore "${project_path}" --no-cache
dotnet build "${project_path}" -c "${configuration}" "${mod_flag_properties[@]}"
dotnet publish "${project_path}" -c "${configuration}" -p:PublishProfile="${configuration}" "${mod_flag_properties[@]}"

# clean upload dir
log_message "Cleaning up the upload directory..."
if [[ -e "${upload_dir}" ]]; then
  rm -rf -- "${upload_dir}"
fi

# create new folder structure
mkdir -p "${upload_dir}"
mkdir -p "${upload_dir}/Source"

# Write commit.ref (origin + current commit)
log_message "Writing commit.ref..."
commit_ref="${upload_dir}/Source/commit.ref"

origin_url=""
commit_sha=""

if command -v git >/dev/null 2>&1 && git -C "${mod_root}" rev-parse --is-inside-work-tree >/dev/null 2>&1; then
  origin_url="$(git -C "${mod_root}" remote get-url origin 2>/dev/null || true)"
  commit_sha="$(git -C "${mod_root}" rev-parse HEAD 2>/dev/null || true)"
fi

origin_https="${origin_url}"

# git@github.com:Owner/Repo(.git)
if [[ "${origin_https}" =~ ^git@github\.com: ]]; then
  origin_https="$(printf '%s' "${origin_https}" | sed -E 's|^git@github\.com:|https://github.com/|')"
fi

# ssh://git@github.com/Owner/Repo(.git)
if [[ "${origin_https}" =~ ^ssh://git@github\.com/ ]]; then
  origin_https="$(printf '%s' "${origin_https}" | sed -E 's|^ssh://git@github\.com/|https://github.com/|')"
fi

# strip trailing .git
origin_https="$(printf '%s' "${origin_https}" | sed -E 's|\.git$||')"

{
  echo "origin=${origin_url}"
  echo "origin_https=${origin_https}"
  echo "commit=${commit_sha}"
  if [[ -n "${origin_https}" && -n "${commit_sha}" ]]; then
    echo "commit_url=${origin_https}/commit/${commit_sha}"
  fi
} > "${commit_ref}"


# add oldversions:
old_versions_dir="${mod_root}/oldversions"
if [[ -d "${old_versions_dir}" ]]; then
  log_message "Adding old versions from ${old_versions_dir}..."

  for version_dir in "${old_versions_dir}"/*/; do
    [[ -d "${version_dir}" ]] || continue

    version_dir="${version_dir%/}"
    version_name="$(basename "${version_dir}")"
    log_message "Processing previous version: ${version_name} ..."

    ref_files=( "${version_dir}"/*.ref )
    if (( ${#ref_files[@]} == 0 )); then
      continue
    fi

    ref_file="${ref_files[0]}"
    raw_version="$(basename "${ref_file}")"
    raw_version="${raw_version%.ref}"

    # Read link (trim common whitespace/newlines)
    download_link="$(sed -e 's/^[[:space:]]\+//' -e 's/[[:space:]]\+$//' "${ref_file}")"
    [[ -n "${download_link}" ]] || continue

    tmpdir="$(mktemp -d)"
    tmpzip="${tmpdir}/${version_name}.zip"
    tmpextract="${tmpdir}/${version_name}_extract"

    # Download (curl preferred)
    if command -v /curl >/dev/null 2>&1; then
      curl -fL --retry 3 --retry-delay 1 -o "${tmpzip}" "${download_link}"
    elif command -v wget >/dev/null 2>&1; then
      wget -O "${tmpzip}" "${download_link}"
    else
      echo "ERROR: neither curl nor wget is available" >&2
      exit 1
    fi

    mkdir -p -- "${tmpextract}"
    unzip -q -o "${tmpzip}" -d "${tmpextract}"

    src_old="${tmpextract}/${project_name}/${version_name}"
    if [[ -d "${src_old}" ]]; then
      cp -a -- "${src_old}" "${upload_dir}/"
      log_message "Added old version: ${raw_version} (${version_name})"
    fi

    rm -rf -- "${tmpdir}"
  done
fi

# create folder for current version
mkdir -p -- "${upload_dir}/${game_version}/Assemblies"

# copy assemblies
dll_src="${mod_root}/Source/${project_name}/artifacts/publish/${project_name}/${configuration}/${project_name}.dll"
if [[ ! -f "${dll_src}" ]]; then
  echo "ERROR: expected build output not found: ${dll_src}" >&2
  exit 1
fi
cp -a -- "${dll_src}" "${upload_dir}/${game_version}/Assemblies/"

# copy  content folders into latest version
for d in Patches Defs Sounds Textures Languages; do
  if [[ -d "${mod_root}/${d}" ]]; then
    cp -a -- "${mod_root}/${d}" "${upload_dir}/${game_version}/"
  else
    log_message "No ${d} folder found in mod root; aborting..."
    exit 1
  fi
done

# copy About into upload root
cp -a -- "${mod_root}/About" "${upload_dir}/"

# copy README because why not
if [[ -f "${mod_root}/README.md" ]]; then
  cp -a -- "${mod_root}/README.md" "${upload_dir}/"
fi

# include docs/wiki
mkdir -p -- "${upload_dir}/docs"
if [[ -d "${mod_root}/docs/wiki" ]]; then
  cp -a -- "${mod_root}/docs/wiki" "${upload_dir}/docs/"
fi

# include LoadFolders.xml
if [[ -f "${mod_root}/LoadFolders.xml" ]]; then
  cp -a -- "${mod_root}/LoadFolders.xml" "${upload_dir}/"
fi

log_message "========== Deployment succeeded =========="
