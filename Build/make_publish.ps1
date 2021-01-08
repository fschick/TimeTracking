# Ensure unsigned powershell script execution ist allowed: Set-ExecutionPolicy -ExecutionPolicy RemoteSigned

param (
  [Parameter(Mandatory=$true)][String]$projectName,
  [Parameter(Mandatory=$true)][String]$version,
  [Parameter(Mandatory=$true)][String]$publshFolder,
  [Parameter(Mandatory=$true)][String]$runtime
)

Push-Location $PSScriptRoot/..

$fileVersion=$version -replace '(\d+(?:\.\d+)*)(.*)', '$1'
$targetFramework="net5.0"
$configuration="Release"
$msBuildPublishDir="$projectName/bin/$configuration/$targetFramework/$runtime/publish"
$msBuildOutDir="$projectName/bin/$configuration/$targetFramework"

# Publish
& dotnet publish $projectName --configuration $configuration --framework $targetFramework --self-contained --runtime $runtime -p:Version=$version -p:FileVersion=$fileVersion
if(!$?) {
	exit $LASTEXITCODE
}

# Clean existing artifacts publish folder
Remove-Item $publshFolder -Recurse -ErrorAction Ignore -Force

if (Test-Path $msBuildPublishDir/$projectName.config.Development.json){
	Remove-Item $msBuildPublishDir/$projectName.config.Development.json
}
if (Test-Path $msBuildPublishDir/$projectName.config.User.json){
	Remove-Item $msBuildPublishDir/$projectName.config.User.json
}

# Generate Open API spec
& dotnet build $projectName --configuration $configuration
if(!$?) {
	exit $LASTEXITCODE
}

& dotnet "$msBuildOutDir/$projectName.dll" --generate-openapi "$projectName/$projectName.openapi.json"
if(!$?) {
	exit $LASTEXITCODE
}

# Switch to UI project
Push-Location FS.TimeTracking.UI.Angular

# Restore npm packages
& npm install --prefer-offline --no-audit
if(!$?) {
	Pop-Location
	exit $LASTEXITCODE
}

# Generate API client
#& npx openapi-generator generate -i ../$projectName/$projectName.openapi.json -g typescript-rxjs -o ./src/services/api -c api.config.json --import-mappings=DateTime=Date --type-mappings=DateTime=Date
#if(!$?) {
#	Pop-Location
#	exit $LASTEXITCODE
#}

# Build SPA
& npx ng build --prod
if(!$?) {
	Pop-Location
	exit $LASTEXITCODE
}

# Move SPA to publish folder
mv dist/TimeTracking ../$msBuildPublishDir/UI

# Restore directory stored by the PUSHD command.
Pop-Location

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

Pop-Location