set TargetName=FS.TimeTracking
set AngularDirectory=..\..\FS.TimeTracking.UI.Angular
set DebugOptions=debugModels,debugOperations,debugSupportingFiles
%AngularDirectory%\node_modules\.bin\openapi-generator-cli generate -c %AngularDirectory%\openapi-generator\config.json -g typescript-angular --template-dir %AngularDirectory%\openapi-generator\templates\typescript-angular --import-mappings=DateTime=luxon,Duration=luxon --type-mappings=DateTime=DateTime,date-span=Duration -i %TargetName%.openapi.json -o %AngularDirectory%\src\api\timetracking --global-property %DebugOptions%