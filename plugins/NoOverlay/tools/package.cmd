@echo off
echo 'Building package' %APPVEYOR_BUILD_VERSION%

msbuild NoOverlay.csproj -p:SolutionDir=%cd%\                                          ^
                                        -p:Configuration=Release                           ^
                                        -t:rebuild                                         ^
                                        -p:ApplicationVersion=%APPVEYOR_BUILD_VERSION%     ^
                                        -v:minimal

cd bin\Release

7z a release.zip NoOverlay.* iRacingDirector.Plugin.Support.*

appveyor PushArtifact release.zip -FileName "release.zip" -DeploymentName deploy-release

cd ..\..
 