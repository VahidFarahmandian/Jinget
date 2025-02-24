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
        var settings = app.ApplicationServices.GetJingetService<BaseSettingModel>();

        if (settings != null && settings.UseGlobalExceptionHandler)
            app.UseJingetExceptionHandler();

        app.UseMiddleware<RequestResponseLoggingMiddleware>();
        //app.UseMiddleware<LogRequestMiddleware>();
        //app.UseMiddleware<LogResponseMiddleware>();

        return app;
    }
}