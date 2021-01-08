# Ensure unsigned powershell script execution ist allowed: Set-ExecutionPolicy -ExecutionPolicy RemoteSigned

Push-Location $PSScriptRoot\..

Get-ChildItem "TestResult" -Recurse | foreach { $_.Delete($TRUE) }
& dotnet test --configuration Release --logger:trx --logger:html
if(!$?) {
	exit $LASTEXITCODE
}

Pop-Location