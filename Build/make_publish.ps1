# Ensure unsigned powershell script execution ist allowed: Set-ExecutionPolicy -ExecutionPolicy RemoteSigned

param (
  [Parameter(Mandatory=$true)][String]$projectName,
  [Parameter(Mandatory=$true)][String]$version,
  [Parameter(Mandatory=$true)][String]$publshFolder,
  [Parameter(Mandatory=$true)][String]$runtime
)

. $PSScriptRoot/make_shared.ps1

Push-Location $PSScriptRoot/..

$projectName="FS.$projectName"
$fileVersion=$version -replace '(\d+(?:\.\d+)*)(.*)', '$1'
$targetFramework="net6.0"
$configuration="Release"
$msBuildOutDir="FS.TimeTracking/FS.TimeTracking/bin/$configuration/$targetFramework"
$msBuildPublishDir="$msBuildOutDir/$runtime/publish"

Npm-Restore
Clean-Folder -folder $publshFolder
Clean-Folder -folder $msBuildPublishDir
Publish-Rest-Services -configuration $configuration -targetFramework $targetFramework -runtime $runtime -version $version -fileVersion $fileVersion -msBuildPublishDir $msBuildPublishDir
Publish-Tool -configuration $configuration -targetFramework $targetFramework -runtime $runtime -version $version -fileVersion $fileVersion -msBuildPublishDir $msBuildPublishDir
Publish-Ui -msBuildPublishDir $msBuildPublishDir
Publish-Merge-To-Artifact-Folder -projectName $projectName -runtime $runtime -publshFolder $publshFolder -msBuildPublishDir $msBuildPublishDir

Pop-Location