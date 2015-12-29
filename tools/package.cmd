@echo off
echo 'Building test package' %APPVEYOR_BUILD_VERSION%

msbuild iRacingReplayOverlay.net.csproj -p:SolutionDir=%cd%\                                                                    ^
                                        -p:Configuration=Release                                                                ^
                                        -p:UpdateRequired=true                                                                  ^
                                        -p:MinimumRequiredVersion=%APPVEYOR_BUILD_VERSION%                                      ^
                                        -t:rebuild                                                                              ^
                                        -t:publish                                                                              ^
                                        -p:"InstallUrl=https://iracing-replay-director.s3-ap-southeast-2.amazonaws.com/stest/"  ^
                                        -p:ApplicationVersion=%APPVEYOR_BUILD_VERSION%                                          ^
                                        -v:minimal                                                                              ^
                                        -p:ProductName="iRacing Replay Director (test)"                                         ^
                                        -p:OverrideAssemblyName=iRacingReplayOverlay.test

appveyor PushArtifact bin\x64\Release\iRacingReplayOverlay.test.exe -FileName "versions\iRacingReplayOverlay.%APPVEYOR_BUILD_VERSION%.exe" -DeploymentName deploy-test 

cd bin\x64\Release\app.publish
for /D %%F in ("Application Files\*") do (
    appveyor PushArtifact "%%F\iRacingReplayOverlay.test.exe.config.deploy" -FileName "stest\%%F\iRacingReplayOverlay.test.exe.config.deploy" -DeploymentName deploy-test 
    appveyor PushArtifact "%%F\iRacingReplayOverlay.test.exe.deploy"        -FileName "stest\%%F\iRacingReplayOverlay.test.exe.deploy"        -DeploymentName deploy-test 
    appveyor PushArtifact "%%F\iRacingReplayOverlay.test.pdb.deploy"        -FileName "stest\%%F\iRacingReplayOverlay.test.pdb.deploy"        -DeploymentName deploy-test 
    appveyor PushArtifact "%%F\iRacingReplayOverlay.test.exe.manifest"      -FileName "stest\%%F\iRacingReplayOverlay.test.exe.manifest"      -DeploymentName deploy-test 
)

appveyor PushArtifact setup.exe                             -FileName "stest\setup.exe"                              -DeploymentName deploy-test
appveyor PushArtifact iRacingReplayOverlay.test.application -FileName "stest\iRacingReplayOverlay.test.application"  -DeploymentName deploy-test
cd ..\..\..\..



msbuild iRacingReplayOverlay.net.csproj -p:SolutionDir=%cd%\                                                                        ^
                                        -p:Configuration=Release                                                                    ^
                                        -p:UpdateRequired=true                                                                      ^
                                        -p:"UpdateUrl=https://iracing-replay-director.s3-ap-southeast-2.amazonaws.com/stest/"       ^
                                        -p:MinimumRequiredVersion=1.0.1.78                                                          ^
                                        -t:rebuild                                                                                  ^
                                        -t:publish                                                                                  ^
                                        -p:"InstallUrl=http://iracing-replay-director.s3-ap-southeast-2.amazonaws.com/test/"        ^
                                        -p:ApplicationVersion=1.0.1.78                                                              ^
                                        -v:minimal                                                                                  ^
                                        -p:ProductName="iRacing Replay Director (test)"                                             ^
                                        -p:OverrideAssemblyName=iRacingReplayOverlay.test

cd bin\x64\Release\app.publish
for /D %%F in ("Application Files\*") do (
    appveyor PushArtifact "%%F\iRacingReplayOverlay.test.exe.config.deploy" -FileName "test\%%F\iRacingReplayOverlay.test.exe.config.deploy" -DeploymentName deploy-test 
    appveyor PushArtifact "%%F\iRacingReplayOverlay.test.exe.deploy"        -FileName "test\%%F\iRacingReplayOverlay.test.exe.deploy"        -DeploymentName deploy-test 
    appveyor PushArtifact "%%F\iRacingReplayOverlay.test.pdb.deploy"        -FileName "test\%%F\iRacingReplayOverlay.test.pdb.deploy"        -DeploymentName deploy-test 
    appveyor PushArtifact "%%F\iRacingReplayOverlay.test.exe.manifest"      -FileName "test\%%F\iRacingReplayOverlay.test.exe.manifest"      -DeploymentName deploy-test 
)

