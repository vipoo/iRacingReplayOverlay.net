@echo off
echo 'Building package' %APPVEYOR_BUILD_VERSION%

msbuild iRacingDirector.Plugin.StandardOverlays.csproj -p:SolutionDir=%cd%\                ^
                                        -p:Configuration=Release                           ^
                                        -t:rebuild                                         ^
                                        -p:ApplicationVersion=%APPVEYOR_BUILD_VERSION%     ^
                                        -v:minimal

cd bin\Release

7z a release.zip iRacingDirector.Plugin.StandardOverlays.*

appveyor PushArtifact release.zip -FileName "release.zip" -DeploymentName deploy-release

cd ..\..
 