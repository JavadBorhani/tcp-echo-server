﻿using Common.Utility;
using System;
using System.Net;
using System.Threading;

namespace EchoServer
{
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

                    Logger.Always("Server started on {0} address", serverIp.ToString());
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
