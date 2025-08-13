#!/usr/bin/env bash

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
executable="${script_dir}/artifacts/CreateIndex.exe"

# check if the build artifacts exist (or the --compile flag is set)
# if we need to compile, build the solution to script_dir/artifacts
if [[ ! -d "${script_dir}/artifacts" || $compile ]]; then
  # clean existing artifacts
  rm -rf "${script_dir}/artifacts/*"
  solution="${script_dir}/CreateIndex/CreateIndex.sln"
  dotnet publish "${solution}" --configuration Release --output "${script_dir}/artifacts" --runtime "${runtime}"
  # re-create the gitkeep file ...
  touch "${script_dir}/artifacts/.gitkeep"
fi

# now run the generator on the content directory, forward the --clean flag if set
if [[ $clean ]]; then
  "${executable}" --root "docs/content" --src "${script_dir}/../content" --clean
else
  "${executable}" --root "docs/content" --src "${script_dir}/../content"
fi