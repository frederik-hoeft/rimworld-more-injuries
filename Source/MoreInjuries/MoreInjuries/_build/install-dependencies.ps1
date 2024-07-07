# Ensure abort after errors are encountered (may happen because of BOMs)
Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$mod_root = (Get-Item -LiteralPath "${PSScriptRoot}/../../../..").FullName

# Read the hostconfig file
$config = Get-Content -LiteralPath "${PSScriptRoot}/hostconfig.json" -Raw | ConvertFrom-Json

# copy dependencies to dependencies folder
# CombatAI (CAI-5000), optional runtime dependency, required for build
Copy-Item -LiteralPath "$($config.steam_root)/steamapps/workshop/content/294100/2938891185/1.5/Assemblies/CombatAI.dll" -Destination "${PSScriptRoot}/dependencies/CombatAI.dll"

Write-Host "Dependencies copied to dependencies folder"