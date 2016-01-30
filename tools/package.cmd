@echo off
echo 'Building test package' %APPVEYOR_BUILD_VERSION%

msbuild iRacingReplayOverlay.net.csproj -p:SolutionDir=%cd%\                               ^
                                        -p:Configuration=Release                           ^
                                        -t:rebuild                                         ^
                                        -p:ApplicationVersion=%APPVEYOR_BUILD_VERSION%     ^
                                        -v:minimal                                         ^
                                        -p:ProductName="iRacing Replay Director"           ^
                                        -p:OverrideAssemblyName=iRacingReplayOverlay

cd bin\x64\Release

7z a release.zip *.*

appveyor PushArtifact release.zip -FileName "release.zip" -DeploymentName deploy-release

cd ..\..\..
 