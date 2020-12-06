echo start
dotnet restore
dotnet build -c Debug
dotnet build -c Release
exit