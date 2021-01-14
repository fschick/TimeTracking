# Ensure unsigned powershell script execution ist allowed: Set-ExecutionPolicy -ExecutionPolicy RemoteSigned

param (
  [Parameter(Mandatory=$true)][String]$projectName,
  [Parameter(Mandatory=$true)][String]$version,
  [Parameter(Mandatory=$true)][String]$publshFolder,
  [Parameter(Mandatory=$true)][String]$runtime
)

. $PSScriptRoot/make_shared.ps1

Push-Location $PSScriptRoot/..

$fileVersion=$version -replace '(\d+(?:\.\d+)*)(.*)', '$1'
$targetFramework="net5.0"
$configuration="Release"
$msBuildPublishDir="$projectName/bin/$configuration/$targetFramework/$runtime/publish"
$msBuildOutDir="$projectName/bin/$configuration/$targetFramework"

Npm-Restore
Publish-Back-End
Publish-Front-End
Move-To-Publish-Folder

Pop-Location


function Publish-Back-End {
    # Publish back-end
	& dotnet publish $projectName --configuration $configuration --framework $targetFramework --self-contained --runtime $runtime -p:Version=$version -p:FileVersion=$fileVersion
	if(!$?) {
		exit $LASTEXITCODE
	}

	# Clean existing artifacts publish folder
	Remove-Item $publshFolder -Recurse -ErrorAction Ignore -Force

	if (Test-Path $msBuildPublishDir/$projectName.config.Development.json){
		Remove-Item $msBuildPublishDir/$projectName.config.Development.json
	}
}

function Publish-Front-End {
	Build-Angular

	# Move SPA to publish folder
	mv FS.TimeTracking.UI.Angular/dist/TimeTracking $msBuildPublishDir/UI
}

function Move-To-Publish-Folder {
	# Move application to publish folder
	if ($runtime.StartsWith("win")) {
		mv $msBuildPublishDir $publshFolder
		cp $PSScriptRoot/service_windows.install.bat $publshFolder/$projectName.WindowsService.Install.bat
		cp $PSScriptRoot/service_windows.uninstall.bat $publshFolder/$projectName.WindowsService.Uninstall.bat
	} else {
		New-Item $publshFolder/opt/$projectName -ItemType Directory
		New-Item $publshFolder/etc/systemd/system -ItemType Directory
		mv $msBuildPublishDir $publshFolder/opt/$projectName/bin
		cp $PSScriptRoot/service_linux.service $publshFolder/etc/systemd/system/$projectName.service
	}
}