using CommandLine;
using Common.Utility;
using System.Collections.Generic;

namespace EchoClient
{
    class ClientArguments
    {
        [Option('s', "servers", Required = true, HelpText = "list of server endpoints to connect")]
        public IEnumerable<string> Server { get; set; }

        [Option('c', "clientCount", Required = true, HelpText = "echo client counts to generate")]
        public int ClientCount { get; set; }

        [Option('p', "messagePerClient", Required = true, HelpText = "message each echo client will send")]
        public int MessagePerClient { get; set; }

        [Option('m', "message", Required = false, HelpText = "echo message")]
        public string EchoMessage { get; set; } = "echo";

        [Option('l', "logLevel", Required = false, HelpText = "server log level")]
        public Logger.LogLevels LogLevel { get; set; } = Logger.LogLevels.Always;

    }
}
