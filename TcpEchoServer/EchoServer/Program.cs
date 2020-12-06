using CommandLine;
using Common.Utility;
using System;
using System.Net;
using System.Threading;

namespace EchoServer
{
    class ServerArguments
    {
        [Option('s', "server", Required = true, HelpText = "server ip and port address")]
        public string Server { get; set; }

        [Option('b', "backbone", Required = true, HelpText = "backbone ip and port address")]
        public string BackboneServer { get; set; }

        [Option('l', "logLevel", Required = false, HelpText = "server log level")]
        public Logger.LogLevels LogLevel { get; set; } = Logger.LogLevels.Critical;

    }

    class Program
    {
        static void Main(string[] args)
        {
            ServerArguments serverArgs = Utils.ReadArguments<ServerArguments>(args);

            Thread thread = new Thread(() =>
            {
                EchoServer server = null;
                try
                {
                    Logger.LogLevel = serverArgs.LogLevel;

                    IPEndPoint serverIp = Utils.ParseIPAddress(serverArgs.Server);
                    IPEndPoint backboneIp = Utils.ParseIPAddress(serverArgs.BackboneServer);

                    server = new EchoServer(serverIp, backboneIp);
                    server.Start();

                    Logger.Critical("Server started on {0} address", serverIp.ToString());
                    Logger.Critical("Press any key to exit...");

                    Console.ReadLine();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message);
                    Console.ReadLine();
                }
                finally
                {
                    server?.Stop();
                }
            });
            thread.Start();
        }
    }
}
