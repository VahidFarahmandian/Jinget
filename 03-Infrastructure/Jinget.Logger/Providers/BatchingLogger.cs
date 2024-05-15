using System;
using Microsoft.Extensions.Logging;

namespace Jinget.Logger.Providers;

public class BatchingLogger : ILogger
{
    private readonly BatchingLoggerProvider _provider;

    public BatchingLogger(BatchingLoggerProvider loggerProvider) => _provider = loggerProvider;

    public IDisposable BeginScope<TState>(TState state) => null;

    public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
        Func<TState, Exception, string> formatter)
    {
        if (!IsEnabled(logLevel))
            return;

        Log(DateTime.Now, logLevel, eventId, state, exception, formatter);
    }

    public void Log<TState>(DateTime timestamp, LogLevel logLevel, EventId eventId, TState state,
        Exception exception, Func<TState, Exception, string> formatter)
    {
        _provider.AddMessage(
            timestamp,
            new LogMessage
            {
                Description = formatter(state, exception),
                Exception = exception == null ? "" : exception.ToString(),
                Severity = logLevel,
                Timestamp = timestamp
            });
    }
}