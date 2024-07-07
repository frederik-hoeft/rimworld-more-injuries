# Ensure abort after errors are encountered (may happen because of BOMs)
Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$project_name = "PostOffice"
$game_version = "1.5"
$mod_root = (Get-Item -LiteralPath "${PSScriptRoot}/../../../..").FullName
$project_path = "${PSScriptRoot}/../${project_name}.csproj"

# Read the hostconfig file
$config = Get-Content -LiteralPath "${PSScriptRoot}/hostconfig.json" -Raw | ConvertFrom-Json

# Use the path from the JSON file
$upload_dir = "$($config.steam_root)/steamapps/common/RimWorld/Mods/${project_name}"

dotnet clean "${project_path}"
if (!$?) { exit $LASTEXITCODE }
dotnet restore "${project_path}" --no-cache
if (!$?) { exit $LASTEXITCODE }
dotnet build "${project_path}" -c Release
if (!$?) { exit $LASTEXITCODE }
dotnet publish "${project_path}" -c Release -p:PublishProfile=release
if (!$?) { exit $LASTEXITCODE }

# clean upload dir
if (Test-Path -LiteralPath $upload_dir) {
    Remove-Item -LiteralPath $upload_dir -Recurse
}

# create new folder structure
New-Item -ItemType Directory $upload_dir
# include source files
New-Item -ItemType Directory "${upload_dir}/Source"
# copy everything in the oldversions directory to the new upload directory root
Copy-Item -Path "${mod_root}/oldversions/*" -Recurse -Destination $upload_dir –Container
# create folder for current version
New-Item -ItemType Directory "${upload_dir}/${game_version}/Assemblies"
# copy assemblies (deps should be handled via mod dependencies from the workshop)
Copy-Item -LiteralPath "${mod_root}/artifacts/${project_name}.dll" -Destination "${upload_dir}/${game_version}/Assemblies"
# copy About
Copy-Item -LiteralPath "${mod_root}/About" -Recurse -Destination $upload_dir –Container

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

# copy Source (exclude sensitive/unnecessary items)
Copy-Folder -FromPath "${mod_root}/Source" -ToPath "${upload_dir}/Source" -Exclude ".vs","bin","obj","*.user"

# copy README because why not
Copy-Item -LiteralPath "${mod_root}/README.md" -Destination $upload_dir

Write-Host "========== Deployment succeeded =========="