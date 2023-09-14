using System;
using System.Runtime.CompilerServices;
using Jinget.Logger.Entities.Log;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Jinget.Logger
{
    public static class ILoggerExtensions
    {
        public static void LogCustom(this ILogger logger, string message,
            Guid? requestId = null,
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] long callerLineNumber = 0,
            [CallerMemberName] string callerMember = "")
        {
            logger.LogDebug(JsonConvert.SerializeObject(new CustomLog
            {
                Description = message,
                CallerFilePath = callerFilePath,
                CallerLineNumber = callerLineNumber,
                CallerMember = callerMember,
                RequestId = requestId ?? Guid.Empty
            }), null);
        }
    }
}
