namespace Jinget.Logger.Providers;

public class BatchingLogger(BatchingLoggerProvider loggerProvider) : ILogger
{
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel) => logLevel != Microsoft.Extensions.Logging.LogLevel.None;

    public void Log<TState>(Microsoft.Extensions.Logging.LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
            return;

        Log(DateTime.Now, logLevel, state, exception, formatter);
    }

    public void Log<TState>(DateTime timestamp, Microsoft.Extensions.Logging.LogLevel logLevel, TState state,
        Exception? exception, Func<TState, Exception?, string> formatter) => loggerProvider.AddMessage(
            new LogMessage
            {
                Description = formatter(state, exception),
                Exception = exception == null ? "" : exception.ToString(),
                Severity = logLevel,
                Timestamp = timestamp
            });
}