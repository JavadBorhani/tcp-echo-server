using Common;
using Common.Utility;
using System;
using System.Net;
using System.Threading;

namespace EchoServer
{

    class Program
    {
        static void Main(string[] args)
        {
            Thread thread = new Thread(async () =>
            {
                TCPServer server = null;
                try
                {
                    IPEndPoint serverIp = new IPEndPoint(IPAddress.Parse("0.0.0.0"), 1111);
                    server = new TCPServer(serverIp, serverIp);
                    await server.StartAsync();

                    Console.WriteLine("Server start on {0} address", serverIp.ToString());
                    Console.WriteLine("Press any key to exit...");
                    Console.ReadLine();
                }
                catch (Exception exception)
                {
                    
                    Logger.Error(exception.Message);
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
