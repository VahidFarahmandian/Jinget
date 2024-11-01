namespace Jinget.Logger.Configuration.File;

public class FileSettingModel : BaseSettingModel
{
    public string FileNamePrefix { get; set; } = "Log";
    public string LogDirectory { get; set; } = "Logs";
    public int RetainFileCountLimit { get; set; } = 5;
    public int FileSizeLimitMB { get; set; } = 10;
}
