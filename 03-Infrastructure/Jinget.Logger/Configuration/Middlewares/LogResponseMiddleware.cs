namespace Jinget.Logger.Configuration.Middlewares;

public class LogResponseMiddleware(
    RequestDelegate next,
    ILogger<LogResponseMiddleware> logger,
    IOptions<BlackListHeader> blackListHeaders,
    IOptions<WhiteListHeader> whiteListHeaders)
{
    private readonly ILog _logger = new ResponseLogger<LogResponseMiddleware>(next, logger, blackListHeaders, whiteListHeaders);

    public async Task InvokeAsync(HttpContext context) => await _logger.LogAsync(context);
}