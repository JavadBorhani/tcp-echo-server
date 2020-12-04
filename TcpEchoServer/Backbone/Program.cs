using CommandLine;
using Common.Utility;
using System;
using System.Net;
using System.Threading;

namespace Backbone
{

    public class BackboneArguments
    {
        [Option()]
        public IPEndPoint BackboneIP { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            //BackboneArguments arguments = Utils.ReadCommandLineArguments<BackboneArguments>(args);

            

            Thread thread = new Thread(() =>
            {
                IPEndPoint backboneIp = new IPEndPoint(IPAddress.Parse("0.0.0.0"), 50000);

                BackboneServer server = new BackboneServer(backboneIp);
                server.Start().NoAwait();

                Console.WriteLine("Backbone is running on {0} ...",  backboneIp.ToString());
                Console.ReadLine();
            });

            thread.Start();
        }
    }
}
