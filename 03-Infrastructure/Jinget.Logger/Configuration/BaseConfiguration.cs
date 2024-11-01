using Jinget.ExceptionHandler.Entities;
using Jinget.ExceptionHandler.Handlers;

namespace Jinget.Logger.Configuration;

public static class BaseConfiguration
{
    public static void ConfigureJingetLoggerPrerequisites(this IServiceCollection services, BaseSettingModel baseSetting)
    {
        services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        if (baseSetting.UseGlobalExceptionHandler)
        {
            services.TryAddSingleton(baseSetting);
            services.AddProblemDetails();

            if (baseSetting.Handle4xxResponses)
                services.AddExceptionHandler<HttpRequestExceptionHandler>();

            services.AddExceptionHandler<GlobalExceptionHandler>();
        }
    }
}