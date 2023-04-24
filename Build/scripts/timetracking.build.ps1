# Ensure unsigned powershell script execution ist allowed: Set-ExecutionPolicy -ExecutionPolicy RemoteSigned

param (
  [Parameter()][String]$version = "0.0.0"
)

. $PSScriptRoot/_core.ps1

Push-Location $PSScriptRoot/../..

# Configure
if (!$version) {
	$version = git describe --tags
}

# Build
Npm-Restore -folder FS.TimeTracking.UI.Angular
Build-Project -project FS.TimeTracking/FS.TimeTracking -version $version
Build-Project -project FS.TimeTracking.Tool/FS.TimeTracking.Tool -version $version
Build-Ui -project FS.TimeTracking.UI.Angular

Pop-Location