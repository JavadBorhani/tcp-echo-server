using System;

namespace Common.Utility
{
    public static class Logger
    {

        private static string infoPrefix = "[Info]: ";
        private static string errorPrefix = "[Error]: ";

        public static void Info(string mesasge, params object[] args)
            => Console.WriteLine(infoPrefix + mesasge, args);

        public static void Error(string message, params object[] args)
            => Console.WriteLine(errorPrefix + message, args);
    }
}
