@echo off
SET APIKEY=E7446C12-BD80-4272-3332-09914BE6EBC8
SET NUROOT=http://localhost:9088/local/v2/publish

cd nuget_exes

echo NuGet.3.5, Upload of simple file

NuGet.3.5.exe SetApiKey %APIKEY% -Source %NUROOT% 
NuGet.3.5.exe Push NLog.4.3.0.nupkg -Source %NUROOT%
dir ..\src\data\packages\*.nupkg


cd ..