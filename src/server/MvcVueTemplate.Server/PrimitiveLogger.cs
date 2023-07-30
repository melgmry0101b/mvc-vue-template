/*-----------------------------------------------------------------*\
 *
 * PrimitiveLogger.cs
 *   MvcVueTemplate.Server
 *     mvc-vue-template
 *
 * See LICENSE at root directory
 *
 * CREATED: 2023-7-28 09:54 PM
 * AUTHORS: Mohammed Elghamry <elghamry.connect[at]outlook[dot]com>
 *
\*-----------------------------------------------------------------*/

using System.Diagnostics;
using System;

namespace MvcVueTemplate.Server
{
    /// <summary>
    /// Types for primitive logs.
    /// </summary>
    public enum PrimitiveLoggerLogType
    {
        Information,
        Warning,
        Error
    }

    /// <summary>
    /// Providing a primitive logger to be used when no advanced logger is available.
    /// </summary>
    public static class PrimitiveLogger
    {
        private static void WriteLog(string message)
        {
            Console.Write(message);
            Debug.Write(message);
        }

        /// <summary>
        /// Write log message.
        /// </summary>
        /// <param name="logType">Log type.</param>
        /// <param name="message">Log message.</param>
        public static void Log(PrimitiveLoggerLogType logType, string message)
        {
            var currentConsoleBackgroundColor = Console.BackgroundColor;
            var currentConsoleForegroundColor = Console.ForegroundColor;

            WriteLog($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff zzz} ");
            WriteLog("[");

            Console.ForegroundColor = ConsoleColor.White;
            switch (logType)
            {
                case PrimitiveLoggerLogType.Information:
                    Console.BackgroundColor = ConsoleColor.Blue;
                    WriteLog("Info");
                    break;

                case PrimitiveLoggerLogType.Warning:
                    Console.BackgroundColor = ConsoleColor.Yellow;
                    WriteLog("Warning");
                    break;

                case PrimitiveLoggerLogType.Error:
                    Console.BackgroundColor = ConsoleColor.Red;
                    WriteLog("Error");
                    break;
            }

            Console.ForegroundColor = currentConsoleForegroundColor;
            Console.BackgroundColor = currentConsoleBackgroundColor;
            WriteLog("]: ");

            WriteLog(message);

            WriteLog(Environment.NewLine);
        }

        /// <summary>
        /// Log information message.
        /// </summary>
        /// <param name="message">Log message.</param>
        public static void LogInformation(string message) => Log(PrimitiveLoggerLogType.Information, message);

        /// <summary>
        /// Log warning message.
        /// </summary>
        /// <param name="message">Log message.</param>
        public static void LogWarning(string message) => Log(PrimitiveLoggerLogType.Warning, message);

        /// <summary>
        /// Log error message.
        /// </summary>
        /// <param name="message">Log message.</param>
        public static void LogError(string message) => Log(PrimitiveLoggerLogType.Error, message);
    }
}
