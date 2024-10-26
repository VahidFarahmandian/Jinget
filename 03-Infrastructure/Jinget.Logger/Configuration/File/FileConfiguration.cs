namespace Jinget.Logger.Configuration.File;

public static class FileConfiguration
{
    public static void ConfigureFileLogger(this IServiceCollection services) => services.ConfigureJingetLoggerPrerequisites();
}