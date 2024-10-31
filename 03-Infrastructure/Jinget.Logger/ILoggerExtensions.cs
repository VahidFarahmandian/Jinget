namespace Jinget.Logger;

public static class ILoggerExtensions
{
    /// <summary>
    /// generates a custom log. This log's severity is <seealso cref="Microsoft.Extensions.Logging.LogLevel.Debug"/>
    /// </summary>
    public static void LogCustom(this ILogger logger, string message,
        // Guid? requestId = null,
        string traceIdentifier = null,
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] long callerLineNumber = 0,
        [CallerMemberName] string callerMember = "")
        => logger.LogDebug(JsonConvert.SerializeObject(new LogModel
        {
            Description = message,
            CallerFilePath = callerFilePath,
            CallerLineNumber = callerLineNumber,
            CallerMember = callerMember,
            TraceIdentifier = traceIdentifier // ?? Guid.Empty
        }), null);
}