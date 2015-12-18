
echo 'Building package' %APPVEYOR_BUILD_NUMBER%
msbuild iRacingReplayOverlay.net.csproj -t:publish -p:ApplicationVersion=%APPVEYOR_BUILD_VERSION% -v:minimal
rd /S /Q "deploy\Application Files"
move "bin\x64\Debug\app.publish\*" deploy
move "bin\x64\Debug\app.publish\Application Files" "deploy\Application Files"
