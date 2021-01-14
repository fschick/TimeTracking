# Ensure unsigned powershell script execution ist allowed: Set-ExecutionPolicy -ExecutionPolicy RemoteSigned

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

function Build-Angular {
    # Switch to UI project
	Push-Location FS.TimeTracking.UI.Angular
	
	# Build SPA
	& npm run build-prod
	if(!$?) {
		Pop-Location
		exit $LASTEXITCODE
	}
	
	Pop-Location
}