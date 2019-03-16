dotnet pack -c Release --include-symbols
nuget push .\IoT.Display\bin\Release\IoT.Display.*.nupkg -Source https://api.nuget.org/v3/index.json