@echo off
dotnet build src/Limbo.Umbraco.YouTube --configuration Debug /t:rebuild /t:pack -p:PackageOutputPath=c:\nuget\Umbraco10