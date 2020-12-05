echo start

start "Backbone" cmd.exe /k "dotnet Backbone\Release\netcoreapp2.2\Backbone.dll -b 0.0.0.0:50001"
sleep 2
REM Start Servers
start "Server 1" cmd.exe /k "dotnet EchoServer\Release\netcoreapp2.2\EchoServer.dll -s 0.0.0.0:50010 -b 127.0.0.1:50001"
start "Server 2" cmd.exe /k "dotnet EchoServer\Release\netcoreapp2.2\EchoServer.dll -s 0.0.0.0:50011 -b 127.0.0.1:50001"
sleep 2
REM Start Client
start "Client 1" cmd.exe /k "dotnet EchoClient\Release\netcoreapp2.2\EchoClient.dll -s 127.0.0.1:50010"
start "Client 2" cmd.exe /k "dotnet EchoClient\Release\netcoreapp2.2\EchoClient.dll -s 127.0.0.1:50010"
start "Client 3" cmd.exe /k "dotnet EchoClient\Release\netcoreapp2.2\EchoClient.dll -s 127.0.0.1:50011"
start "Client 4" cmd.exe /k "dotnet EchoClient\Release\netcoreapp2.2\EchoClient.dll -s 127.0.0.1:50011"


exit