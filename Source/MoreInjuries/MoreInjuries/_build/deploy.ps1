# Ensure abort after errors are encountered (may happen because of BOMs)
Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

# IMPORTANT: change to Release for stable deployments
$configuration = "Debug"
$project_name = "MoreInjuries"
$game_version = "1.6"
$mod_root = (Get-Item -LiteralPath "${PSScriptRoot}/../../../..").FullName
$project_path = "${PSScriptRoot}/../${project_name}.csproj"

# Read the hostconfig file
$config = Get-Content -LiteralPath "${PSScriptRoot}/hostconfig.json" -Raw | ConvertFrom-Json

# Use the path from the JSON file
$upload_dir = "$($config.steam_root)/steamapps/common/RimWorld/Mods/${project_name}"

# build and publish the project
dotnet clean "${project_path}"
if (!$?) { exit $LASTEXITCODE }
dotnet restore "${project_path}" --no-cache
if (!$?) { exit $LASTEXITCODE }
dotnet build "${project_path}" -c "${configuration}"
if (!$?) { exit $LASTEXITCODE }
dotnet publish "${project_path}" -c "${configuration}" -p:PublishProfile=$configuration
if (!$?) { exit $LASTEXITCODE }

# clean upload dir
if (Test-Path -LiteralPath $upload_dir) {
    Remove-Item -LiteralPath $upload_dir -Recurse
}

function Copy-Folder {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory)]
        [String]$FromPath,

        [Parameter(Mandatory)]
        [String]$ToPath,

        [string[]] $Exclude
    )

    if (Test-Path $FromPath -PathType Container) {
        New-Item $ToPath -ItemType Directory -ErrorAction SilentlyContinue | Out-Null
        Get-ChildItem $FromPath -Force | ForEach-Object {
            # avoid the nested pipeline variable
            $item = $_
            $target_path = Join-Path $ToPath $item.Name
            if (($Exclude | ForEach-Object { $item.Name -like $_ }) -notcontains $true) {
                if (Test-Path $target_path) { Remove-Item $target_path -Recurse -Force }
                Copy-Item $item.FullName $target_path
                Copy-Folder -FromPath $item.FullName $target_path $Exclude
            }
        }
    }
}

# create new folder structure
New-Item -ItemType Directory $upload_dir
# include source files
New-Item -ItemType Directory "${upload_dir}/Source"
# add oldversions:
# 1. enumerate subdirectories in the oldversions directory (e.g. 1.5, 1.6, etc.)
# 2. for each subdirectory, there will be a .ref file that contains a raw download link for the corresponding GitHub (zip) release, download that to a temporary location
# 3. extract the zip file, and copy the corresponding version folder (e.g. MoreInjuries/1.5, MoreInjuries/1.6, etc., matching the subdirectory name) to the upload directory root
# 4. delete the temporary zip file
# enumerate subdirectories in the oldversions directory
$old_versions_dir = "${mod_root}/oldversions"
if (Test-Path -LiteralPath $old_versions_dir) {
    Get-ChildItem -Path $old_versions_dir -Directory | ForEach-Object {
        $version_dir = $_.FullName
        $version_name = $_.Name
        # contains a single .ref file with the raw download link, glob the .ref file
        $ref_file = Get-ChildItem -Path $version_dir -Filter "*.ref" -File | Select-Object -First 1
        # check if the glob matched a file
        if ($ref_file -and $ref_file.Exists) {
            $raw_version = $ref_file.Name -replace '\.ref$', ''
            # read the raw download link from the .ref file
            $download_link = Get-Content -LiteralPath $ref_file.FullName -Raw
            # download the zip file to a temporary location
            $temp_zip = Join-Path $env:TEMP "${version_name}.zip"
            Invoke-WebRequest -Uri $download_link -OutFile $temp_zip
            # extract the zip file to a temporary directory
            $temp_extract_dir = Join-Path $env:TEMP "${version_name}_extract"
            Expand-Archive -Path $temp_zip -DestinationPath $temp_extract_dir -Force
            # copy the version folder to the upload directory root
            Copy-Item -LiteralPath (Join-Path $temp_extract_dir "${project_name}/${version_name}") -Destination $upload_dir -Recurse -Container -Force
            # delete the temporary zip file and extraction directory
            Remove-Item -LiteralPath $temp_zip -Force
            Remove-Item -LiteralPath $temp_extract_dir -Recurse -Force
            Write-Host "Added old version: ${raw_version} (${version_name})"
        }
    }
}

# create folder for current version
New-Item -ItemType Directory "${upload_dir}/${game_version}/Assemblies"
# copy assemblies (external deps should be handled via mod dependencies from the workshop)
Copy-Item -LiteralPath "${mod_root}/artifacts/${project_name}.dll" -Destination "${upload_dir}/${game_version}/Assemblies"
# if there is a Patches directory in the mod root, copy that as well (to the latest version)
if (Test-Path -LiteralPath "${mod_root}/Patches") {
	Copy-Item -LiteralPath "${mod_root}/Patches" -Recurse -Destination "${upload_dir}/${game_version}" -Container
}
# if there is a Defs directory in the mod root, copy that as well (to the latest version)
if (Test-Path -LiteralPath "${mod_root}/Defs") {
	Copy-Item -LiteralPath "${mod_root}/Defs" -Recurse -Destination "${upload_dir}/${game_version}" -Container
}
# if there is a Sounds directory in the mod root, copy that as well (to the latest version)
if (Test-Path -LiteralPath "${mod_root}/Sounds") {
	Copy-Item -LiteralPath "${mod_root}/Sounds" -Recurse -Destination "${upload_dir}/${game_version}" -Container
}
# if there is a Textures directory in the mod root, copy that as well (to the latest version)
if (Test-Path -LiteralPath "${mod_root}/Textures") {
	Copy-Item -LiteralPath "${mod_root}/Textures" -Recurse -Destination "${upload_dir}/${game_version}" -Container
}
# copy Languages directory
if (Test-Path -LiteralPath "${mod_root}/Languages") {
    Copy-Item -LiteralPath "${mod_root}/Languages" -Recurse -Destination "${upload_dir}/${game_version}" -Container
}
# copy About
Copy-Item -LiteralPath "${mod_root}/About" -Recurse -Destination $upload_dir -Container

# copy Source (exclude sensitive/unnecessary items)
Copy-Folder -FromPath "${mod_root}/Source" -ToPath "${upload_dir}/Source" -Exclude ".vs","bin","obj","*.user","TestResults"

# copy README because why not
Copy-Item -LiteralPath "${mod_root}/README.md" -Destination $upload_dir

# include the LoadFolders.xml file
Copy-Item -LiteralPath "${mod_root}/LoadFolders.xml" -Destination $upload_dir

Write-Host "========== Deployment succeeded =========="