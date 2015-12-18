
echo 'Building package' %APPVEYOR_BUILD_NUMBER%
msbuild iRacingReplayOverlay.net.csproj -t:publish -p:ApplicationVersion=%APPVEYOR_BUILD_NUMBER% -v:minimal
rd /S /Q "deploy-package\Application Files"
move "bin\x64\Debug\app.publish\*" deploy-package
move "bin\x64\Debug\app.publish\Application Files" "deploy-package\Application Files"
