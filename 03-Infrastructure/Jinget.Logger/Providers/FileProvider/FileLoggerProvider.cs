﻿namespace Jinget.Logger.Providers.FileProvider;

/// <summary>
///     An <see cref="ILoggerProvider" /> that writes logs
/// </summary>
[ProviderAlias("File")]
public class FileLoggerProvider : BatchingLoggerProvider
{
    private readonly string _fileName;
    private readonly long? _maxFileSize;
    private readonly int? _maxRetainedFiles;
    private readonly string _path;

    /// <summary>
    ///     Creates an instance of the <see cref="FileLoggerProvider" />
    /// </summary>
    /// <param name="options">The options object controlling the logger</param>
    public FileLoggerProvider(IOptions<FileLoggerOptions> options) : base(options)
    {
        var loggerOptions = options.Value;
        _path = loggerOptions.LogDirectory;
        _fileName = loggerOptions.FileName;
        _maxFileSize = loggerOptions.FileSizeLimit * 1024 * 1024;
        _maxRetainedFiles = loggerOptions.RetainedFileCountLimit;
    }

    /// <inheritdoc />
    protected override async Task WriteMessagesAsync(IEnumerable<LogMessage> messages,
        CancellationToken cancellationToken)
    {
        if (!Directory.Exists(_path)) Directory.CreateDirectory(_path);

        foreach (var group in messages.GroupBy(GetGrouping))
        {
            var fullName = GetFullName(group.Key);
            var fileInfo = new FileInfo(fullName);
            if (_maxFileSize.HasValue &&
                _maxFileSize > 0 &&
                fileInfo.Exists &&
                fileInfo.Length > _maxFileSize)
                return;

            using var streamWriter = File.AppendText(fullName);
            foreach (var item in group)
                await streamWriter.WriteAsync(item.ToString());
        }

        RollFiles();
    }

    private string GetFullName((int Year, int Month, int Day) group) => Path.Combine(_path, $"{_fileName}{@group.Year:0000}{@group.Month:00}{@group.Day:00}.txt");

    private (int Year, int Month, int Day) GetGrouping(LogMessage message) => (message.Timestamp.Year, message.Timestamp.Month, message.Timestamp.Day);

    /// <summary>
    ///     Deletes old log files, keeping a number of files defined by <see cref="FileLoggerOptions.RetainedFileCountLimit" />
    /// </summary>
    protected void RollFiles()
    {
        if (_maxRetainedFiles > 0)
        {
            var files = new DirectoryInfo(_path)
                .GetFiles(_fileName + "*")
                .OrderByDescending(f => f.Name)
                .Skip(_maxRetainedFiles.Value);

            foreach (var item in files) item.Delete();
        }
    }
}