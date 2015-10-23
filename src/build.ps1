[CmdletBinding()]
Param(
    [string] $solutionDir = ${env:SolutionDir},
    [string] $projectPath = ${env:ProjectPath},
    [string] $targetDir = ${env:TargetDir},
    [string] $config = ${env:ConfigurationName}
)

# build "$(SolutionDir)." "$(ProjectPath)" "$(TargetDir)."
# powershell "$(SolutionDir)build.ps1" "$(SolutionDir)." "$(ProjectPath)" "$(TargetDir)." -verbose

Set-Location $solutionDir
$nugetPath = "packages\" +  (Get-ChildItem -Path packages -Recurse -Name nuget.exe)
if (!(Test-Path $nugetPath)) {
  Write-Warning "could not find nuget.exe"
  return $null
  }

$nuget = (Resolve-Path $nugetPath).Path
$dest = 'c:\LocalNuGet'
Write-Host "Testing for LocalNuGet"
if (Test-Path $dest) {
  Write-Host "using LocalNuGet"
  } else {
  $dest = $targetDir
  Write-Host "using target $dest"
  }

& "$nuget" pack "$projectPath" -Properties Configuration=$config -OutputDirectory "$dest"  -Verbosity detailed

