/c/Program\ Files\ \(x86\)/MSBuild/12.0/Bin/MSBuild.exe iRacingReplayOverlay.net.csproj -t:rebuild -t:publish -p:MSBuildExtensionsPath=/c/Program\ Files/MSBuild/ -p:ApplicationVersion=1.0.0.9 -p:"InstallUrl=http://iracingreplaydirector-beta.s3-ap-southeast-2.amazonaws.com/" -p:ProductName="iRacing Replay Director (beta)" -p:OverrideAssemblyName=iRacingReplayOverlay.beta
rm -rf publish-beta/Application\ Files/
mv bin/x64/Debug/app.publish/* publish-beta
