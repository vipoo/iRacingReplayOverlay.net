
echo 'Building test package' %APPVEYOR_BUILD_NUMBER%

msbuild iRacingReplayOverlay.net.csproj -t:publish -p:"InstallUrl=http://iracingreplaydirector-test.s3-ap-southeast-2.amazonaws.com/" -p:ApplicationVersion=%APPVEYOR_BUILD_VERSION% -v:minimal -p:ProductName="iRacing Replay Director (test)" -p:OverrideAssemblyName=iRacingReplayOverlay.test
rd /S /Q "deploy\test\Application Files"
move "bin\x64\Debug\app.publish\*" deploy\test
move "bin\x64\Debug\app.publish\Application Files" "deploy\test\Application Files"
cd deploy\test
7z a .\..\..\deploy-test.zip .\**\*
cd .\..\..
appveyor PushArtifact deploy-test.zip

