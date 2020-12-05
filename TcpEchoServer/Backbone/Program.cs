using CommandLine;
using Common.Utility;
using System;
using System.Net;
using System.Threading;

namespace Backbone
{
    class BackboneArguments
    {
        [Option('b', "backbone", Required = true, HelpText = "backbone ip and port address")]
        public string Server { get; set; }

        [Option('l', "logLevel", Required = false, HelpText = "server log level")]
        public Logger.LogLevels LogLevel { get; set; } = Logger.LogLevels.ForceLog;
    }

    class Program
    {
        static void Main(string[] args)
        {
            BackboneArguments backboneArgs = Utils.ReadArguments<BackboneArguments>(args);

            Thread thread = new Thread(() =>
            {
                BackboneServer server = null;
                try
                {
                    Logger.LogLevel = backboneArgs.LogLevel;
                    IPEndPoint backboneIp = Utils.ParseIPAddress(backboneArgs.Server);

                    server = new BackboneServer(backboneIp);
                    server.Start();

                    Logger.ForceLog("Backbone started on {0} address", backboneIp.ToString());
                    Logger.ForceLog("Press any key to exit...");

                    Console.ReadLine();
                }
                catch(Exception ex)
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
