echo start
sleep 2
start "Clients" cmd.exe /k "dotnet EchoClient\Release\net5.0\EchoClient.dll -c 1000 -p 1 -s 127.0.0.1:50010 127.0.0.1:50011 127.0.0.1:50012 127.0.0.1:50013 127.0.0.1:50014"
exit