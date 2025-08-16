#!/usr/bin/env bash

# strict mode
set -euo pipefail

compile=
clean=

# parse arguments (--compile, --clean flags)
while [[ $# -gt 0 ]]; do
  case $1 in
    --compile)
      compile=1
      shift
      ;;
    --clean)
      clean=1
      shift
      ;;
    *)
      shift
      ;;
  esac
done

script_dir=$(realpath "$(dirname "${BASH_SOURCE[0]}")")

runtime=win-x64
output_dir="${script_dir}/artifacts/bin"
executable="${output_dir}/MoreInjuries.WikiGen.exe"

# check if the build artifacts exist (or the --compile flag is set)
# if we need to compile, build the solution to script_dir/artifacts
if [[ ! -f "${executable}" || $compile ]]; then
  # clean existing artifacts
  if [[ -d "${output_dir}" ]]; then
    rm -r "${output_dir}"
  fi
  project="${script_dir}/../../Source/MoreInjuries/MoreInjuries.WikiGen/MoreInjuries.WikiGen.csproj"
  dotnet publish "${project}" --configuration Release --output "${output_dir}" --runtime "${runtime}"
fi

# now run the generator on the wiki directory, forward the --clean flag if set
if [[ $clean ]]; then
  "${executable}" --root "docs/wiki" --src "${script_dir}/../wiki" --clean
else
  "${executable}" --root "docs/wiki" --src "${script_dir}/../wiki"
fi