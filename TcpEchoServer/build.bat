echo start
dotnet restore
dotnet build 
dotnet build -c Release
exit