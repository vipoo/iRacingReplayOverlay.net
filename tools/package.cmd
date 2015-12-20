@echo off
echo 'Building test package' %APPVEYOR_BUILD_VERSION%

msbuild iRacingReplayOverlay.net.csproj -t:rebuild -t:publish -p:"InstallUrl=http://iracing-replay-director.s3-ap-southeast-2.amazonaws.com/test/" -p:ApplicationVersion=%APPVEYOR_BUILD_VERSION% -v:minimal -p:ProductName="iRacing Replay Director (test)" -p:OverrideAssemblyName=iRacingReplayOverlay.test
rd /S /Q "Application Files"

appveyor PushArtifact bin\x64\Debug\iRacingReplayOverlay.test.exe -FileName "versions\iRacingReplayOverlay.%APPVEYOR_BUILD_VERSION%.exe" -DeploymentName deploy-test 

cd bin\x64\Debug\app.publish
for /D %%F in ("Application Files\*") do (
	appveyor PushArtifact "%%F\iRacingReplayOverlay.test.exe.config.deploy" -FileName "test\%%F\iRacingReplayOverlay.test.exe.config.deploy" -DeploymentName deploy-test 
	appveyor PushArtifact "%%F\iRacingReplayOverlay.test.exe.deploy"        -FileName "test\%%F\iRacingReplayOverlay.test.exe.deploy"        -DeploymentName deploy-test 
	appveyor PushArtifact "%%F\iRacingReplayOverlay.test.exe.manifest"      -FileName "test\%%F\iRacingReplayOverlay.test.exe.manifest"      -DeploymentName deploy-test 
)

appveyor PushArtifact setup.exe                             -FileName "test\setup.exe"                              -DeploymentName deploy-test
appveyor PushArtifact iRacingReplayOverlay.test.application -FileName "test\iRacingReplayOverlay.test.application"  -DeploymentName deploy-test

cd ..\..\..\..

echo 'Building beta package' %APPVEYOR_BUILD_VERSION%

msbuild iRacingReplayOverlay.net.csproj -t:rebuild -t:publish -p:"InstallUrl=http://iracing-replay-director.s3-ap-southeast-2.amazonaws.com/beta/" -p:ApplicationVersion=%APPVEYOR_BUILD_VERSION% -v:minimal -p:ProductName="iRacing Replay Director (beta)" -p:OverrideAssemblyName=iRacingReplayOverlay.beta
rd /S /Q "Application Files"

cd bin\x64\Debug\app.publish
for /D %%F in ("Application Files\*") do (
	appveyor PushArtifact "%%F\iRacingReplayOverlay.beta.exe.config.deploy" -FileName "beta\%%F\iRacingReplayOverlay.beta.exe.config.deploy" -DeploymentName deploy-beta 
	appveyor PushArtifact "%%F\iRacingReplayOverlay.beta.exe.deploy"        -FileName "beta\%%F\iRacingReplayOverlay.beta.exe.deploy"        -DeploymentName deploy-beta 
	appveyor PushArtifact "%%F\iRacingReplayOverlay.beta.exe.manifest"      -FileName "beta\%%F\iRacingReplayOverlay.beta.exe.manifest"      -DeploymentName deploy-beta 
)

appveyor PushArtifact setup.exe                             -FileName "beta\setup.exe"                              -DeploymentName deploy-beta
appveyor PushArtifact iRacingReplayOverlay.beta.application -FileName "beta\iRacingReplayOverlay.beta.application"  -DeploymentName deploy-beta

cd ..\..\..\..