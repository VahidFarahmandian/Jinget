using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Jinget.Logger;

public static class ILoggerExtensions
{
    /// <summary>
    /// generates a custom log. This log's severity is <seealso cref="LogLevel.Debug"/>
    /// </summary>
    public static void LogCustom(this ILogger logger, HttpContext? httpContext, string message,
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] long callerLineNumber = 0,
        [CallerMemberName] string callerMember = "")
    {
        if (httpContext == null)
            return;
        var log = LogModel.GetNewCustomObject(httpContext);
        log.Description = message;
        log.CallerFilePath = callerFilePath;
        log.CallerLineNumber = callerLineNumber;
        log.CallerMember = callerMember;

        logger.Log(LogLevel.Information, JsonConvert.SerializeObject(log));
    }

    public static void LogInformation(this ILogger logger, HttpContext? httpContext, string message)
    {
        if (httpContext == null)
            return;
        var log = LogModel.GetNewCustomObject(httpContext);
        log.Description = message;

        logger.Log(LogLevel.Information, JsonConvert.SerializeObject(log));
    }

    public static void LogError(this ILogger logger, HttpContext? httpContext, string message, Exception? exception = null)
    {
        if (httpContext == null)
            return;
        var log = LogModel.GetNewErrorObject(httpContext);
        log.AdditionalData = JsonConvert.SerializeObject(new
        {
            description = message,
            exception
        });
        logger.Log(LogLevel.Error, JsonConvert.SerializeObject(log));
    }
}