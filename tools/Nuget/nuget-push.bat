@echo off
for /r %%i in (*.nupkg) do (
    ..\..\.nuget\NuGet.exe push %%i -ApiKey %1 -Source https://www.nuget.org/api/v2/package
)