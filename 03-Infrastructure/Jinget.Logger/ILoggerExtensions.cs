using Jinget.ExceptionHandler.Entities.Log;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Jinget.Logger;

public static class ILoggerExtensions
{
    /// <summary>
    /// generates a custom log. This log's severity is <seealso cref="Microsoft.Extensions.Logging.LogLevel.Debug"/>
    /// </summary>
    public static void LogCustom(this ILogger logger, HttpContext httpContext, string message,
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] long callerLineNumber = 0,
        [CallerMemberName] string callerMember = "")
    {
        var log = LogModel.GetNewCustomObject(httpContext);
        log.Description = message;
        log.CallerFilePath = callerFilePath;
        log.CallerLineNumber = callerLineNumber;
        log.CallerMember = callerMember;

        logger.Log(LogLevel.Information, JsonConvert.SerializeObject(log));
    }

    public static void LogInformation(this ILogger logger, HttpContext httpContext, string message)
    {
        var log = LogModel.GetNewCustomObject(httpContext);
        log.Description = message;

        logger.Log(LogLevel.Information, JsonConvert.SerializeObject(log));
    }

    public static void LogError(this ILogger logger, HttpContext httpContext, string message,
        Exception exception = null)
    {
        var log = LogModel.GetNewErrorObject(httpContext);
        log.AdditionalData = JsonConvert.SerializeObject(new
        {
            description = message,
            exception
        });
        logger.Log(LogLevel.Error, JsonConvert.SerializeObject(log));
    }
}