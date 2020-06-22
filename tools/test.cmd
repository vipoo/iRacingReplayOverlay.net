SET SolutionDir=%~dp0..\
SET ProjectDir=%~dp0..\

msbuild %ProjectDir%iRacingReplayDirector.net.Tests\iRacingReplayDirector.net.Tests.csproj ^
	-p:SolutionDir=%SolutionDir% ^
	-t:build ^
	-v:minimal ^
	-p:Configuration=Release

.\packages\NUnit.Runners.2.6.3\tools\nunit-console.exe -nodots -nologo -labels ^
	.\iRacingReplayDirector.net.Tests\bin\x64\Release\iRacingReplayDirector.net.Tests.exe
