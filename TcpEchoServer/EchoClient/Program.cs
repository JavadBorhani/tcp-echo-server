using CommandLine;
using Common.Utility;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace EchoClient
{

    class ClientArguments
    {
        [Option('s', "server", Required = true, HelpText = "server endpoint to connect")]
        public string Server { get; set; }

        [Option('l', "logLevel", Required = false, HelpText = "server log level")]
        public Logger.LogLevels LogLevel { get; set; } = Logger.LogLevels.ForceLog;
    }


    class Program
    {
        static void Main(string[] args)
        {
            var clientArgs = Utils.ReadArguments<ClientArguments>(args);

            Thread thread = new Thread(async () =>
            {
                TCPClient client = null;
                try
                {
                    IPEndPoint endPoint = Utils.ParseIPAddress(clientArgs.Server);
                    client = new TCPClient(endPoint);
                    await client.StartAsync();
                    
                    Logger.ForceLog("waiting 3 seconds");
                    await Task.Delay(10000);
                    Logger.ForceLog("sending requests...");

                    string echo = "echo";
                    int numOfMessages = 10000;
                    for (int i = 0; i < numOfMessages; ++i)
                        client.WriteMessage(echo).NoAwait();

                    Logger.ForceLog($"{numOfMessages} sent");
                    while (true)
                    {
                        await Task.Delay(1000);
                        Logger.ForceLog("received messages : {0}", client.TotalReceivedMessages);
                    }
                    
                }
                catch(Exception e)
                {
                    client?.Dispose();
                    Logger.Error(e.StackTrace);
                }
            });
            thread.Start();
            Console.ReadLine();
        }
    }
}
