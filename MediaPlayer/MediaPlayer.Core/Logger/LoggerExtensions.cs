using System.Collections.Generic;
using NLog;

namespace MediaPlayer.Core.Logger
{
    public static class LoggerExtensions
    {
        public static void Info<T>(this IList<ILogger> loggers, T value)
        {
            foreach (var logger in loggers)
                logger.Info(value);
        }

        public static void Debug<T>(this IList<ILogger> loggers, T value)
        {
            foreach (var logger in loggers)
                logger.Debug(value);
        }

        public static void Error<T>(this IList<ILogger> loggers, T value)
        {
            foreach (var logger in loggers)
                logger.Error(value);
        }

        public static void Fatal<T>(this IList<ILogger> loggers, T value)
        {
            foreach (var logger in loggers)
                logger.Fatal(value);
        }

        public static void Trace<T>(this IList<ILogger> loggers, T value)
        {
            foreach (var logger in loggers)
                logger.Trace(value);
        }

        public static void Warn<T>(this IList<ILogger> loggers, T value)
        {
            foreach (var logger in loggers)
                logger.Warn(value);
        }
    }
}