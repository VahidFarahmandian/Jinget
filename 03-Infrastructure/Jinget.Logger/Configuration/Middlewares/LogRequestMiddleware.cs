namespace Jinget.Logger.Configuration.Middlewares;

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