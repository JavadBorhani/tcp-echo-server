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
            Thread thread = new Thread(async () =>
            {
                TCPClient client = null;
                try
                {
                    var endPoint = new IPEndPoint(IPAddress.Loopback, 50001);
                    client = new TCPClient(endPoint);
                    await client.StartAsync();

                    //while (true)
                    //{
                    //    var input = Console.ReadLine();
                    //    if (input == "!")
                    //        break;

                    //    
                    //}
                    Logger.ForceLog("waiting 3 seconds");
                    Thread.Sleep(3000);
                    Logger.ForceLog("sending requests...");

                    string echo = "echo";
                    int numOfMessages = 1000000;
                    for (int i = 0; i < numOfMessages; ++i)
                        client.WriteMessage(echo).NoAwait();

                    Logger.ForceLog($"{numOfMessages} sent");
                    while (true)
                    {
                        await Task.Delay(1000);
                        Logger.ForceLog("received messages : {0}", client.TotalReceivedMessages);
                    }
                }
                catch(Exception e)
                {
                    client?.Dispose();
                    Logger.Error(e.StackTrace);
                }
            });
            thread.Start();
            Console.WriteLine("thread finished");
            Console.ReadLine();
        }
    }
}
