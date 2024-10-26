namespace Jinget.Logger.Configuration;

public static class BaseConfiguration
{
    public static void ConfigureJingetLoggerPrerequisites(this IServiceCollection services)
    {
        services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.TryAddScoped<IExceptionHandler<LogRequestMiddleware>, ExceptionHandler<LogRequestMiddleware>>();
    }
}