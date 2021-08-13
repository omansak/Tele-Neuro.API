using System;
using Microsoft.Extensions.Logging;

namespace PlayCore.Core.Logger
{
    public interface IBasicLogger : ILogger
    {
        void Log(string message);
        void Log(string message, object obj);
        void LogException(string message, Exception ex);
    }
    public interface IBasicLogger<out TCategory> : ILogger<TCategory>, IBasicLogger where TCategory : class
    {
    }
}
