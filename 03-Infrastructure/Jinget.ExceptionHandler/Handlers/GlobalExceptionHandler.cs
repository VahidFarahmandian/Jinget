namespace Jinget.ExceptionHandler.Handlers;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IHostEnvironment env, BaseSettingModel baseSetting) :
    CoreExceptionHandler(logger, env, baseSetting.UseGlobalExceptionHandler), IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        => await HandleAsync(httpContext, exception, 500, cancellationToken);
}