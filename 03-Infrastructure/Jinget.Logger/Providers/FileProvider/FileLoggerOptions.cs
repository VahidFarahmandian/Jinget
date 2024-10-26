﻿namespace Jinget.Logger.Providers.FileProvider;

/// <summary>
///     Options for file logging.
/// </summary>
public class FileLoggerOptions : BatchingLoggerOptions
{
    private string _fileName = "logs-";
    private int? _fileSizeLimit = 10;
    private int? _retainedFileCountLimit = 2;

    /// <summary>
    ///     Gets or sets a strictly positive value representing the maximum log size in MB or null for no limit.
    ///     Once the log is full, no more messages will be appended.
    ///     Defaults to <c>10MB</c>.
    /// </summary>
    public int? FileSizeLimit
    {
        get => _fileSizeLimit;
        set
        {
            if (value.HasValue && value <= 0)
                throw new ArgumentOutOfRangeException(nameof(value), $"Jinget Says: {nameof(FileSizeLimit)} must be positive.");
            _fileSizeLimit = value;
        }
    }

    /// <summary>
    ///     Gets or sets a strictly positive value representing the maximum retained file count or null for no limit.
    ///     Defaults to <c>2</c>.
    /// </summary>
    public int? RetainedFileCountLimit
    {
        get => _retainedFileCountLimit;
        set
        {
            if (value.HasValue && value <= 0)
                throw new ArgumentOutOfRangeException(nameof(value),
                    $"Jinget Says: {nameof(RetainedFileCountLimit)} must be positive.");
            _retainedFileCountLimit = value;
        }
    }

    /// <summary>
    ///     Gets or sets the filename prefix to use for log files.
    ///     Defaults to <c>logs-</c>.
    /// </summary>
    public string FileName
    {
        get => _fileName;
        set
        {
            if (string.IsNullOrEmpty(value)) throw new ArgumentException("Jinget Says: " + nameof(value));
            _fileName = value;
        }
    }

    /// <summary>
    ///     The directory in which log files will be written, relative to the app process.
    ///     Default to <c>Logs</c>
    /// </summary>
    /// <returns></returns>
    public string LogDirectory { get; set; } = "Logs";
}