appveyor PushArtifact setup.exe                             -FileName "test\setup.exe"                              -DeploymentName deploy-test
appveyor PushArtifact iRacingReplayOverlay.test.application -FileName "test\iRacingReplayOverlay.test.application"  -DeploymentName deploy-test
cd ..\..\..\..











echo 'Building beta package' %APPVEYOR_BUILD_VERSION%

msbuild iRacingReplayOverlay.net.csproj -p:SolutionDir=%cd%\ -p:Configuration=Release -t:rebuild -t:publish -p:"InstallUrl=http://iracing-replay-director.s3-ap-southeast-2.amazonaws.com/beta/" -p:ApplicationVersion=%APPVEYOR_BUILD_VERSION% -v:minimal -p:ProductName="iRacing Replay Director (beta)" -p:OverrideAssemblyName=iRacingReplayOverlay.beta

cd bin\x64\Release\app.publish
for /D %%F in ("Application Files\*") do (
    appveyor PushArtifact "%%F\iRacingReplayOverlay.beta.exe.config.deploy" -FileName "beta\%%F\iRacingReplayOverlay.beta.exe.config.deploy" -DeploymentName deploy-beta 
    appveyor PushArtifact "%%F\iRacingReplayOverlay.beta.exe.deploy"        -FileName "beta\%%F\iRacingReplayOverlay.beta.exe.deploy"        -DeploymentName deploy-beta 
    appveyor PushArtifact "%%F\iRacingReplayOverlay.beta.pdb.deploy"        -FileName "beta\%%F\iRacingReplayOverlay.beta.pdb.deploy"        -DeploymentName deploy-beta 
    appveyor PushArtifact "%%F\iRacingReplayOverlay.beta.exe.manifest"      -FileName "beta\%%F\iRacingReplayOverlay.beta.exe.manifest"      -DeploymentName deploy-beta 
)

appveyor PushArtifact setup.exe                             -FileName "beta\setup.exe"                              -DeploymentName deploy-beta
appveyor PushArtifact iRacingReplayOverlay.beta.application -FileName "beta\iRacingReplayOverlay.beta.application"  -DeploymentName deploy-beta

cd ..\..\..\..

echo 'Building main package' %APPVEYOR_BUILD_VERSION%

msbuild iRacingReplayOverlay.net.csproj -p:SolutionDir=%cd%\ -p:Configuration=Release -t:rebuild -t:publish -p:"InstallUrl=http://iracing-replay-director.s3-ap-southeast-2.amazonaws.com/main/" -p:ApplicationVersion=%APPVEYOR_BUILD_VERSION% -v:minimal -p:ProductName="iRacing Replay Director" -p:OverrideAssemblyName=iRacingReplayOverlay

cd bin\x64\Release\app.publish
for /D %%F in ("Application Files\*") do (
    appveyor PushArtifact "%%F\iRacingReplayOverlay.exe.config.deploy" -FileName "main\%%F\iRacingReplayOverlay.exe.config.deploy" -DeploymentName deploy-main 
    appveyor PushArtifact "%%F\iRacingReplayOverlay.exe.deploy"        -FileName "main\%%F\iRacingReplayOverlay.exe.deploy"        -DeploymentName deploy-main 
    appveyor PushArtifact "%%F\iRacingReplayOverlay.pdb.deploy"        -FileName "main\%%F\iRacingReplayOverlay.pdb.deploy"        -DeploymentName deploy-main 
    appveyor PushArtifact "%%F\iRacingReplayOverlay.exe.manifest"      -FileName "main\%%F\iRacingReplayOverlay.exe.manifest"      -DeploymentName deploy-main 
)

appveyor PushArtifact setup.exe                        -FileName "main\setup.exe"                         -DeploymentName deploy-main
appveyor PushArtifact iRacingReplayOverlay.application -FileName "main\iRacingReplayOverlay.application"  -DeploymentName deploy-main

cd ..\..\..\..