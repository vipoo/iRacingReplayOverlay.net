@echo off
SET SolutionDir=%~dp0..\..\
SET ProjectDir=%~dp0..\

msbuild %ProjectDir%SuperMFLib.csproj -p:SolutionDir=%SolutionDir% -t:rebuild -p:ApplicationVersion=%APPVEYOR_BUILD_VERSION% -v:minimal -p:Configuration=Release -p:OverrideAssemblyName=MediaFoundation.unmerged

%SolutionDir%packages\ILMerge.2.14.1208\tools\ILMerge.exe /out:%ProjectDir%bin\x64\Release\MediaFoundation.Net.dll %ProjectDir%bin\x64\Release\MediaFoundation.unmerged.dll %ProjectDir%bin\x64\Release\MediaFoundation.dll

%ProjectDir%tools\nuget.exe pack %ProjectDir%SuperMFLib.nuspec -Prop Configuration=release -Verbosity detail -Version %APPVEYOR_BUILD_VERSION%