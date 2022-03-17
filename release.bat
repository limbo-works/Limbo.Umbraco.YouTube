@echo off
dotnet build src/Limbo.Umbraco.YouTube --configuration Release /t:rebuild /t:pack -p:BuildTools=1 -p:PackageOutputPath=../../releases/nuget