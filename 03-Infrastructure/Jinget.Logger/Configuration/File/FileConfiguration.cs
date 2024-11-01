using Jinget.Logger.Extensions;

namespace Jinget.Logger.Configuration.File;

public static class FileConfiguration
{
    public static void ConfigureFileLogger(this IServiceCollection services, FileSettingModel fileSetting) => services.ConfigureJingetLoggerPrerequisites(fileSetting);
}