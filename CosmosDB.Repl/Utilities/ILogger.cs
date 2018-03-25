using System;

namespace CosmosDB.Repl.Utilities
{
    public enum LogLevel
    {
        All,
        Info,
        Warning,
        Error
    }
    public interface ILogger
    {
        void Verbose(string message);
        void Info(string message);
        void Warning(string message);
        void Error(string message, Exception ex);
    }
}