# Ensure unsigned powershell script execution ist allowed: Set-ExecutionPolicy -ExecutionPolicy RemoteSigned

. $PSScriptRoot/make_shared.ps1

Push-Location $PSScriptRoot\..

Npm-Restore
Build-Rest-Services
Test-Rest-Services
Test-Ui

Pop-Location


