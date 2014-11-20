/c/Program\ Files\ \(x86\)/MSBuild/12.0/Bin/MSBuild.exe iRacingReplayOverlay.net.csproj -t:rebuild -t:publish -p:MSBuildExtensionsPath=/c/Program\ Files/MSBuild/ -p:ApplicationVersion=1.0.0.18
rm -rf publish/Application\ Files/
mv bin/x64/Debug/app.publish/* publish
