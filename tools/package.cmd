@echo off
echo 'Building package' %APPVEYOR_BUILD_VERSION%

msbuild JockeOverlays.csproj -p:SolutionDir=%cd%\                                          ^
                                        -p:Configuration=Release                           ^
                                        -t:rebuild                                         ^
                                        -p:ApplicationVersion=%APPVEYOR_BUILD_VERSION%     ^
                                        -v:minimal

cd bin\Release

7z a release.zip JockeOverlays.* JockeOverlays.*

appveyor PushArtifact release.zip -FileName "release.zip" -DeploymentName deploy-release

cd ..\..
 