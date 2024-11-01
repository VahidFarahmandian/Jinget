using Jinget.ExceptionHandler.Extensions;
using Jinget.Logger.Middlewares;

namespace Jinget.Logger.Extensions;

public static class IApplicationBuilderExtensions
{
    /// <summary>
    /// Add logging required middlewares to pipeline
    /// </summary>
    public static IApplicationBuilder UseJingetLogging(this IApplicationBuilder app)
    {
        app.UseJingetExceptionHandler();
        app.UseMiddleware<LogRequestMiddleware>();
        app.UseMiddleware<LogResponseMiddleware>();

        return app;
    }
}