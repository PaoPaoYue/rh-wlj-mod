@echo off
setlocal

echo [1/3] Building WljMod...
pushd WljMod
call build.bat
popd
if %errorlevel% neq 0 exit /b %errorlevel%

echo [2/3] Deploying artifact...
pushd artifact
call deploy.bat
popd
if %errorlevel% neq 0 exit /b %errorlevel%

echo [3/3] Launching RouletteHero...
start "" "F:\Steam Games\steamapps\common\RouletteHero\Mod\ModDebug\RouletteHero.exe"

echo Done.