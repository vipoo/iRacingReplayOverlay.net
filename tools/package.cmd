@echo off
echo 'Building test package' %APPVEYOR_BUILD_VERSION%

msbuild iRacingReplayOverlay.net.csproj -t:rebuild -t:publish -p:"InstallUrl=http://iracing-replay-director.s3-ap-southeast-2.amazonaws.com/test/" -p:ApplicationVersion=%APPVEYOR_BUILD_VERSION% -v:minimal -p:ProductName="iRacing Replay Director (test)"

appveyor PushArtifact bin\x64\Debug\iRacingReplayOverlay.exe -FileName "versions\iRacingReplayOverlay.%APPVEYOR_BUILD_VERSION%.exe" -DeploymentName deploy-test 

cd bin\x64\Debug\app.publish
for /D %%F in ("Application Files\*") do (
	appveyor PushArtifact "%%F\iRacingReplayOverlay.exe.config.deploy" -FileName "test\%%F\iRacingReplayOverlay.exe.config.deploy" -DeploymentName deploy-test 
	appveyor PushArtifact "%%F\iRacingReplayOverlay.exe.deploy"        -FileName "test\%%F\iRacingReplayOverlay.exe.deploy"        -DeploymentName deploy-test 
	appveyor PushArtifact "%%F\iRacingReplayOverlay.pdb.deploy"        -FileName "test\%%F\iRacingReplayOverlay.pdb.deploy"        -DeploymentName deploy-test 
	appveyor PushArtifact "%%F\iRacingReplayOverlay.exe.manifest"      -FileName "test\%%F\iRacingReplayOverlay.exe.manifest"      -DeploymentName deploy-test 
)

appveyor PushArtifact setup.exe                             -FileName "test\setup.exe"                              -DeploymentName deploy-test
appveyor PushArtifact iRacingReplayOverlay.test.application -FileName "test\iRacingReplayOverlay.test.application"  -DeploymentName deploy-test

cd ..\..\..\..

echo 'Building beta package' %APPVEYOR_BUILD_VERSION%

msbuild iRacingReplayOverlay.net.csproj -t:rebuild -t:publish -p:"InstallUrl=http://iracing-replay-director.s3-ap-southeast-2.amazonaws.com/beta/" -p:ApplicationVersion=%APPVEYOR_BUILD_VERSION% -v:minimal -p:ProductName="iRacing Replay Director (beta)"

cd bin\x64\Debug\app.publish
for /D %%F in ("Application Files\*") do (
	appveyor PushArtifact "%%F\iRacingReplayOverlay.exe.config.deploy" -FileName "beta\%%F\iRacingReplayOverlay.exe.config.deploy" -DeploymentName deploy-beta 
	appveyor PushArtifact "%%F\iRacingReplayOverlay.exe.deploy"        -FileName "beta\%%F\iRacingReplayOverlay.exe.deploy"        -DeploymentName deploy-beta 
	appveyor PushArtifact "%%F\iRacingReplayOverlay.pdb.deploy"        -FileName "beta\%%F\iRacingReplayOverlay.pdb.deploy"        -DeploymentName deploy-beta 
	appveyor PushArtifact "%%F\iRacingReplayOverlay.exe.manifest"      -FileName "beta\%%F\iRacingReplayOverlay.exe.manifest"      -DeploymentName deploy-beta 
)

appveyor PushArtifact setup.exe                             -FileName "beta\setup.exe"                              -DeploymentName deploy-beta
appveyor PushArtifact iRacingReplayOverlay.beta.application -FileName "beta\iRacingReplayOverlay.beta.application"  -DeploymentName deploy-beta

cd ..\..\..\..

echo 'Building main package' %APPVEYOR_BUILD_VERSION%

msbuild iRacingReplayOverlay.net.csproj -t:rebuild -t:publish -p:"InstallUrl=http://iracing-replay-director.s3-ap-southeast-2.amazonaws.com/main/" -p:ApplicationVersion=%APPVEYOR_BUILD_VERSION% -v:minimal -p:ProductName="iRacing Replay Director"

cd bin\x64\Debug\app.publish
for /D %%F in ("Application Files\*") do (
	appveyor PushArtifact "%%F\iRacingReplayOverlay.exe.config.deploy" -FileName "main\%%F\iRacingReplayOverlay.exe.config.deploy" -DeploymentName deploy-main 
	appveyor PushArtifact "%%F\iRacingReplayOverlay.exe.deploy"        -FileName "main\%%F\iRacingReplayOverlay.exe.deploy"        -DeploymentName deploy-main 
	appveyor PushArtifact "%%F\iRacingReplayOverlay.pdb.deploy"        -FileName "main\%%F\iRacingReplayOverlay.pdb.deploy"        -DeploymentName deploy-main 
	appveyor PushArtifact "%%F\iRacingReplayOverlay.exe.manifest"      -FileName "main\%%F\iRacingReplayOverlay.exe.manifest"      -DeploymentName deploy-main 
)

appveyor PushArtifact setup.exe                        -FileName "main\setup.exe"                         -DeploymentName deploy-main
appveyor PushArtifact iRacingReplayOverlay.application -FileName "main\iRacingReplayOverlay.application"  -DeploymentName deploy-main

cd ..\..\..\..