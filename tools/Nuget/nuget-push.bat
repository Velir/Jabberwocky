@echo off

for /r .\symbols %%i in (*.symbols.nupkg) do (
    ..\..\.nuget\NuGet.exe push %%i -ApiKey %1 -Source https://nuget.smbsrc.net
)

for %%i in (*.nupkg) do (
    ..\..\.nuget\NuGet.exe push %%i -ApiKey %1 -Source https://www.nuget.org/api/v2/package
)