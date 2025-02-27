using Jinget.ExceptionHandler.Extensions;

namespace Jinget.Logger.Extensions;

/// <summary>
/// Provides extension methods for <see cref="IServiceCollection"/> to configure Jinget logger prerequisites.
/// </summary>
public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Configures the necessary prerequisites for the Jinget logger, including HTTP context access and exception handling.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure.</param>
    /// <param name="baseSetting">Optional base setting model for exception handling configuration.</param>
    public static void ConfigureJingetLoggerPrerequisites(this IServiceCollection services, BaseSettingModel? baseSetting)
    {
        // Adds IHttpContextAccessor as a singleton if it hasn't been added already.
        services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        // Configures Jinget exception handling if base settings are provided.
        if (baseSetting != null)
        {
            services.ConfigureJingetExceptionHandler(baseSetting);
        }
    }
}