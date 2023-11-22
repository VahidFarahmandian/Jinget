using System;
using System.Threading.Tasks;
using Jinget.Core.IOptionTypes.Log;
using Jinget.Logger.ExceptionHandler;
using Jinget.Logger.Members;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Jinget.Core.ExtensionMethods;

namespace Jinget.Logger.Configuration.Middlewares
{
    public class LogRequestMiddleware
    {
        private readonly ILog _logger;

        public LogRequestMiddleware(RequestDelegate next, IServiceProvider serviceProvider, ILogger<LogRequestMiddleware> logger,
            IOptions<BlackListHeader> blackListHeaders,
            IOptions<WhiteListHeader> whiteListHeaders)
            => _logger =
            new RequestLogger<LogRequestMiddleware>(next, logger, serviceProvider.GetJingetService<IExceptionHandler<LogRequestMiddleware>>(), blackListHeaders, whiteListHeaders);

        public async Task InvokeAsync(HttpContext context) => await _logger.LogAsync(context);
    }
}