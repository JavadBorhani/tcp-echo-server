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
        public Logger.LogLevels LogLevel { get; set; } = Logger.LogLevels.Info;
    }

    public class ClientStats
    {
        public int TotalMessageRecieved = 0;
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
                    Logger.LogLevel = clientArgs.LogLevel;

                    List<IPEndPoint> endPoints = Utils.ParseMultipleIPAddress(clientArgs.Server);
                    int clientId = 0;
                    ClientStats stats = new ClientStats();
                    client = new EchoClient(clientId, endPoints, stats);

                    await client.Start();

                    //while (true)
                    //{
                    //    await Task.Delay(1000);
                    //    Logger.Critical("received messages : {0}", stats.TotalMessageRecieved);
                    //}

                }
                catch (Exception e)
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
