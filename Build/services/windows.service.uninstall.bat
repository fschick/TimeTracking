@echo off
set pathToRoot=%~dp0..\
pushd %pathToRoot%

set serviceName=TimeTracking
sc stop %serviceName%
sc delete %serviceName%

popd %pathToRoot%
pause