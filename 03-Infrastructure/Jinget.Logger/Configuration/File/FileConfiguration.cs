using Microsoft.Extensions.DependencyInjection;

namespace Jinget.Logger.Configuration;

public static class FileConfiguration
{
    public static void ConfigureFileLogger(this IServiceCollection services) => services.ConfigureJingetLoggerPrerequisites();
}