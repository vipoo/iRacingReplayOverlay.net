@echo off
echo 'Building test package' %APPVEYOR_BUILD_VERSION%

msbuild iRacingReplayOverlay.net.csproj -p:SolutionDir=%cd%\                                                                    ^
                                        -p:Configuration=Release                                                                ^
                                        -t:rebuild                                                                              ^
                                        -p:ApplicationVersion=%APPVEYOR_BUILD_VERSION%                                          ^
                                        -v:minimal                                                                              ^
                                        -p:ProductName="iRacing Replay Director"                                         ^
                                        -p:OverrideAssemblyName=iRacingReplayOverlay

copy bin\x64\Release\iRacingReplayOverlay.exe          iRacingReplayOverlay.exe
copy bin\x64\Release\iRacingReplayOverlay.pdb          iRacingReplayOverlay.pdb
copy bin\x64\Release\iRacingReplayOverlay.exe.config   iRacingReplayOverlay.exe.config
copy bin\x64\Release\iRacingReplayOverlay.exe.manifest iRacingReplayOverlay.exe.manifest

7z a release.zip iRacingReplayOverlay.exe  iRacingReplayOverlay.pdb iRacingReplayOverlay.exe.config iRacingReplayOverlay.exe.manifest

appveyor PushArtifact release.zip -FileName "release.zip" -DeploymentName deploy-release
 