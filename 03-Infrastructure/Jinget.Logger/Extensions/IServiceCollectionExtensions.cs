using Jinget.ExceptionHandler.Extensions;

namespace Jinget.Logger.Extensions;

public static class IServiceCollectionExtensions
{
    public static void ConfigureJingetLoggerPrerequisites(this IServiceCollection services, BaseSettingModel baseSetting)
    {
        services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.ConfigureJingetExceptionHandler(baseSetting);
    }
}