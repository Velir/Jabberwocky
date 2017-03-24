.nuget\NuGet.exe pack Jabberwocky.Core/Jabberwocky.Core.csproj -IncludeReferencedProjects -symbols -Prop Configuration=Release
.nuget\NuGet.exe pack Jabberwocky.Glass/Jabberwocky.Glass.csproj -IncludeReferencedProjects -symbols -Prop Configuration=Release
.nuget\NuGet.exe pack Jabberwocky.Glass.Autofac/Jabberwocky.Glass.Autofac.csproj -IncludeReferencedProjects -symbols -Prop Configuration=Release
.nuget\NuGet.exe pack Jabberwocky.Glass.Mvc/Jabberwocky.Glass.Mvc.csproj -IncludeReferencedProjects -symbols -Prop Configuration=Release
.nuget\NuGet.exe pack Jabberwocky.Glass.Autofac.Mvc/Jabberwocky.Glass.Autofac.Mvc.csproj -IncludeReferencedProjects -symbols -Prop Configuration=Release
.nuget\NuGet.exe pack Jabberwocky.Glass.Autofac.WebApi/Jabberwocky.Glass.Autofac.WebApi.csproj -IncludeReferencedProjects -symbols -Prop Configuration=Release
.nuget\NuGet.exe pack Jabberwocky.WebApi/Jabberwocky.WebApi.csproj -IncludeReferencedProjects -symbols -Prop Configuration=Release
.nuget\NuGet.exe pack Jabberwocky.Autofac/Jabberwocky.Autofac.csproj -IncludeReferencedProjects -symbols -Prop Configuration=Release
.nuget\NuGet.exe pack Jabberwocky.Autofac.Extras.MiniProfiler/Jabberwocky.Autofac.Extras.MiniProfiler.csproj -IncludeReferencedProjects -symbols -Prop Configuration=Release
.nuget\NuGet.exe pack Jabberwocky.Core.CodeAnalysis/Jabberwocky.Core.CodeAnalysis/Jabberwocky.Core.CodeAnalysis.nuspec -NoPackageAnalysis -Version 1.0.2 -OutputDirectory . -Prop Configuration=Release
.nuget\NuGet.exe pack Jabberwocky.Glass.CodeAnalysis/Jabberwocky.Glass.CodeAnalysis/Jabberwocky.Glass.CodeAnalysis.nuspec -NoPackageAnalysis -Version 1.0.2 -OutputDirectory . -Prop Configuration=Release

del /F /Q Nuget
move /Y *.nupkg Nuget