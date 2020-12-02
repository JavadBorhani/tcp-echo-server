using System;
using System.Net;
using System.Threading;

namespace EchoClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread thread = new Thread(async () =>
            {
                var endPoint = new IPEndPoint(IPAddress.Loopback, 1111);
                TCPClient client = new TCPClient(endPoint);
                client.StartAsync();

                
                while(true)
                {
                    var input = Console.ReadLine();
                    if (input == "!")
                        break;

                    for(int i = 0; i < 10; ++i)
                        client.WriteMessage(input);
                }
            });
            thread.Start();
        }
    }
}
