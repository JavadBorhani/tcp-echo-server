using CommandLine;
using Common.Utility;
using System;
using System.Net;
using System.Threading;

namespace EchoServer
{
    public class ServerArguments
    {
        [Option('s', "server", Required = true, HelpText = "this server ip and port address")]
        public string Server { get; set; }

        [Option('b', "backbone", Required = true, HelpText = "backbone ip and port address")]
        public string BackboneServer { get; set; }

        [Option('l', "logLevel", Required = false, HelpText = "server log level")]
        public Logger.LogLevels LogLevel { get; set; } = Logger.LogLevels.ForceLog;

    }

    class Program
    {
        static void Main(string[] args)
        {
            //args = new string[] { "-s 0.0.0.0:1111", "-b 127.0.0.1:2222", "-l Error" };
            
            ServerArguments serverArgs = Utils.ReadArguments<ServerArguments>(args);

            Thread thread = new Thread(async () =>
            {
                TCPServer server = null;
                try
                {
                    Logger.LogLevel = serverArgs.LogLevel;

                    IPEndPoint serverIp = Utils.ReadIPAddress(serverArgs.Server);
                    IPEndPoint backbone = Utils.ReadIPAddress(serverArgs.BackboneServer);

                    server = new TCPServer(serverIp, backbone);
                    await server.StartAsync();

                    Logger.ForceLog("Server start on {0} address", serverIp.ToString());
                    Logger.ForceLog("Press any key to exit...");

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
