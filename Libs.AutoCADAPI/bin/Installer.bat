@echo off
setlocal

set "sourceDir=%~dp0Libs.AutoCADAPI.bundle"
set "targetDir=C:\ProgramData\Autodesk\ApplicationPlugins\Libs.AutoCADAPI.bundle"

if exist "%targetDir%" (
    echo Deleting... %targetDir%
    rmdir /S /Q "%targetDir%"
)

mkdir "%targetDir%"

xcopy "%sourceDir%\*" "%targetDir%\" /E /I /Y

echo Done!
pause