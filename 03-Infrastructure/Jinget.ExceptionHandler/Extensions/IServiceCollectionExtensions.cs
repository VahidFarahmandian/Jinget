namespace Jinget.ExceptionHandler.Extensions;

public static class IServiceCollectionExtensions
{
    public static void ConfigureJingetExceptionHandler(this IServiceCollection services, BaseSettingModel baseSetting)
    {
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