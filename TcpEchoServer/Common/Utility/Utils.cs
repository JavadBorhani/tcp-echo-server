using CommandLine;
using System;
using System.Threading.Tasks;

namespace Common.Utility
{
    public static class Utils
    {
        public static T ReadCommandLineArguments<T>(string[] args) where T : new()
        {
            ParserResult<T> result = Parser.Default.ParseArguments<T>(args);
            T arguments = new T();
            if (result.Tag == ParserResultType.Parsed)
                arguments = ((Parsed<T>)result).Value;
            return arguments;
        }

        public static void NoAwait(this Task task) { }

    }
}
