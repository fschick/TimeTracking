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
Publish-Rest-Services -projectName $projectName -configuration $configuration -targetFramework $targetFramework -runtime $runtime -version $version -fileVersion $fileVersion -publshFolder $publshFolder -msBuildPublishDir $msBuildPublishDir
Publish-Ui -msBuildPublishDir $msBuildPublishDir
Publish-Merge-To-Artifact-Folder -projectName $projectName -runtime $runtime -publshFolder $publshFolder -msBuildPublishDir $msBuildPublishDir

Pop-Location