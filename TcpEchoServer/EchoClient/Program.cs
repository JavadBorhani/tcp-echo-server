using CommandLine;
using Common.Utility;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace EchoClient
{
    class ClientArguments
    {
        [Option('s', "servers", Required = true, HelpText = "list of server endpoints to connect")]
        public IEnumerable<string> Server { get; set; }

        [Option('m', "message", Required = false, HelpText = "echo message")]
        public string EchoMessage { get; set; } = "echo";

        [Option('l', "logLevel", Required = false, HelpText = "server log level")]
        public Logger.LogLevels LogLevel { get; set; } = Logger.LogLevels.Critical;
    }

    
    class Program
    {
        static void Main(string[] args)
        {
            ClientArguments clientArgs = Utils.ReadArguments<ClientArguments>(args);

            Thread thread = new Thread(async () =>
            {
                EchoClient client = null;
                try
                {

                    List<IPEndPoint> endPoints = Utils.ParseMultipleIPAddress(clientArgs.Server);

                    IPEndPointProvider endPointProvider = new IPEndPointProvider(endPoints);

                    client = new EchoClient(endPointProvider);

                    client.Start();
                    
                    Logger.Critical("waiting 3 seconds");
                    await Task.Delay(3 * 1000);
                    Logger.Critical("sending requests...");

                    string echo = "echo";
                    int numOfMessages = 10000;
                    for (int i = 0; i < numOfMessages; ++i)
                        client.WriteMessage(echo).NoAwait();

                    Logger.Critical($"{numOfMessages} sent");

                    while (true)
                    {
                        await Task.Delay(1000);
                        Logger.Critical("received messages : {0}", client.TotalReceivedMessages);
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
