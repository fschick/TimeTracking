# Ensure unsigned powershell script execution ist allowed: Set-ExecutionPolicy -ExecutionPolicy RemoteSigned

#
# Shared
#
function Npm-Restore {
    # Switch to UI project
	Push-Location FS.TimeTracking.UI/FS.TimeTracking.UI.Angular
	
	# Restore npm packages
	& npm install --prefer-offline --no-audit --silent
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
	& dotnet build FS.TimeTracking.sln -warnaserror --configuration Debug
	& dotnet build FS.TimeTracking.sln -warnaserror --configuration Release -p:Version=$version -p:FileVersion=$fileVersion
	if(!$?) {
		exit $LASTEXITCODE
	}
}

function Build-Tool([String] $version = "0.0.0", [String] $fileVersion = "0.0.0") {
	# Build back-end
	& dotnet build FS.TimeTracking.Tool.sln -warnaserror --configuration Debug
	& dotnet build FS.TimeTracking.Tool.sln -warnaserror --configuration Release -p:Version=$version -p:FileVersion=$fileVersion
	if(!$?) {
		exit $LASTEXITCODE
	}
}

function Build-Ui {
    # Switch to UI project
	Push-Location FS.TimeTracking.UI/FS.TimeTracking.UI.Angular
	
	& npm run lint
	if(!$?) {
		Pop-Location
		exit $LASTEXITCODE
	}
	
	# Build SPA
	& npm run build
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
	& dotnet test FS.TimeTracking.sln --configuration Release --logger:trx --logger:html
	if(!$?) {
		exit $LASTEXITCODE
	}
}

function Test-Tool {
	Get-ChildItem "TestResult" -Recurse | foreach { $_.Delete($TRUE) }
	& dotnet test FS.TimeTracking.Tool.sln --configuration Release --logger:trx --logger:html
	if(!$?) {
		exit $LASTEXITCODE
	}
}

function Test-Ui {
	# Switch to UI project
	Push-Location FS.TimeTracking.UI/FS.TimeTracking.UI.Angular
	
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
function Clean-Folder([String] $folder) {
	Remove-Item $folder -Recurse -ErrorAction Ignore -Force
}

function Publish-Rest-Services([String] $configuration, [String] $targetFramework, [String] $runtime, [String] $version, [String] $fileVersion, [String] $publshFolder, [String] $msBuildPublishDir) {
    # Generate Open API spec, Angular client and validation spec by running runtime independent build
	& dotnet build FS.TimeTracking/FS.TimeTracking/FS.TimeTracking.csproj --configuration $configuration
	if(!$?) {
		exit $LASTEXITCODE
	}
	
	# Publish back-end
	& dotnet publish FS.TimeTracking/FS.TimeTracking/FS.TimeTracking.csproj --configuration $configuration --framework $targetFramework --self-contained --runtime $runtime -p:Version=$version -p:FileVersion=$fileVersion
	if(!$?) {
		exit $LASTEXITCODE
	}

	Remove-Item $msBuildPublishDir/config/FS.TimeTracking.config.Development.json -ErrorAction Ignore
}

function Publish-Tool([String] $configuration, [String] $targetFramework, [String] $runtime, [String] $version, [String] $fileVersion, [String] $msBuildPublishDir) {
    # Publish back-end
	& dotnet publish FS.TimeTracking.Tool/FS.TimeTracking.Tool/FS.TimeTracking.Tool.csproj --configuration $configuration --framework $targetFramework --self-contained --runtime $runtime -p:Version=$version -p:FileVersion=$fileVersion
	if(!$?) {
		exit $LASTEXITCODE
	}

	# Move Tool to publish folder
	mv FS.TimeTracking.Tool/FS.TimeTracking.Tool/bin/$configuration/$targetFramework/$runtime/publish/FS.TimeTracking.Tool $msBuildPublishDir
	mv FS.TimeTracking.Tool/FS.TimeTracking.Tool/bin/$configuration/$targetFramework/$runtime/publish/FS.TimeTracking.Tool.* $msBuildPublishDir
}

function Publish-Ui([String] $msBuildPublishDir) {
	Build-Ui

	# Move SPA to publish folder
	mv FS.TimeTracking.UI/FS.TimeTracking.UI.Angular/dist/TimeTracking $msBuildPublishDir/webui
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