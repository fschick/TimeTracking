# Ensure unsigned powershell script execution is allowed: Set-ExecutionPolicy -ExecutionPolicy RemoteSigned

param (
  [Parameter()][String]$version = "0.0.0"
)

. $PSScriptRoot/_core.ps1

Push-Location $PSScriptRoot/../..

# Configure
if (!$version) {
	$version = git describe --tags
}

$filter = ""
if ($env:GITHUB_ACTION) {
	$filter = "TestCategory!=DatabaseRequired"
}

# Run tests
Npm-Restore -folder FS.TimeTracking.UI.Angular
Test-Project -project FS.TimeTracking/FS.TimeTracking.sln -version $version -filter $filter
Test-Project -project FS.TimeTracking.Tool/FS.TimeTracking.Tool.sln -version $version -filter $filter
Test-Ui -project FS.TimeTracking.UI.Angular

Pop-Location


