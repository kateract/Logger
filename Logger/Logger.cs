using System;
using System.Collections.Generic;
using System.Linq;


namespace LogHost
{
    public static class Logger
    {

        private static int number = 1;
        private static readonly List<LogHandler> handlers;
        public static void Log(LogLevel level, object message, params object[] args) 
            => Log(new LogData() { Level = level, Message = string.Format(message.ToString(), args) });

        public static void Log(LogData data)
        {
            handlers.ForEach(handler => 
            {
                if ((data.Level & handler.Level) == data.Level)
                {
                    try
                    {
                        handler.Handler(data);
                    }
                    catch
                    {
                        //hide all exceptions from the logger
                    }
                }
            });
        }

        public static int Subscribe(LogEventHandler func, LogLevel level)
        {

            handlers.Add(new LogHandler() { ID = ++number, Level = level, Handler = func });
            return number;
        }

        public static void Unsubscribe(int identifier) => handlers.Remove(handlers.First(h => h.ID == identifier));

    }

    public delegate void LogEventHandler(LogData message);

    [Flags]
    public enum LogLevel
    {
        FAIL = 1,
        WARN = 2,
        INFO = 4,
        NORMAL = 7,
        DEBUG = 8,
        ALL = 15
    }
    public struct LogData
    {
        public string Message { get; set; }
        public LogLevel Level { get; set; }
        public override string ToString() => Level.ToString() + ": " + Message;
    }
    public struct LogHandler
    {
        public int ID { get; set; }
        public LogLevel Level { get; set; }
        public LogEventHandler Handler { get; set; }
    }
}