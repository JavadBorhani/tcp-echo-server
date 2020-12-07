using Common.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace EchoClient
{
    public class ClientStats
    {
        public int TotalMessageSent = 0;
        public int TotalMessageRecieved = 0;
        public int TotalExpectedReceivedMessages = 0;
        public int TotalExpectedSendMessages = 0;
        public int ClientCounts = 0;
        public int MessagePerClient = 0;

        public void PrintStats()
        {
            Logger.Always("ClientCounts                  : {0}", ClientCounts);
            Logger.Always("MessagePerClient              : {0}", MessagePerClient);
            Logger.Always("TotalExpectedSendMessages     : {0}", TotalExpectedSendMessages);
            Logger.Always("TotalMessageSent              : {0}", TotalMessageSent);
            Logger.Always("TotalExpectedReceivedMessages : {0}", TotalExpectedReceivedMessages);
            Logger.Always("TotalMessageRecieved          : {0}", TotalMessageRecieved);
        }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            ClientArguments clientArgs = Utils.ReadArguments<ClientArguments>(args);

            Logger.LogLevel = clientArgs.LogLevel;

            List<IPEndPoint> endPoints = Utils.ParseMultipleIPAddress(clientArgs.Server);

            ClientStats stats = new ClientStats();
            stats.ClientCounts = clientArgs.ClientCount;
            stats.TotalExpectedSendMessages = clientArgs.ClientCount * clientArgs.MessagePerClient;
            stats.TotalExpectedReceivedMessages = clientArgs.ClientCount * clientArgs.ClientCount * clientArgs.MessagePerClient;
            stats.MessagePerClient = clientArgs.MessagePerClient;

            Stopwatch stopwatch = Stopwatch.StartNew();

            List<Task> clients = new List<Task>();

            for (int i = 0; i < clientArgs.ClientCount; ++i)
                clients.Add(CreateNewEchoClient(i, endPoints, stats, clientArgs.MessagePerClient, clientArgs.EchoMessage));

            Logger.Always("Sending message after 3 seconds");
            CancellationTokenSource cts = new CancellationTokenSource();
            Task t = Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(1000);
                    Logger.Always("");

                    stats.PrintStats();
                    Logger.Always("ElapsedTime: {0}", stopwatch.Elapsed);

                    cts.Token.ThrowIfCancellationRequested();
                }
            });

            await Task.WhenAll(clients);
            cts.Cancel();
            Console.ReadLine();
        }

        public static async Task CreateNewEchoClient
        (
            int clientId,
            List<IPEndPoint> endPoints,
            ClientStats clientStats,
            int messageCount,
            string message
        )
        {
            EchoClient client = null;
            try
            {
                client = new EchoClient(clientId, endPoints, clientStats);
                await client.Start();

                //wait that all clients created
                await Task.Delay(3 * 1000);

                List<Task> sentMessages = new List<Task>();
                for (int i = 0; i < messageCount; ++i)
                {
                    await Task.Delay(10);
                    sentMessages.Add(client.WriteMessage(message));
                }
                   
                await Task.WhenAll(sentMessages);

                while(clientStats.TotalMessageRecieved < clientStats.TotalExpectedReceivedMessages)
                {
                    await Task.Delay(2000);
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message);
            }
            finally
            {
                client.Stop();
            }

        }

    }
}
