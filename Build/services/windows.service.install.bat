@echo off
set pathToRoot=%~dp0..\
pushd %pathToRoot%

set serviceName=TimeTracking
set executable=FS.TimeTracking.exe
for %%e in (%executable%) do set "executablePath=%%~fe"

sc create %serviceName% binPath= "%executablePath%" start= delayed-auto
sc start %serviceName%

popd %pathToRoot%
pause