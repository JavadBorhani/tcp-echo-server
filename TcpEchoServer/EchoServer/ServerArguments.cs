using CommandLine;
using Common.Utility;

namespace EchoServer
{
    class ServerArguments
    {
        [Option('s', "server", Required = true, HelpText = "server ip and port address")]
        public string Server { get; set; }

        [Option('b', "backbone", Required = true, HelpText = "backbone ip and port address")]
        public string BackboneServer { get; set; }

        [Option('l', "logLevel", Required = false, HelpText = "server log level")]
        public Logger.LogLevels LogLevel { get; set; } = Logger.LogLevels.Always;

    }
}
