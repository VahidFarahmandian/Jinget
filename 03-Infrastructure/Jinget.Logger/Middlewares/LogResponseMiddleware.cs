using Jinget.Logger.Loggers;

namespace Jinget.Logger.Middlewares;

public class LogResponseMiddleware(
    RequestDelegate next,
    ILogger<LogResponseMiddleware> logger,
    IOptions<BlackListHeader> blackListHeaders,
    IOptions<WhiteListHeader> whiteListHeaders)
{
    private readonly ILog _logger = new ResponseLogger<LogResponseMiddleware>(next, logger, blackListHeaders, whiteListHeaders);

    public async Task InvokeAsync(HttpContext context) => await _logger.LogAsync(context);
}