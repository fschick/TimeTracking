# Ensure unsigned powershell script execution ist allowed: Set-ExecutionPolicy -ExecutionPolicy RemoteSigned

. $PSScriptRoot/make_shared.ps1

Push-Location $PSScriptRoot\..

Npm-Restore
Test-Back-End
Test-Front-End

Pop-Location


function Test-Back-End {
	Get-ChildItem "TestResult" -Recurse | foreach { $_.Delete($TRUE) }
	& dotnet test --configuration Release --logger:trx --logger:html
	if(!$?) {
		Pop-Location
		exit $LASTEXITCODE
	}
}

function Test-Front-End {
	# Switch to UI project
	Push-Location FS.TimeTracking.UI.Angular
	& npm run test-headless
	if(!$?) {
		Pop-Location
		exit $LASTEXITCODE
	}
	Pop-Location
}