
msbuild iRacingReplayOverlay.net.csproj  -t:rebuild -t:publish -p:ApplicationVersion=1.0.0.50 -v:normal
rd /S /Q "publish\Application Files"
move "bin\x64\Debug\app.publish\*" publish
move "bin\x64\Debug\app.publish\Application Files" "publish\Application Files"
