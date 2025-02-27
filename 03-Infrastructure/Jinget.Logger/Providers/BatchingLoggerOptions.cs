namespace Jinget.Logger.Providers;

public class BatchingLoggerOptions
{
    private int? _backgroundQueueSize;
    private int? _batchSize = 32;
    private string[] _blacklistStrings = [];
    private string[]? _blacklistUrls = [];
    private TimeSpan _flushPeriod = TimeSpan.FromSeconds(1);

    const long MB_10 = 1024 * 1024 * 10;

    /// <summary>
    /// maximum request body size to log. 
    /// request body larger than this value will be logged as `--REQUEST BODY TOO LARGE--` string
    /// </summary>
    public long MaxRequestBodySize { get; set; } = MB_10;

    /// <summary>
    /// maximum response body size to log. 
    /// response body larger than this value will be logged as `--REQUEST BODY TOO LARGE--` string
    /// </summary>
    public long MaxResponseBodySize { get; set; } = MB_10;

    /// <summary>
    /// Defines the min log level that should processed
    /// Defaults to <seealso cref="Microsoft.Extensions.Logging.LogLevel.Error"/>
    /// </summary>
    public Microsoft.Extensions.Logging.LogLevel MinAllowedLogLevel { get; set; } = Microsoft.Extensions.Logging.LogLevel.Information;

    /// <summary>
    ///     Gets or sets the period after which logs will be flushed to the store.
    /// </summary>
    public TimeSpan FlushPeriod
    {
        get => _flushPeriod;
        set
        {
            if (value <= TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(value), $"Jinget Says: {nameof(FlushPeriod)} must be positive.");
            _flushPeriod = value;
        }
    }

    /// <summary>
    ///     Gets or sets the blacklist string. reuqest/responses contain the blacklist array items will not logged.
    ///     Defaults to <c>empty</c>, so every thing will be logged
    /// </summary>
    public string[] BlackListStrings
    {
        get => _blacklistStrings;
        set => _blacklistStrings = value ?? [];
    }

    /// <summary>
    ///     Gets or sets the blacklist string. reuqest/responses contain the blacklist array items will not logged.
    ///     Defaults to <c>empty</c>, so every thing will be logged
    /// </summary>
    public string[]? BlackListUrls
    {
        get => _blacklistUrls;
        set => _blacklistUrls = value ?? [];
    }

    /// <summary>
    ///     Gets or sets the maximum size of the background log message queue or null for no limit.
    ///     After maximum queue size is reached log event sink would start blocking.
    ///     Defaults to <c>null</c>.
    /// </summary>
    public int? BackgroundQueueSize
    {
        get => _backgroundQueueSize;
        set
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), $"Jinget Says: {nameof(BackgroundQueueSize)} must be non-negative.");
            _backgroundQueueSize = value;
        }
    }

    /// <summary>
    ///     Gets or sets a maximum number of events to include in a single batch or null for no limit.
    /// </summary>
    public int? BatchSize
    {
        get => _batchSize;
        set
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(nameof(value), $"Jinget Says: {nameof(BatchSize)} must be positive.");
            _batchSize = value;
        }
    }

    /// <summary>
    ///     Gets or sets value indicating if logger accepts and queues writes.
    /// </summary>
    public bool IsEnabled { get; set; }
}