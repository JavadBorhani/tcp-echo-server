using CommandLine;
using Common.Utility;
using System.Collections.Generic;

namespace EchoClient
{
    class ClientArguments
    {
        [Option('s', "servers", Required = true, HelpText = "list of server endpoints to connect")]
        public IEnumerable<string> Server { get; set; }

        [Option('m', "message", Required = false, HelpText = "echo message")]
        public string EchoMessage { get; set; } = "echo";

        [Option('l', "logLevel", Required = false, HelpText = "server log level")]
        public Logger.LogLevels LogLevel { get; set; } = Logger.LogLevels.Always;
    }
}
