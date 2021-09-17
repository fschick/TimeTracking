# Ensure unsigned powershell script execution ist allowed: Set-ExecutionPolicy -ExecutionPolicy RemoteSigned

param (
  [Parameter(Mandatory=$true)][String]$version
)

. $PSScriptRoot/make_shared.ps1

Push-Location $PSScriptRoot\..

$fileVersion=$version -replace '(\d+(?:\.\d+)*)(.*)', '$1'

Npm-Restore
Build-Rest-Services -version $version -fileVersion $fileVersion
Build-Tool -version $version -fileVersion $fileVersion
Build-Ui

Pop-Location