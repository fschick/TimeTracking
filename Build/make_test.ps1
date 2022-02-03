# Ensure unsigned powershell script execution ist allowed: Set-ExecutionPolicy -ExecutionPolicy RemoteSigned

. $PSScriptRoot/make_shared.ps1

Push-Location $PSScriptRoot\..

Npm-Restore
if ($env:GITHUB_ACTION) {
	Test-Rest-Services -filter TestCategory!=DatabaseRequired
} else {
	Test-Rest-Services
}
Test-Tool
Test-Ui

Pop-Location


