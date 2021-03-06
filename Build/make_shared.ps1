# Ensure unsigned powershell script execution ist allowed: Set-ExecutionPolicy -ExecutionPolicy RemoteSigned

#
# Shared
#
function Npm-Restore {
    # Switch to UI project
	Push-Location FS.TimeTracking.UI.Angular
	
	# Restore npm packages
	& npm install --prefer-offline --no-audit
	if(!$?) {
		Pop-Location
		exit $LASTEXITCODE
	}
	
	Pop-Location
}

#
#region Build
#
function Build-Rest-Services([String] $version = "0.0.0", [String] $fileVersion = "0.0.0") {
	# Build back-end
	& dotnet build --configuration Release -p:Version=$version -p:FileVersion=$fileVersion
	if(!$?) {
		exit $LASTEXITCODE
	}
}

function Build-Ui {
    # Switch to UI project
	Push-Location FS.TimeTracking.UI.Angular
	
	& npm run lint
	if(!$?) {
		Pop-Location
		exit $LASTEXITCODE
	}
	
	# Build SPA
	& npm run build-prod
	if(!$?) {
		Pop-Location
		exit $LASTEXITCODE
	}
	
	Pop-Location
}
#endregion

#
#region Tests
#
function Test-Rest-Services {
	Get-ChildItem "TestResult" -Recurse | foreach { $_.Delete($TRUE) }
	& dotnet test --configuration Release --logger:trx --logger:html
	if(!$?) {
		exit $LASTEXITCODE
	}
}

function Test-Ui {
	# Switch to UI project
	Push-Location FS.TimeTracking.UI.Angular
	
	& npm run test-headless
	if(!$?) {
		Pop-Location
		exit $LASTEXITCODE
	}
	
	Pop-Location
}
#endregion

#
#region Publish
#
function Publish-Rest-Services([String] $projectName, [String] $configuration, [String] $targetFramework, [String] $runtime, [String] $version, [String] $fileVersion, [String] $publshFolder, [String] $msBuildPublishDir) {
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

function Publish-Ui([String] $msBuildPublishDir) {
	Build-Ui

	# Move SPA to publish folder
	mv FS.TimeTracking.UI.Angular/dist/TimeTracking $msBuildPublishDir/UI
}

function Publish-Merge-To-Artifact-Folder([String] $projectName, [String] $runtime, [String] $publshFolder, [String] $msBuildPublishDir) {
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
#endregion