# Ensure abort after errors are encountered (may happen because of BOMs)
Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$mod_root = (Get-Item -LiteralPath "${PSScriptRoot}/../../../..").FullName

# Read the hostconfig file
$config = Get-Content -LiteralPath "${PSScriptRoot}/hostconfig.json" -Raw | ConvertFrom-Json
$steam_root = $config.steam_root

# Read dependencies.json (script directory)
$deps_config = Get-Content -LiteralPath "${PSScriptRoot}/dependencies.json" -Raw | ConvertFrom-Json

# Expand workshop_root template
$workshop_root = $deps_config.workshop_root -replace '\{steam_root\}', $steam_root -replace '\\\\', '\'

$dest_dir = "${PSScriptRoot}/dependencies"
New-Item -ItemType Directory -Path $dest_dir -Force | Out-Null

foreach ($dep in $deps_config.dependencies) {
    $name = $dep.name

    $src_path = $dep.path `
        -replace '\{steam_root\}', $steam_root `
        -replace '\{workshop_root\}', $workshop_root `
        -replace '\{name\}', $name `
        -replace '\\\\', '\'

    $dest_path = Join-Path $dest_dir $name

    Copy-Item -LiteralPath $src_path -Destination $dest_path
}

Write-Host "Dependencies copied to dependencies folder"
