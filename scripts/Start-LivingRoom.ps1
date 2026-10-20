param([string]$DatabasePath, [string]$GoogleCredentialsPath, [ValidateSet('Debug','Release')][string]$Configuration = 'Release')
$ErrorActionPreference = 'Stop'
$projectRoot = Split-Path $PSScriptRoot -Parent
$hostExe = Join-Path $projectRoot "Host\bin\$Configuration\Host.exe"
$clientExe = Join-Path $projectRoot "ofirManaFamily\bin\$Configuration\ofirManaFamily.exe"
if (!(Test-Path $hostExe) -or !(Test-Path $clientExe)) { throw 'Build first with scripts/Build-LivingRoom.ps1.' }
if (!$DatabasePath) {
    $localFolder = Join-Path $projectRoot '.local'
    New-Item -ItemType Directory -Path $localFolder -Force | Out-Null
    $DatabasePath = Join-Path $localFolder 'LivingRoom.accdb'
    if (!(Test-Path -LiteralPath $DatabasePath)) {
        Copy-Item -LiteralPath (Join-Path $projectRoot 'viewModel\DATA\FamilyProjectOfir.accdb') -Destination $DatabasePath
    }
}
$env:LIVINGROOM_DATABASE_PATH = (Resolve-Path -LiteralPath $DatabasePath).Path
if ($GoogleCredentialsPath) { $env:GOOGLE_APPLICATION_CREDENTIALS = (Resolve-Path -LiteralPath $GoogleCredentialsPath).Path }
try {
    $existingSignal = [System.Threading.Mutex]::OpenExisting('Local\LivingRoomHostReady')
    $existingSignal.Dispose()
    throw 'LivingRoom is already running. Close the previous host first.'
} catch [System.Threading.WaitHandleCannotBeOpenedException] { }
# Both are interactive windows. The host must remain open while using the client.
$hostProcess = Start-Process -FilePath $hostExe -WorkingDirectory (Split-Path $hostExe) -PassThru
$ready = $false
for ($attempt=0; $attempt -lt 40; $attempt++) {
    if ($hostProcess.HasExited) { throw 'The host exited. Check its error message before trying again.' }
    try {
        $readySignal = [System.Threading.Mutex]::OpenExisting('Local\LivingRoomHostReady')
        $readySignal.Dispose()
        $ready = $true
        break
    } catch [System.Threading.WaitHandleCannotBeOpenedException] { }
    Start-Sleep -Milliseconds 250
}
if (!$ready) { throw 'The host did not become ready. Check its window before trying again.' }
Start-Process -FilePath $clientExe -WorkingDirectory (Split-Path $clientExe)
Write-Host 'LivingRoom started. Close the client and host windows when finished.'
