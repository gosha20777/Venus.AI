using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Venus.AI.WebApi.Models.Utils
{
    public static class Log
    {
        private static string _filePatch;
        private static LogLevel _scvMinLogLevel, _consoleMinLogLevel;
        private static bool _isLogfileRady;
        private static List<string> _lines;

        public static void Initialize(string filePatch, LogLevel scvMinLogLevel, LogLevel consoleMinLogLevel)
        {
            _filePatch = filePatch;
            _scvMinLogLevel = scvMinLogLevel;
            _consoleMinLogLevel = consoleMinLogLevel;
            _lines = new List<string>();
            _isLogfileRady = File.Exists(_filePatch);
        }

        public static void Initialize()
        {
            _filePatch = Directory.GetCurrentDirectory() + "\\log.csv";
            _scvMinLogLevel = LogLevel.Information;
            _consoleMinLogLevel = LogLevel.Debug;
            _lines = new List<string>();
            _isLogfileRady = File.Exists(_filePatch);
        }

        public static void LogInformation(long userId, long taskId, string messageOwner, string message)
        {
            LogMessageAsync(LogLevel.Information, userId, taskId, messageOwner, message);
        }
        public static void LogDebug(long userId, long taskId, string messageOwner, string message)
        {
            LogMessageAsync(LogLevel.Debug, userId, taskId, messageOwner, message);
        }
        public static void LogWarning(long userId, long taskId, string messageOwner, string message)
        {
            LogMessageAsync(LogLevel.Warning, userId, taskId, messageOwner, message);
        }
        public static void LogError(long userId, long taskId, string messageOwner, string message)
        {
            LogMessageAsync(LogLevel.Error, userId, taskId, messageOwner, message);
        }
        public static void LogCritical(long userId, long taskId, string messageOwner, string message)
        {
            LogMessageAsync(LogLevel.Critical, userId, taskId, messageOwner, message);
        }

        private static async void LogMessageAsync(LogLevel logLevel, long userId, long taskId, string messageOwner, string message)
        {
            if (logLevel == LogLevel.None)
                return;
            if (logLevel >= _consoleMinLogLevel)
            {
                var time = DateTime.Now;
                Console.Write($"[{time}:{string.Format("{0:000}", time.Millisecond)}] |");
                switch (logLevel)
                {
                    case LogLevel.Critical:
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.BackgroundColor = ConsoleColor.Red;
                        break;
                    case LogLevel.Error:
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.BackgroundColor = ConsoleColor.Black;
                        break;
                    case LogLevel.Warning:
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.BackgroundColor = ConsoleColor.Black;
                        break;
                    case LogLevel.Information:
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.BackgroundColor = ConsoleColor.Black;
                        break;
                    case LogLevel.Debug:
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.BackgroundColor = ConsoleColor.Black;
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.BackgroundColor = ConsoleColor.Black;
                        break;
                }
                Console.Write($"{logLevel.ToString()} ");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.BackgroundColor = ConsoleColor.Black;
                Console.WriteLine($"|{userId} |{taskId} |{messageOwner} |{message}");
            }
            if (logLevel >= _scvMinLogLevel)
            {
                if (!_isLogfileRady)
                {
                    string header = "time,message type,user id,task id,message owner,message";
                    await File.AppendAllTextAsync(_filePatch, header + Environment.NewLine);
                    _isLogfileRady = true;
                }
                var time = DateTime.Now;
                string text = $"{time}:{string.Format("{0:000}",time.Millisecond)},{logLevel.ToString()},{userId},{taskId},{messageOwner},\"{message.Replace('\n', ' ')}\"";
                _lines.Add(text);

                if(_lines.Count > 100)
                {
                    var lines = _lines;
                    _lines.Clear();
                    await File.AppendAllLinesAsync(_filePatch, lines);
                }
            }
        }
    }
}