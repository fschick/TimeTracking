# Ensure unsigned powershell script execution ist allowed: Set-ExecutionPolicy -ExecutionPolicy RemoteSigned

param (
  [Parameter(Mandatory=$true)][String]$version
)

. $PSScriptRoot/make_shared.ps1

Push-Location $PSScriptRoot\..

$fileVersion=$version -replace '(\d+(?:\.\d+)*)(.*)', '$1'

Npm-Restore
Build-Back-End
Build-Front-End

Pop-Location


function Build-Back-End {
	# Build back-end
	& dotnet build --configuration Release -p:Version=$version -p:FileVersion=$fileVersion
	if(!$?) {
		exit $LASTEXITCODE
	}
}

function Build-Front-End {
	& npm run lint
	
	# Build front-end
	Build-Angular
}