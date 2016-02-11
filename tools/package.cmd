SET SolutionDir=%~dp0..\
SET ProjectDir=%~dp0..\iRacingDirector.Plugin.Tester\

msbuild %ProjectDir%iRacingDirector.Plugin.Tester.csproj -p:SolutionDir=%SolutionDir% -t:rebuild -p:ApplicationVersion=%APPVEYOR_BUILD_VERSION% -v:minimal -p:Configuration=Release

%SolutionDir%tools\nuget.exe pack %SolutionDir%iRacingDirector.Plugin.Support.nuspec -Prop Configuration=release -Verbosity detail -Version %APPVEYOR_BUILD_VERSION%

