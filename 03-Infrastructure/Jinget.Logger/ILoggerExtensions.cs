namespace Jinget.Logger;

public static class ILoggerExtensions
{
    /// <summary>
    /// generates a custom log. This log's severity is <seealso cref="Microsoft.Extensions.Logging.LogLevel.Debug"/>
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="message"></param>
    /// <param name="requestId"></param>
    /// <param name="callerFilePath"></param>
    /// <param name="callerLineNumber"></param>
    /// <param name="callerMember"></param>
    public static void LogCustom(this ILogger logger, string message,
        Guid? requestId = null,
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] long callerLineNumber = 0,
        [CallerMemberName] string callerMember = "")
        => logger.LogDebug(JsonConvert.SerializeObject(new LogModel
        {
            Description = message,
            CallerFilePath = callerFilePath,
            CallerLineNumber = callerLineNumber,
            CallerMember = callerMember,
            RequestId = requestId ?? Guid.Empty
        }), null);
}
