# Ensure unsigned powershell script execution ist allowed: Set-ExecutionPolicy -ExecutionPolicy RemoteSigned

param (
  [Parameter(Mandatory=$true)][String]$version,
  [Parameter(Mandatory=$false)][String]$runtime,
  [Parameter(Mandatory=$false)][String]$publshFolder
)

. $PSScriptRoot/_core.ps1

Push-Location $PSScriptRoot/../..

# Configure
$framework = "net7.0"

# Publish
Npm-Restore -folder FS.TimeTracking.UI.Angular
Publish-Project -project FS.TimeTracking.Tool/FS.TimeTracking.Tool -version $version -framework $framework -runtime $runtime -publshFolder $publshFolder
Build-Project -project FS.TimeTracking/FS.TimeTracking -version $version
Publish-Project -project FS.TimeTracking/FS.TimeTracking -version $version -framework $framework -runtime $runtime -publshFolder $publshFolder
Publish-Ui -project FS.TimeTracking.UI.Angular -publshFolder ../$publshFolder/webui
if ($runtime.StartsWith("win")) {
	Copy-Item ./Build/service/windows.service.install.bat $publshFolder/FS.TimeTracking.service.install.bat
	Copy-Item ./Build/service/windows.service.uninstall.bat $publshFolder/FS.TimeTracking.service.uninstall.bat
} else {
	Copy-Item ./Build/service/linux.service.template $publshFolder/FS.TimeTracking.service.template
}

Pop-Location