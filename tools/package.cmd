@echo off
SET SolutionDir=%~dp0..\

echo 'Building test package' %APPVEYOR_BUILD_VERSION%

set STANDARD_OVERLAY_PLUGIN=1.0.0.28


msbuild %SolutionDir%plugins\iRacingDirector.Plugin.Tester\iRacingDirector.Plugin.Tester.csproj -p:SolutionDir=%SolutionDir% -t:rebuild -p:ApplicationVersion=%APPVEYOR_BUILD_VERSION% -v:minimal -p:Configuration=Release

msbuild iRacingReplayOverlay.net.csproj -p:SolutionDir=%cd%\                               ^
                                        -p:Configuration=Release                           ^
                                        -t:rebuild                                         ^
                                        -p:ApplicationVersion=%APPVEYOR_BUILD_VERSION%     ^
                                        -v:minimal                                         ^
                                        -p:ProductName="iRacing Replay Director"           ^
                                        -p:OverrideAssemblyName=iRacingReplayOverlay

copy plugins\iRacingDirector.Plugin.Tester\bin\Release\iRacingDirector.Plugin.Tester.* bin\x64\Release\

cd bin\x64\Release

mkdir plugins\iRacingDirector.Plugin.StandardOverlays

cd plugins\iRacingDirector.Plugin.StandardOverlays
curl -L https://github.com/vipoo/iRacingDirector.Plugin.StandardOverlays/releases/download/%STANDARD_OVERLAY_PLUGIN%/release.zip --output release.zip
7z e release.zip
rm release.zip
cd ..\..

7z a -r release.zip *.*

appveyor PushArtifact release.zip -FileName "release.zip" -DeploymentName deploy-release

cd ..\..\..
