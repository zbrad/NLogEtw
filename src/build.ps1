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
$nuget = Get-Path ("packages\" + (Get-ChildItem -Path packages -Recurse -Name nuget.exe))

$dest = 'c:\LocalNuGet'
if (!(Test-Path $dest)) {
  $dest = $targetDir
}


& "$nuget" pack "$projectPath" -Properties Configuration=$config -OutputDirectory "$dest"  -Verbosity detailed

