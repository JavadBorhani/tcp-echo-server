using System;

namespace Common.Utility
{
    public static class Logger
    {
        public enum LogLevels
        {
            Info,
            Error,
            Always
        };

        public static LogLevels LogLevel = LogLevels.Always;
        private static string infoPrefix = "[Info]: ";
        private static string errorPrefix = "[Error]: ";

        public static void Info(string mesasge, params object[] args)
        {
            if (LogLevel == LogLevels.Info)
                Console.WriteLine(infoPrefix + mesasge, args);
        }

        public static void Error(string message, params object[] args)
        {
            if (LogLevel == LogLevels.Error || LogLevel == LogLevels.Info)
                Console.WriteLine(errorPrefix + message, args);
        }

        public static void Always(string message, params object[] args)
        {
            Console.WriteLine(message, args);
        }

    }
}
