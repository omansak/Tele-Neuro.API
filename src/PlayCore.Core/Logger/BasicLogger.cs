using Microsoft.Extensions.Logging;
using PlayCore.Core.Extension;
using System;
using System.IO;
using System.Threading;

namespace PlayCore.Core.Logger
{
    public class BasicLogger<TCategory> : IBasicLogger<TCategory> where TCategory : class
    {
        private readonly BasicLoggerConfiguration _configuration;
        private string Path => _configuration.Directory + "/" + _configuration.FileName + "." + _configuration.Extension;
        private static readonly ReaderWriterLockSlim ReadWriteLock = new();
        public BasicLogger(BasicLoggerConfiguration configuration)
        {
            _configuration = configuration;
            if (!Directory.Exists(_configuration.Directory))
            {
                Directory.CreateDirectory(_configuration.Directory);
            }
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            ReadWriteLock.EnterWriteLock();
            try
            {
                using (TextWriter textWriter = TextWriter.Synchronized(File.AppendText(Path)))
                {
                    textWriter.WriteLine($"[{DateTime.Now}] [{logLevel}] [{typeof(TCategory).Name}] : {formatter(state, exception)}");
                }
            }
            finally
            {
                ReadWriteLock.ExitWriteLock();
            }
        }
        public void Log(string message)
        {
            Log<string>(LogLevel.None, 0, null, null, (s, exception) => message);
        }
        public void Log(string message, object obj)
        {
            Log<string>(LogLevel.Error, 0, null, null, (s, exception) => new { Message = message, Object = obj }.ToJson());
        }
        public void LogException(string message, Exception ex)
        {
            Log(message, ex);
        }
        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state) // TODO
        {
            return null;
        }
    }
    public class BasicLogger : BasicLogger<BasicLogger>, IBasicLogger
    {
        public BasicLogger(BasicLoggerConfiguration configuration) : base(configuration)
        {

        }
    }

    public class BasicLoggerConfiguration
    {
        public string Directory { get; set; }
        public string FileName { get; set; }
        public string Extension => "log";
    }
}
