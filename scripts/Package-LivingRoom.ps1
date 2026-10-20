param(
    [ValidateSet('Debug','Release')][string]$Configuration = 'Release',
    [string]$OutputPath
)
$ErrorActionPreference = 'Stop'
$projectRoot = Split-Path $PSScriptRoot -Parent
if (!$OutputPath) { $OutputPath = Join-Path $projectRoot '.local\LivingRoom-Windows-x64.zip' }
$OutputPath = [System.IO.Path]::GetFullPath($OutputPath)
New-Item -ItemType Directory -Path (Split-Path $OutputPath) -Force | Out-Null
$staging = Join-Path $projectRoot ('.local\package-' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $staging -Force | Out-Null
foreach ($appFolder in @('Host','ofirManaFamily')) {
    $source = Join-Path $projectRoot "$appFolder\bin\$Configuration"
    if (!(Test-Path -LiteralPath (Join-Path $source "$appFolder.exe"))) { throw "Build $Configuration before packaging: $appFolder.exe is missing." }
    $destination = Join-Path $staging "$appFolder\bin\Release"
    foreach ($file in Get-ChildItem -LiteralPath $source -File -Recurse) {
        $relative = $file.FullName.Substring($source.Length).TrimStart('\')
        if ($file.Extension -in @('.pdb','.xml','.accdb','.laccdb') -or $file.Name -match '(?i)GCKEY|credentials|token' -or $relative -like 'Config\*') { continue }
        $target = Join-Path $destination $relative
        New-Item -ItemType Directory -Path (Split-Path $target) -Force | Out-Null
        Copy-Item -LiteralPath $file.FullName -Destination $target
    }
}
foreach ($relative in @('viewModel\DATA\FamilyProjectOfir.accdb','scripts\Start-LivingRoom.ps1','Start-LivingRoom.cmd','README.md','docs\VERIFICATION.md','docs\QUICKSTART.he.md','docs\ENGINEERING.md','docs\PROVENANCE.md')) {
    $target = Join-Path $staging $relative
    New-Item -ItemType Directory -Path (Split-Path $target) -Force | Out-Null
    Copy-Item -LiteralPath (Join-Path $projectRoot $relative) -Destination $target
}
@'
LivingRoom - Windows x64

1. Extract the entire ZIP into a writable folder.
2. Double-click Start-LivingRoom.cmd.
3. Keep the host window open while using the app.
4. Register a family and its first member; select the manager checkbox.

Requires .NET Framework 4.7.2+ and Microsoft ACE 16.0 x64.
The first launch copies a clean demo database into .local.
Original family data and Google credentials are not included.
Google Calendar requires separate configuration; local features work without it.
See README.md for troubleshooting and verification limits.
'@ | Set-Content -LiteralPath (Join-Path $staging 'RUN-ME.txt') -Encoding UTF8
Compress-Archive -Path (Join-Path $staging '*') -DestinationPath $OutputPath -Force
$hash = (Get-FileHash -LiteralPath $OutputPath -Algorithm SHA256).Hash.ToLowerInvariant()
"$hash  $([IO.Path]::GetFileName($OutputPath))" | Set-Content -LiteralPath "$OutputPath.sha256" -Encoding ASCII
Write-Host "Created $OutputPath"
