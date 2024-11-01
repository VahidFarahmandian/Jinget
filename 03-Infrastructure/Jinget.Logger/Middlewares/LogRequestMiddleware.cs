using Jinget.Logger.Loggers;

namespace Jinget.Logger.Middlewares;

public class LogRequestMiddleware(
    RequestDelegate next,
    ILogger<LogRequestMiddleware> logger,
    IOptions<BlackListHeader> blackListHeaders,
    IOptions<WhiteListHeader> whiteListHeaders)
{
    private readonly ILog _logger = new RequestLogger<LogRequestMiddleware>(
        next,
        logger,
        blackListHeaders,
        whiteListHeaders);

    public async Task InvokeAsync(HttpContext context) => await _logger.LogAsync(context);
}