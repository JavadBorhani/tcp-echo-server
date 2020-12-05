echo start

start "Backbone" cmd.exe /k "dotnet Backbone\Debug\netcoreapp2.2\Backbone.dll -b 0.0.0.0:50010"
sleep 2
start "Server 1" cmd.exe /k "dotnet EchoServer\Debug\netcoreapp2.2\EchoServer.dll -s 0.0.0.0:50001 -b 127.0.0.1:50010"
REM start "Server 2" cmd.exe /k "dotnet EchoServer\Debug\netcoreapp2.2\EchoServer.dll -s 0.0.0.0:50002 -b 127.0.0.1:50010"
REM start "Server 3" cmd.exe /k "dotnet EchoServer\Debug\netcoreapp2.2\EchoServer.dll -s 0.0.0.0:50003 -b 127.0.0.1:50010"

exit