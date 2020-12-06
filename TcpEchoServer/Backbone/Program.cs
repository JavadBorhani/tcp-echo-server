using Common.Utility;
using System;
using System.Net;
using System.Threading;

namespace Backbone
{

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

                    Logger.Always("Backbone started on {0} address", backboneIp.ToString());
                    Logger.Always("Press any key to exit...");

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
