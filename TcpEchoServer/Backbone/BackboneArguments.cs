using CommandLine;
using Common.Utility;

namespace Backbone
{
    class BackboneArguments
    {
        [Option('b', "backbone", Required = true, HelpText = "backbone ip and port address")]
        public string Server { get; set; }

        [Option('l', "logLevel", Required = false, HelpText = "server log level")]
        public Logger.LogLevels LogLevel { get; set; } = Logger.LogLevels.Always;
    }
}
