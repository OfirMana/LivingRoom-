param(
    [ValidateSet('Debug','Release')][string]$Configuration = 'Release',
    [string]$DotNetPath,
    [string]$ReferenceAssembliesPath
)
$ErrorActionPreference = 'Stop'
$projectRoot = Split-Path $PSScriptRoot -Parent
$vswherePath = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
if ($DotNetPath) {
    if (!$ReferenceAssembliesPath) { throw 'Supply -ReferenceAssembliesPath pointing to a directory containing .NETFramework/v4.7.2.' }
    $frameworkRoot = (Resolve-Path -LiteralPath $ReferenceAssembliesPath).Path + '\'
    & $DotNetPath msbuild (Join-Path $projectRoot 'ofirManaFamily.sln') /t:Rebuild "/p:Configuration=$Configuration" "/p:TargetFrameworkRootPath=$frameworkRoot" /m /v:minimal /nologo
    if ($LASTEXITCODE -ne 0) { throw "Build failed with exit code $LASTEXITCODE" }
    return
}
if (!(Test-Path -LiteralPath $vswherePath)) { throw 'Install Visual Studio Build Tools 2022 with .NET desktop build tools and the .NET Framework 4.7.2 targeting pack, or supply -DotNetPath and -ReferenceAssembliesPath.' }
$msbuildPath = & $vswherePath -latest -products '*' -requires Microsoft.Component.MSBuild -find 'MSBuild\**\Bin\MSBuild.exe' | Select-Object -First 1
if (!$msbuildPath) { throw 'MSBuild was not found. Install the .NET desktop build tools workload, or supply -DotNetPath and -ReferenceAssembliesPath.' }
& $msbuildPath (Join-Path $projectRoot 'ofirManaFamily.sln') /restore /p:RestorePackagesConfig=true "/p:Configuration=$Configuration" /m /v:minimal /nologo
if ($LASTEXITCODE -ne 0) { throw "Build failed with exit code $LASTEXITCODE" }
