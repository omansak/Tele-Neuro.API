using System;
using Microsoft.Extensions.Logging;

namespace PlayCore.Core.Logger
{
    public interface IBasicLogger : ILogger
    {
        void LogBegin(string message);
        void LogEnd(string message);
        void Log(string message);
        void LogTrace(string message);
        void LogDebug(string message);
        void LogInfo(string message);
        void LogError(string message);
        void Log(string message, object obj);
        void LogTrace(string message, object obj);
        void LogInfo(string message, object obj);
        void LogError(string message, object obj);
        void LogException(string message, Exception ex);
    }
    public interface IBasicLogger<out T> : ILogger<T>, IBasicLogger where T : class
    {
    }
}
