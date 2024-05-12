# Ensure unsigned powershell script execution ist allowed: Set-ExecutionPolicy -ExecutionPolicy RemoteSigned

param (
  [Parameter(Mandatory=$true)][String]$version,
  [Parameter(Mandatory=$false)][String]$runtime,
  [Parameter(Mandatory=$false)][String]$publshFolder
)

. $PSScriptRoot/_core.ps1

Push-Location $PSScriptRoot/../..

# Configure
$framework = "net8.0"

# Publish
Npm-Restore -folder FS.TimeTracking.UI.Angular
Build-Project -project FS.TimeTracking/FS.TimeTracking -version $version
Publish-Project -project FS.TimeTracking/FS.TimeTracking -version $version -framework $framework -runtime $runtime -publshFolder $publshFolder
Publish-Ui -project FS.TimeTracking.UI.Angular -publshFolder ../$publshFolder/webui

Pop-Location