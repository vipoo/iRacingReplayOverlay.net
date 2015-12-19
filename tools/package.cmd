@echo off
echo 'Building test package' %APPVEYOR_BUILD_NUMBER%

msbuild iRacingReplayOverlay.net.csproj -t:publish -p:"InstallUrl=http://iracingreplaydirector-test.s3-ap-southeast-2.amazonaws.com/" -p:ApplicationVersion=%APPVEYOR_BUILD_VERSION% -v:minimal -p:ProductName="iRacing Replay Director (test)" -p:OverrideAssemblyName=iRacingReplayOverlay.test
rd /S /Q "Application Files"
move "bin\x64\Debug\app.publish\setup.exe" setup.exe
move "bin\x64\Debug\app.publish\iRacingReplayOverlay.test.application" iRacingReplayOverlay.test.application
move "bin\x64\Debug\app.publish\Application Files" "Application Files"

appveyor PushArtifact setup.exe -DeploymentName deploy-test
appveyor PushArtifact iRacingReplayOverlay.test.application -DeploymentName deploy-test

for /D %%F in ("Application Files\*") do (
	appveyor PushArtifact "%%F\iRacingReplayOverlay.test.exe.config.deploy" -FileName "%%F\iRacingReplayOverlay.test.exe.config.deploy" -DeploymentName deploy-test 
	appveyor PushArtifact "%%F\iRacingReplayOverlay.test.exe.deploy"        -FileName "%%F\iRacingReplayOverlay.test.exe.deploy"        -DeploymentName deploy-test 
	appveyor PushArtifact "%%F\iRacingReplayOverlay.test.exe.manifest"      -FileName "%%F\iRacingReplayOverlay.test.exe.manifest"      -DeploymentName deploy-test 
)



