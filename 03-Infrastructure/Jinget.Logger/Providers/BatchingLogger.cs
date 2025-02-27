namespace Jinget.Logger.Providers;

/// <summary>
/// A logger that batches log messages for efficient processing.
/// </summary>
public class BatchingLogger(BatchingLoggerProvider loggerProvider) : ILogger
{
    /// <summary>
    /// Begins a logical operation scope. (Not implemented in this logger.)
    /// </summary>
    /// <typeparam name="TState">The type of the state.</typeparam>
    /// <param name="state">The identifier for the scope.</param>
    /// <returns>An IDisposable that ends the logical operation scope.</returns>
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    /// <summary>
    /// Checks if the given log level is enabled.
    /// </summary>
    /// <param name="logLevel">The log level to check.</param>
    /// <returns>True if the log level is enabled; otherwise, false.</returns>
    public bool IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel) => logLevel != Microsoft.Extensions.Logging.LogLevel.None;

    /// <summary>
    /// Writes a log message at the specified log level.
    /// </summary>
    /// <typeparam name="TState">The type of the state.</typeparam>
    /// <param name="logLevel">The log level of the message.</param>
    /// <param name="eventId">The ID of the event.</param>
    /// <param name="state">The state of the event.</param>
    /// <param name="exception">The exception related to this message.</param>
    /// <param name="formatter">A function to create a message from the state and exception.</param>
    public void Log<TState>(Microsoft.Extensions.Logging.LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        Log(DateTime.Now, logLevel, state, exception, formatter);
    }

    /// <summary>
    /// Writes a log message with a specified timestamp.
    /// </summary>
    /// <typeparam name="TState">The type of the state.</typeparam>
    /// <param name="timestamp">The timestamp of the log message.</param>
    /// <param name="logLevel">The log level of the message.</param>
    /// <param name="state">The state of the event.</param>
    /// <param name="exception">The exception related to this message.</param>
    /// <param name="formatter">A function to create a message from the state and exception.</param>
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