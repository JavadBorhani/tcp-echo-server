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
            Thread thread = new Thread(() =>
            {
                TCPServer server = null;
                try
                {
                    IPEndPoint serverIp = new IPEndPoint(IPAddress.Parse("0.0.0.0"), 1111);
                    server = new TCPServer(serverIp, serverIp);
                    server.StartAsync();

                    Console.WriteLine("Server start on {0} address \nPress any key to exit...", serverIp.ToString());
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
