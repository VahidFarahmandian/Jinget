namespace Jinget.Logger.Configuration.File;

/// <summary>
/// Represents the settings model for file logging.
/// </summary>
public class FileSettingModel : BaseSettingModel
{
    /// <summary>
    /// Gets or sets the prefix for the log file names. Defaults to "Log".
    /// </summary>
    public string FileNamePrefix { get; set; } = "Log";

    /// <summary>
    /// Gets or sets the directory where log files are stored. Defaults to "Logs".
    /// </summary>
    public string LogDirectory { get; set; } = "Logs";

    /// <summary>
    /// Gets or sets the maximum number of log files to retain. Defaults to 5.
    /// </summary>
    public int RetainFileCountLimit { get; set; } = 5;

    /// <summary>
    /// Gets or sets the maximum size of a log file in megabytes. Defaults to 10.
    /// </summary>
    public int FileSizeLimitMB { get; set; } = 10;
}