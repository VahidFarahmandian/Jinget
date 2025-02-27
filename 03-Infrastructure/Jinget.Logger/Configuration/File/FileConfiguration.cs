using Jinget.Logger.Extensions;

namespace Jinget.Logger.Configuration.File;

/// <summary>
/// Provides extension methods for configuring file logging.
/// </summary>
public static class FileConfiguration
{
    /// <summary>
    /// Configures file logging by setting up necessary prerequisites.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure.</param>
    /// <param name="fileSetting">The file logging settings model.</param>
    public static void ConfigureFileLogger(this IServiceCollection services, FileSettingModel fileSetting) =>
        services.ConfigureJingetLoggerPrerequisites(fileSetting);
}