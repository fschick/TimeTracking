# Ensure unsigned powershell script execution ist allowed: Set-ExecutionPolicy -ExecutionPolicy RemoteSigned

#
# Shared
#
function Npm-Restore {
    # Switch to UI project
	Push-Location FS.TimeTracking.UI.Angular
	
	# Restore npm packages
	& npm ci --prefer-offline --no-audit
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
	# Switch to backend project
	Push-Location FS.TimeTracking
	
	# Build back-end
	Copy-Item $NUGET_CONFIG /nuget.config -Force
	& dotnet build -warnaserror --configuration Debug
	& dotnet build -warnaserror --configuration Release -p:Version=$version -p:FileVersion=$fileVersion
	if(!$?) {
		Pop-Location
		exit $LASTEXITCODE
	}
	
	Pop-Location
}

function Build-Tool([String] $version = "0.0.0", [String] $fileVersion = "0.0.0") {
	# Switch to tool project
	Push-Location FS.TimeTracking.Tool
	
	# Build back-end
	Copy-Item $NUGET_CONFIG /nuget.config -Force
	& dotnet build -warnaserror --configuration Debug
	& dotnet build -warnaserror --configuration Release -p:Version=$version -p:FileVersion=$fileVersion
	if(!$?) {
		Pop-Location
		exit $LASTEXITCODE
	}
	
	Pop-Location
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
function Test-Rest-Services([String] $filter) {
	# Switch to backend project
	Push-Location FS.TimeTracking
	
	Copy-Item $NUGET_CONFIG /nuget.config -Force
	Get-ChildItem "TestResult" -Recurse | foreach { $_.Delete($TRUE) }
	if ($filter) {
		& dotnet test --configuration Release --logger:trx --logger:html --filter $filter
	} else {
		& dotnet test --configuration Release --logger:trx --logger:html
	}
	if(!$?) {
		Pop-Location
		exit $LASTEXITCODE
	}
	
	Pop-Location
}

function Test-Tool([String] $filter) {
	# Switch to tool project
	Push-Location FS.TimeTracking.Tool
	
	Copy-Item $NUGET_CONFIG /nuget.config -Force
	Get-ChildItem "TestResult" -Recurse | foreach { $_.Delete($TRUE) }
	if ($filter) {
		& dotnet test --configuration Release --logger:trx --logger:html --filter $filter
	} else {
		& dotnet test --configuration Release --logger:trx --logger:html
	}
	if(!$?) {
		Pop-Location
		exit $LASTEXITCODE
	}
	
	Pop-Location
}

function Test-Ui {
	# Switch to UI project
	Push-Location FS.TimeTracking.UI.Angular
	
	& npm run test:headless
	if(!$?) {
		Pop-Location
		exit $LASTEXITCODE
	}
	
	& npm run test:e2e
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
	Copy-Item $NUGET_CONFIG /nuget.config -Force
    
	# Generate Open API spec, Angular client and validation spec by running runtime independent build
	& dotnet build FS.TimeTracking/FS.TimeTracking/FS.TimeTracking.csproj --configuration $configuration
	if(!$?) {
		exit $LASTEXITCODE
	}
	
	# Publish back-end
	if ($runtime) {
		& dotnet publish FS.TimeTracking/FS.TimeTracking/FS.TimeTracking.csproj --configuration $configuration --framework $targetFramework -p:Version=$version -p:FileVersion=$fileVersion --self-contained --runtime $runtime 
	} else {
		& dotnet publish FS.TimeTracking/FS.TimeTracking/FS.TimeTracking.csproj --configuration $configuration --framework $targetFramework -p:Version=$version -p:FileVersion=$fileVersion --no-self-contained
	}
	if(!$?) {
		exit $LASTEXITCODE
	}

	Remove-Item $msBuildPublishDir/config/FS.TimeTracking.config.Development.json -ErrorAction Ignore
}

function Publish-Tool([String] $configuration, [String] $targetFramework, [String] $runtime, [String] $version, [String] $fileVersion, [String] $msBuildPublishDir) {
    Copy-Item $NUGET_CONFIG /nuget.config -Force
	
	# Publish tool
	if ($runtime) {
		& dotnet publish FS.TimeTracking.Tool/FS.TimeTracking.Tool/FS.TimeTracking.Tool.csproj --configuration $configuration --framework $targetFramework -p:Version=$version -p:FileVersion=$fileVersion --self-contained --runtime $runtime
	} else {
		& dotnet publish FS.TimeTracking.Tool/FS.TimeTracking.Tool/FS.TimeTracking.Tool.csproj --configuration $configuration --framework $targetFramework -p:Version=$version -p:FileVersion=$fileVersion --no-self-contained
	}
	
	if(!$?) {
		exit $LASTEXITCODE
	}

	# Move Tool to publish folder
	if ($runtime) {
		Move-Item FS.TimeTracking.Tool/FS.TimeTracking.Tool/bin/$configuration/$targetFramework/$runtime/publish/FS.TimeTracking.Tool $msBuildPublishDir -ErrorAction SilentlyContinue
		Move-Item FS.TimeTracking.Tool/FS.TimeTracking.Tool/bin/$configuration/$targetFramework/$runtime/publish/FS.TimeTracking.Tool.* $msBuildPublishDir
	} else {
		Move-Item FS.TimeTracking.Tool/FS.TimeTracking.Tool/bin/$configuration/$targetFramework/publish/FS.TimeTracking.Tool $msBuildPublishDir -ErrorAction SilentlyContinue
		Move-Item FS.TimeTracking.Tool/FS.TimeTracking.Tool/bin/$configuration/$targetFramework/publish/FS.TimeTracking.Tool.* $msBuildPublishDir
	}
}

function Publish-Ui([String] $msBuildPublishDir) {
	Build-Ui

	# Move SPA to publish folder
	Move-Item FS.TimeTracking.UI.Angular/dist/TimeTracking $msBuildPublishDir/webui
}

function Publish-Merge-To-Artifact-Folder([String] $projectName, [String] $runtime, [String] $publshFolder, [String] $msBuildPublishDir) {
	# Move application to publish folder
	if (!$runtime) {
		Move-Item $msBuildPublishDir $publshFolder
	} elseif ($runtime.StartsWith("win")) {
		Move-Item $msBuildPublishDir $publshFolder
		Copy-Item $PSScriptRoot/service_windows.install.bat $publshFolder/$projectName.WindowsService.Install.bat
		Copy-Item $PSScriptRoot/service_windows.uninstall.bat $publshFolder/$projectName.WindowsService.Uninstall.bat
	} else {
		New-Item $publshFolder/opt/$projectName -ItemType Directory
		New-Item $publshFolder/etc/systemd/system -ItemType Directory
		Move-Item $msBuildPublishDir $publshFolder/opt/$projectName/bin
		Copy-Item $PSScriptRoot/service_linux.service $publshFolder/etc/systemd/system/$projectName.service
	}
}
#endregion