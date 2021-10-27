using PlayCore.Core.Extension;
using System;
using System.IO;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace PlayCore.Core.Logger
{
    public class BasicLogger<T> : IBasicLogger<T> where T : class
    {
        private static readonly ReaderWriterLockSlim ReadWriteLock = new();
        private LogLevel MinLogLevel => _configuration.LogLevel;
        private readonly BasicLoggerConfiguration _configuration;
        private string Path => _configuration.Directory + "/" + _configuration.FileName + "." + _configuration.Extension;
        private string BackupDirectory => _configuration.Directory + "backup\\";

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
                if ((int)logLevel >= (int)MinLogLevel)
                {
                    if (File.Exists(Path))
                    {
                        //Rotate Log Files
                        if (new FileInfo(Path).Length > _configuration.MaxFileLength)
                        {
                            if (!Directory.Exists(BackupDirectory))
                            {
                                Directory.CreateDirectory(BackupDirectory);
                            }

                            string newFileName = BackupDirectory + _configuration.FileName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + _configuration.Extension;

                            File.Move(Path, newFileName);
                        }
                    }

                    using (TextWriter textWriter = TextWriter.Synchronized(new StreamWriter(Path, true)))
                    {
                        textWriter.WriteLine($"[{DateTime.Now}] [{logLevel}] [{typeof(T).Name}] : {formatter(state, exception)}");
                    }
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
        public void LogBegin(string message)
        {
            Log<string>(LogLevel.None, 0, null, null, (s, exception) => $"----------------------------------BEGIN----------------------------------");
            Log<string>(LogLevel.None, 0, null, null, (s, exception) => message);
        }
        public void LogEnd(string message)
        {
            Log<string>(LogLevel.None, 0, null, null, (s, exception) => message);
            Log<string>(LogLevel.None, 0, null, null, (s, exception) => $"----------------------------------END----------------------------------");
        }
        public void LogTrace(string message)
        {
            Log<string>(LogLevel.Trace, 0, null, null, (s, exception) => message);
        }
        public void LogDebug(string message)
        {
            Log<string>(LogLevel.Debug, 0, null, null, (s, exception) => message);
        }
        public void LogInfo(string message)
        {
            Log<string>(LogLevel.Information, 0, null, null, (s, exception) => message);
        }
        public void LogError(string message)
        {
            Log<string>(LogLevel.Error, 0, null, null, (s, exception) => message);
        }
        public void LogException(string message, Exception ex)
        {
            Log<string>(LogLevel.Critical, 0, null, null, (s, exception) => new { Message = message, Object = ex }.ToJson());
        }
        public void Log(string message, object obj)
        {
            Log<string>(LogLevel.None, 0, null, null, (s, exception) => new { Message = message, Object = obj }.ToJson());
        }
        public void LogTrace(string message, object obj)
        {
            Log<string>(LogLevel.Trace, 0, null, null, (s, exception) => new { Message = message, Object = obj }.ToJson());
        }
        public void LogInfo(string message, object obj)
        {
            Log<string>(LogLevel.Information, 0, null, null, (s, exception) => new { Message = message, Object = obj }.ToJson());
        }
        public void LogError(string message, object obj)
        {
            Log<string>(LogLevel.Error, 0, null, null, (s, exception) => new { Message = message, Object = obj }.ToJson());
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return (int)logLevel > (int)_configuration.LogLevel;
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
        public LogLevel LogLevel { get; set; }
        public string Directory { get; set; }
        public string FileName { get; set; }
        public long MaxFileLength { get; set; } = 25 * 1024 * 1024;
        public string Extension => ".log";

    }
}
