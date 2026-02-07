#!/usr/bin/env pwsh
Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

# IMPORTANT: change to Release for stable deployments
$configuration = 'Release'
$projectName   = 'MoreInjuries'
$gameVersion   = '1.6'

# conditional compilation flags (mapped to: -p:<flag>=enable)
$modFeatureFlags = @('ModBadHygiene')

# Resolve script directory (where this script lives)
$scriptDir = (Resolve-Path -LiteralPath $PSScriptRoot).Path
# Resolve mod root (4 levels up from script directory)
$modRoot   = (Resolve-Path -LiteralPath (Join-Path $scriptDir '..\..\..\..')).Path
$projectPath = Join-Path (Join-Path $scriptDir '..') "$projectName.csproj"

function Log-Message {
  param([Parameter(Mandatory)][string]$Message)
  $ts = Get-Date -Format 'yyyy-MM-dd HH:mm:ss'
  Write-Host "[$projectName build $ts]: $Message"
}

# Read the hostconfig file (located next to this script)
Log-Message "Reading host configuration file..."
$hostConfigPath = Join-Path $scriptDir 'hostconfig.json'
$hostConfigJson = Get-Content -LiteralPath $hostConfigPath -Raw | ConvertFrom-Json
$steamRoot = [string]$hostConfigJson.steam_root
$uploadDir = Join-Path $steamRoot "steamapps/common/RimWorld/Mods/$projectName"

# Build mod flag properties
$modFlagProperties = foreach ($flag in $modFeatureFlags) { "-p:$flag=enable" }

# build and publish the project
Log-Message "Building and publishing $projectName v$gameVersion..."
dotnet clean   $projectPath
dotnet restore $projectPath --no-cache
dotnet build   $projectPath -c $configuration @modFlagProperties
dotnet publish $projectPath -c $configuration "-p:PublishProfile=$configuration" @modFlagProperties

# clean upload dir
Log-Message "Cleaning up the upload directory..."
if (Test-Path -LiteralPath $uploadDir) {
  Remove-Item -LiteralPath $uploadDir -Recurse -Force
}

# create new folder structure
New-Item -ItemType Directory -Path $uploadDir | Out-Null
New-Item -ItemType Directory -Path (Join-Path $uploadDir 'Source') | Out-Null

# Write commit.ref (origin + current commit)
Log-Message "Writing commit.ref..."
$commitRef = Join-Path $uploadDir 'Source\commit.ref'

$originUrl = ''
$commitSha = ''

$git = Get-Command git -ErrorAction SilentlyContinue
if ($git) {
  $inside = $false
  try {
    $out = & git -C $modRoot rev-parse --is-inside-work-tree 2>$null
    if ($LASTEXITCODE -eq 0 -and $out.Trim() -eq 'true') { $inside = $true }
  } catch { $inside = $false }

  if ($inside) {
    try { $originUrl = (& git -C $modRoot remote get-url origin 2>$null).Trim() } catch { $originUrl = '' }
    try { $commitSha = (& git -C $modRoot rev-parse HEAD 2>$null).Trim() } catch { $commitSha = '' }
  }
}

$originHttps = $originUrl

# git@github.com:Owner/Repo(.git)
if ($originHttps -match '^git@github\.com:') {
  $originHttps = $originHttps -replace '^git@github\.com:', 'https://github.com/'
}

# ssh://git@github.com/Owner/Repo(.git)
if ($originHttps -match '^ssh://git@github\.com/') {
  $originHttps = $originHttps -replace '^ssh://git@github\.com/', 'https://github.com/'
}

# strip trailing .git
$originHttps = $originHttps -replace '\.git$', ''

$lines = New-Object System.Collections.Generic.List[string]
$lines.Add("origin=$originUrl")
$lines.Add("origin_https=$originHttps")
$lines.Add("commit=$commitSha")
if (-not [string]::IsNullOrWhiteSpace($originHttps) -and -not [string]::IsNullOrWhiteSpace($commitSha)) {
  $lines.Add("commit_url=$originHttps/commit/$commitSha")
}
Set-Content -LiteralPath $commitRef -Value $lines -Encoding UTF8

