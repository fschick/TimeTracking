# Ensure unsigned powershell script execution ist allowed: Set-ExecutionPolicy -ExecutionPolicy RemoteSigned

param (
  [Parameter(Mandatory=$true)][String]$version
)

Push-Location $PSScriptRoot\..

$fileVersion=$version -replace '(\d+(?:\.\d+)*)(.*)', '$1'

# Build solution
& dotnet build --configuration Release -p:Version=$version -p:FileVersion=$fileVersion
if(!$?) {
	exit $LASTEXITCODE
}

Pop-Location