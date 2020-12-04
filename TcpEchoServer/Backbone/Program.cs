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
            var arguments = Utils.ReadCommandLineArguments<BackboneArguments>(args);

            Console.WriteLine("Server is running...");

            Thread t = new Thread(() =>
            {
                BackboneServer server = new BackboneServer(arguments.BackboneIP);
                server.Start();
            });
            t.Start();
        }
    }
}
