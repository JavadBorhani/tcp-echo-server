@echo off
echo start
dotnet restore
dotnet build -c Release
pause