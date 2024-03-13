@echo off
dotnet build src/Limbo.Umbraco.YouTube --configuration Release /t:rebuild /t:pack -p:PackageOutputPath=../../releases/nuget