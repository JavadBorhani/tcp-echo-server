using System;

namespace Common.Utility
{
    public static class Logger
    {
        public enum LogLevels
        {
            Info,
            Error,
            ForceLog
        };

        public static LogLevels LogLevel = LogLevels.ForceLog;
        private static string infoPrefix = "[Info]: ";
        private static string errorPrefix = "[Error]: ";

        public static void Info(string mesasge, params object[] args)
        {
            if (LogLevel == LogLevels.Info)
                Console.WriteLine(infoPrefix + mesasge, args);
        }

        public static void Error(string message, params object[] args)
        {
            if (LogLevel == LogLevels.Info || LogLevel == LogLevels.Error)
                Console.WriteLine(errorPrefix + message, args);
        }

        public static void ForceLog(string message, params object[] args)
        {
            if (LogLevel == LogLevels.Info  || 
                LogLevel == LogLevels.Error || 
                LogLevel == LogLevels.ForceLog)
                Console.WriteLine(message, args);
        }

    }
}
