namespace Jinget.ExceptionHandler.Handlers;

public sealed class HttpRequestExceptionHandler
    (ILogger<GlobalExceptionHandler> logger, IHostEnvironment env, BaseSettingModel baseSetting) : CoreExceptionHandler(logger, env, baseSetting.UseGlobalExceptionHandler), IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        => exception is HttpRequestException httpRequestException
            ? await HandleAsync(httpContext, httpRequestException, (int)httpRequestException.StatusCode, cancellationToken)
            : false;
    protected override ProblemDetails CreateProblemDetails(in HttpContext context, in Exception exception, int statusCode)
    {
        var problemDetails = base.CreateProblemDetails(context, exception, statusCode);
        problemDetails.Extensions.Add("url", context.Request.GetDisplayUrl());
        return problemDetails;
    }
}