# add oldversions:
$oldVersionsDir = Join-Path $modRoot 'oldversions'
if (Test-Path -LiteralPath $oldVersionsDir -PathType Container) {
  Log-Message "Adding old versions from $oldVersionsDir..."

  foreach ($versionDirItem in Get-ChildItem -LiteralPath $oldVersionsDir -Directory) {
    $versionDir  = $versionDirItem.FullName
    $versionName = $versionDirItem.Name
    Log-Message "Processing previous version: $versionName ..."

    $refFiles = Get-ChildItem -LiteralPath $versionDir -Filter *.ref -File -ErrorAction SilentlyContinue
    if (-not $refFiles -or $refFiles.Count -eq 0) { continue }

    $refFile = $refFiles[0].FullName
    $rawVersion = [IO.Path]::GetFileNameWithoutExtension($refFile)

    # Read link (trim common whitespace/newlines)
    $downloadLink = (Get-Content -LiteralPath $refFile -Raw).Trim()
    if ([string]::IsNullOrWhiteSpace($downloadLink)) { continue }

    $tmpDir = Join-Path ([IO.Path]::GetTempPath()) ([guid]::NewGuid().ToString('N'))
    New-Item -ItemType Directory -Path $tmpDir | Out-Null

    $tmpZip     = Join-Path $tmpDir "$versionName.zip"
    $tmpExtract = Join-Path $tmpDir "${versionName}_extract"
    New-Item -ItemType Directory -Path $tmpExtract | Out-Null

    # Download
    Invoke-WebRequest -Uri $downloadLink -OutFile $tmpZip -MaximumRedirection 20

    # Extract
    Expand-Archive -LiteralPath $tmpZip -DestinationPath $tmpExtract -Force

    $srcOld = Join-Path (Join-Path $tmpExtract $projectName) $versionName
    if (Test-Path -LiteralPath $srcOld -PathType Container) {
      Copy-Item -LiteralPath $srcOld -Destination $uploadDir -Recurse -Force
      Log-Message "Added old version: $rawVersion ($versionName)"
    }

    Remove-Item -LiteralPath $tmpDir -Recurse -Force
  }
}

# create folder for current version
New-Item -ItemType Directory -Path (Join-Path $uploadDir "$gameVersion/Assemblies") -Force | Out-Null

# copy assemblies
$dllSrc = Join-Path $modRoot "Source/$projectName/artifacts/publish/$projectName/$configuration/$projectName.dll"
if (-not (Test-Path -LiteralPath $dllSrc -PathType Leaf)) {
  throw "ERROR: expected build output not found: $dllSrc"
}
Copy-Item -LiteralPath $dllSrc -Destination (Join-Path $uploadDir "$gameVersion/Assemblies/") -Force

# copy content folders into latest version
foreach ($d in @('Patches','Defs','Sounds','Textures','Languages')) {
  $src = Join-Path $modRoot $d
  if (Test-Path -LiteralPath $src -PathType Container) {
    Copy-Item -LiteralPath $src -Destination (Join-Path $uploadDir $gameVersion) -Recurse -Force
  } else {
    Log-Message "No $d folder found in mod root; aborting..."
    exit 1
  }
}

# copy About into upload root
Copy-Item -LiteralPath (Join-Path $modRoot 'About') -Destination $uploadDir -Recurse -Force

# copy README because why not
$readme = Join-Path $modRoot 'README.md'
if (Test-Path -LiteralPath $readme -PathType Leaf) {
  Copy-Item -LiteralPath $readme -Destination $uploadDir -Force
}

# include docs/wiki
New-Item -ItemType Directory -Path (Join-Path $uploadDir 'docs') -Force | Out-Null
$wiki = Join-Path $modRoot 'docs/wiki'
if (Test-Path -LiteralPath $wiki -PathType Container) {
  Copy-Item -LiteralPath $wiki -Destination (Join-Path $uploadDir 'docs') -Recurse -Force
}

# include LoadFolders.xml
$loadFolders = Join-Path $modRoot 'LoadFolders.xml'
if (Test-Path -LiteralPath $loadFolders -PathType Leaf) {
  Copy-Item -LiteralPath $loadFolders -Destination $uploadDir -Force
}

Log-Message "========== Deployment succeeded =========="
