using Jinget.Logger.Configuration.Middlewares;
using Microsoft.AspNetCore.Builder;

namespace Jinget.Logger.Configuration;

public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Add logging required middlewares to pipeline
    /// </summary>
    public static IApplicationBuilder UseJingetLogging(this IApplicationBuilder app)
    {
        app.UseMiddleware<LogResponseMiddleware>();
        app.UseMiddleware<LogRequestMiddleware>();

        return app;
    }
}