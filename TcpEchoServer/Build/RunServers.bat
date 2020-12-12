echo start
REM Start Backbone
start "Backbone" cmd.exe /k "dotnet Backbone\Release\net5.0\Backbone.dll -b 0.0.0.0:50001 "
sleep 2
REM Start Servers
start "Server 1" cmd.exe /k "dotnet EchoServer\Release\net5.0\EchoServer.dll -s 0.0.0.0:50010 -b 127.0.0.1:50001 "
start "Server 2" cmd.exe /k "dotnet EchoServer\Release\net5.0\EchoServer.dll -s 0.0.0.0:50011 -b 127.0.0.1:50001 "
start "Server 3" cmd.exe /k "dotnet EchoServer\Release\net5.0\EchoServer.dll -s 0.0.0.0:50012 -b 127.0.0.1:50001 "
start "Server 4" cmd.exe /k "dotnet EchoServer\Release\net5.0\EchoServer.dll -s 0.0.0.0:50013 -b 127.0.0.1:50001 "
start "Server 5" cmd.exe /k "dotnet EchoServer\Release\net5.0\EchoServer.dll -s 0.0.0.0:50014 -b 127.0.0.1:50001 "
exit