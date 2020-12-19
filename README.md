### Brief Architecture

NOTE: EXPERIMENTAL IMPLEMENTATION - ONE WEEK CHALLENGE

Intro:
	This echo server and client implementation consist of three
	modules each of them is described in the following sentences

Backbone:
	Backbone is a console application and it's purpose is to
	connect all echo servers together and forward any incomming 
	message to all echo servers, basically it acts as a hub for 
	echo servers.
	
EchoServer:
	This console application will create one instance of echo server.
	At first connect to backbone, then start listening to incomming
	echo client connection, after connection establishment, if any 
	message receieved from a client, it will forward the message to 
	backbone and waits for any message to be receieved from backbone,
	eventually, it will forward any receieved message from backbone
	to all connected echo clients
	
EchoClient:
	This console application will generate multiple echo clients 
	with the amount of echo request each client should make towards 
	the connected echo server based on the command line arguments 
	passed in to application, it can be considered as 'LoadTest' 
	application for echo servers.
	
	
### Issues
	1- Half-opened and closed connections 
	2- Handling the message as events Is less performant  
	3- Backbone reconnect and buffering the incomming messages
	4- ConcurrentCollection should be replaced either with more optimized 
	   version or C# concurrent containers	

	
### How To Build

	* please note that .net 5.0 should have been installed.
	
	There are two approaches to build which is described in 
	the following parts
		 
	Build.bat:
		This file will restore the required packages and build the
		"TcpEchoServer.sln" in Release mode.
		After this batch file execution succeeded, three files 
		will be added to "Build" directory with the following names
		("Backbone", "EchoClient","EchoServer"). "Build" directory 
		is located in the "TcpEchoServer" directory
		
	VisualStudio 2019:
		Open "TcpEchoServer.sln" from TcpEchoServer folder then select
		"Release" as configuration, then click "Build Solution" from
		Build menu.
		After Build succeeded, three folders will be generated in "Build" 
		folder which is located in the "TcpEchoServer" directory with the 
		following names ("Backbone", "EchoClient","EchoServer")
		
### How To Run

	* please note that .net 5.0 should have been installed.
	
	There are two batch scripts in "Build" directory located in 
	"TcpEchoServer" folder, each of them explained below:
	
	RunServers.bat:
		This batch script at first create one instance of "Backbone"
		and then create multiple instances of "EchoServer", the ip address 
		and port number of each echo server instance and backbone 
		can be changed in batchscript.
	
	RunClient.bat:
		This batch script will start Echo client with the number of 
		actual echo client instances and number of echo message per 
		each client, the ip address of echo servers should be passsed 
		in the commandline as well.
	
	* with default configuration 1 instance of backbone and 5 instance of echo servers will be started on the local ip address
	
### Commandline Configurations

	Each module has it's own commandline configuraion which is 
	explained in the following part.
	
	
	Backbone:
		-b(required) 	"backbone IP address and port number separted by ':'" 
		-l(optional) 	"server log level which can be 'Info','Error','Alwasy'"
		
		Example :  -b 0.0.0.0:50001 -l Info
		
	EchoServer:
		-s(required)	"server IP address and port number separted by ':' "			
		-b(required) 	"backbone IP address and port number separted by ':'" 
		-l(optional) 	"server log level which can be 'Info','Error','Alwasy'"
		
		Example : -s 0.0.0.0:50010 -b 127.0.0.1:50001 -l Info

	EchoClient:
		-c(required)	"echo client count to be generated"
		-p(required)	"number of echo messages per echo client"
		-s(required)	"list of available echo servers for echo client to make connection"
		
		Example : -c 1000 -p 1 -s 127.0.0.1:50010 127.0.0.1:50011 127.0.0.1:50012

### Design Decisions 

	Protocol:
		The message protocol consist of two parts, an int32 as a header
		which is mandatory, indicating the size of actual message and 
		the message iteself which can have a dynamic length.
		
	Thoughts:
		The purpose of this test was to make echo test and I tried 
		to accomplish this with minimal protocol design, but it can
		be extented with adding additional headers like the version 
		of the message for backward compatibilty. in that regards
		A binary serializer and deserializer like MessagePack 
		could have been used instead of simple string encoding.
	
	Architecture:
		To simply describe the round-trip of echo, an echo client will
		send a message to echo server and it will be forwarded to the 
		backbone, then backbone will send it to all servers and the 
		message is going to be echoed back to the sender as well.
		
	Thoughts:
		The bottle neck of this design is "Backbone" which is the hub
		of echo servers. all messages has to be round-triped through 
		the "Backbone", this will bring single point of failure issue.
		A better design would be to break the Backbone into multiple
		backbones and connect all of them together like a full mesh 
		topology, in this way if one backbone is failed then 
		echo servers can connect to another remained backbone and 
		continue working. 
		