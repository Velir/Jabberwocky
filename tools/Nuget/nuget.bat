setlocal enabledelayedexpansion
for /r ..\..\src\ %%i in (*.nuspec) do  (
set projfile=%%i
set projfile=!projfile:nuspec=csproj!
..\..\.nuget\NuGet.exe pack !projfile! -IncludeReferencedProjects -symbols -Build -OutputDirectory . -Prop Configuration=Release
)

..\..\.nuget\NuGet.exe pack ..\..\Jabberwocky.Library.nuspec -NoPackageAnalysis -Version 3.0.0-beta -OutputDirectory .

..\..\.nuget\NuGet.exe pack ..\..\diagnostics\Jabberwocky.Core.CodeAnalysis/Jabberwocky.Core.CodeAnalysis/Jabberwocky.Core.CodeAnalysis.nuspec -NoPackageAnalysis -Version 3.0.0-beta -OutputDirectory . -Prop Configuration=Release
..\..\.nuget\NuGet.exe pack ..\..\diagnostics\Jabberwocky.Glass.CodeAnalysis/Jabberwocky.Glass.CodeAnalysis/Jabberwocky.Glass.CodeAnalysis.nuspec -NoPackageAnalysis -Version 3.0.0-beta -OutputDirectory . -Prop Configuration=Release

REM - Copy symbol sources to 'symbols' directory
mkdir symbols

for %%i in (*.symbols.nupkg) do (
    move %%i ./symbols
)