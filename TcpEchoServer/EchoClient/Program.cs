using Common.Utility;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace EchoClient
{
    class Program
    {
        static void Main(string[] args)
        {
            TCPClient client = null;
            try
            {
                Thread thread = new Thread(() =>
                {
                    var endPoint = new IPEndPoint(IPAddress.Loopback, 1111);
                    client = new TCPClient(endPoint);
                    Task.Run(() => client.StartAsync());

                    while (true)
                    {
                        var input = Console.ReadLine();
                        if (input == "!")
                            break;

                        Task.Run(() => client.WriteMessage(input));
                    }
                });
                thread.Start();
            }
            catch (Exception e)
            {
                client?.Dispose();
                Logger.Error(e.StackTrace);
            }
        }
    }
}
