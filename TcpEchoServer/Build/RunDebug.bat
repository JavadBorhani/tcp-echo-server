echo start

start "Backbone" cmd.exe /k "dotnet Backbone\Debug\netcoreapp2.2\Backbone.dll -b 0.0.0.0:50001"
sleep 2
REM Start Servers
start "Server 1" cmd.exe /k "dotnet EchoServer\Debug\netcoreapp2.2\EchoServer.dll -s 0.0.0.0:50010 -b 127.0.0.1:50001"
REM start "Server 2" cmd.exe /k "dotnet EchoServer\Debug\netcoreapp2.2\EchoServer.dll -s 0.0.0.0:50011 -b 127.0.0.1:50001"
REM start "Server 3" cmd.exe /k "dotnet EchoServer\Debug\netcoreapp2.2\EchoServer.dll -s 0.0.0.0:50012 -b 127.0.0.1:50001"

REM Start Client
start "Client 1" cmd.exe /k "dotnet EchoClient\Debug\netcoreapp2.2\EchoClient.dll -s 0.0.0.0:50010 -b 127.0.0.1:50001"
start "Client 2" cmd.exe /k "dotnet EchoClient\Debug\netcoreapp2.2\EchoClient.dll -s 0.0.0.0:50010 -b 127.0.0.1:50001"

exit