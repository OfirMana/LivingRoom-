param([ValidateSet('Debug','Release')][string]$Configuration = 'Release')
$ErrorActionPreference = 'Stop'
$projectRoot = Split-Path $PSScriptRoot -Parent
$runtimeFolder = Join-Path $projectRoot '.local\test-runtime'
New-Item -ItemType Directory -Path $runtimeFolder -Force | Out-Null
Copy-Item (Join-Path $projectRoot "Host\bin\$Configuration\*.dll") $runtimeFolder -Force
Copy-Item (Join-Path $projectRoot "ofirManaFamily\bin\$Configuration\ofirManaFamily.exe") $runtimeFolder -Force
[xml]$hostConfig = Get-Content (Join-Path $projectRoot "Host\bin\$Configuration\Host.exe.config") -Raw
[xml]$clientConfig = Get-Content (Join-Path $projectRoot "ofirManaFamily\bin\$Configuration\ofirManaFamily.exe.config") -Raw
foreach ($sectionName in @('bindings','client')) {
    $section = $clientConfig.configuration.'system.serviceModel'.$sectionName
    if ($section) { $null = $hostConfig.configuration.'system.serviceModel'.AppendChild($hostConfig.ImportNode($section,$true)) }
}
$hostConfig.Save((Join-Path $runtimeFolder 'IntegrationSmoke.exe.config'))
$compilerPath = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'
Push-Location $runtimeFolder
try {
    & $compilerPath /nologo /platform:x64 /out:IntegrationSmoke.exe /r:Model.dll /r:viewModel.dll /r:WcfServiceLibrary1.dll /r:ofirManaFamily.exe /r:System.ServiceModel.dll /r:System.Runtime.Serialization.dll (Join-Path $projectRoot 'tests\IntegrationSmoke.cs')
    if ($LASTEXITCODE -ne 0) { throw 'Test compilation failed.' }
    & '.\IntegrationSmoke.exe' (Join-Path $projectRoot 'viewModel\DATA\FamilyProjectOfir.accdb')
    if ($LASTEXITCODE -ne 0) { throw 'Integration checks failed.' }
} finally { Pop-Location }
