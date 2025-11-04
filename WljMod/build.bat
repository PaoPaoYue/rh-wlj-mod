@echo off
chcp 65001 >nul
setlocal enabledelayedexpansion

:: Set the target folder path (relative to parent artifacts folder)
set "TARGET_DIR=..\artifacts\王老菊Mod\plugins"

:: Build the project using dotnet CLI
echo Building the project...
dotnet build
if errorlevel 1 (
    echo Build failed. Exiting script.
    exit /b 1
)

:: Create the target folder if it doesn't exist
if not exist "%TARGET_DIR%" (
    echo Creating target folder: %TARGET_DIR%
    mkdir "%TARGET_DIR%"
)

:: Copy all DLLs from the build output to the target folder
echo Copying DLLs to %TARGET_DIR% ...
xcopy "bin\Debug\netstandard2.1\*.dll" "%TARGET_DIR%\" /Y /I

echo Done.
pause
