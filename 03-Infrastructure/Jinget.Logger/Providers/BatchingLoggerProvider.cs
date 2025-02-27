using Microsoft.VisualStudio.Threading;
using System.Text.Json;

namespace Jinget.Logger.Providers;

/// <summary>
/// Abstract base class for logger providers that batch log messages.
/// </summary>
public abstract class BatchingLoggerProvider : ILoggerProvider
{
    private readonly Microsoft.Extensions.Logging.LogLevel _minAllowedLogLevel;
    private readonly int? _batchSize;
    private readonly string[] _blacklistStrings;
    private readonly string[] _blacklistUrls;
    private readonly List<LogMessage> _currentBatch = [];
    private readonly TimeSpan _interval;
    private readonly int? _queueSize;
    private CancellationTokenSource? _cancellationTokenSource;

    private BlockingCollection<LogMessage>? _messageQueue;
    private Task? _outputTask;

    /// <summary>
    /// Initializes a new instance of the <see cref="BatchingLoggerProvider"/> class.
    /// </summary>
    /// <param name="options">The options for batching logger.</param>
    protected BatchingLoggerProvider(IOptions<BatchingLoggerOptions> options)
    {
        var loggerOptions = options.Value;

        if (loggerOptions.BatchSize <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(loggerOptions.BatchSize), $"Jinget Says: {nameof(loggerOptions.BatchSize)} must be a positive number.");
        }

        if (loggerOptions.FlushPeriod <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(loggerOptions.FlushPeriod), $"Jinget Says: {nameof(loggerOptions.FlushPeriod)} must be longer than zero.");
        }

        _interval = loggerOptions.FlushPeriod;
        _batchSize = loggerOptions.BatchSize;
        _queueSize = loggerOptions.BackgroundQueueSize;
        _blacklistStrings = loggerOptions.BlackListStrings.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.ToLower()).ToArray() ?? [];
        _blacklistUrls = loggerOptions.BlackListUrls?.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.ToLower()).ToArray() ?? [];
        _minAllowedLogLevel = loggerOptions.MinAllowedLogLevel;

        Start();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        StopSynchronous();
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    public ILogger CreateLogger(string categoryName) => new BatchingLogger(this);

    /// <summary>
    /// Writes the log messages asynchronously.
    /// </summary>
    /// <param name="messages">The log messages to write.</param>
    /// <param name="token">A cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected abstract Task WriteMessagesAsync(IEnumerable<LogMessage> messages, CancellationToken token);

    private async Task ProcessLogQueueAsync()
    {
        if (_cancellationTokenSource != null && _messageQueue != null)
        {
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                var limit = _batchSize ?? int.MaxValue;

                while (limit > 0 && _messageQueue.TryTake(out var message))
                {
                    _currentBatch.Add(message);
                    limit--;
                }

                if (_currentBatch.Count > 0)
                {
                    try
                    {
                        await WriteMessagesAsync(_currentBatch, _cancellationTokenSource.Token);
                    }
                    catch
                    {
                        // Ignored
                    }

                    _currentBatch.Clear();
                }

                await IntervalAsync(_interval, _cancellationTokenSource.Token);
            }
        }
    }

    /// <summary>
    /// Waits for the specified interval or until cancellation is requested.
    /// </summary>
    /// <param name="interval">The interval to wait.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected virtual Task IntervalAsync(TimeSpan interval, CancellationToken cancellationToken) => Task.Delay(interval, cancellationToken);

    internal void AddMessage(LogMessage message)
    {
        if (message.Severity < _minAllowedLogLevel)
        {
            return;
        }

        if (_blacklistStrings.Any(message.ToString().Contains))
        {
            return;
        }

        try
        {
            var jsonMessage = message.ToString()[message.ToString().IndexOf("{")..];
            var jsonUrl = JsonDocument.Parse(jsonMessage)
                .RootElement
                .EnumerateObject()
                .FirstOrDefault(x => string.Equals(x.Name, "Url", StringComparison.OrdinalIgnoreCase));

            var jsonPageUrl = JsonDocument.Parse(jsonMessage)
                .RootElement
                .EnumerateObject()
                .FirstOrDefault(x => string.Equals(x.Name, "PageUrl", StringComparison.OrdinalIgnoreCase));

            if (_blacklistUrls.Any(jsonUrl.ToString().Contains) || _blacklistUrls.Any(jsonPageUrl.ToString().Contains))
            {
                return;
            }
        }
        catch
        {
            // Ignored
        }

        if (_cancellationTokenSource != null && _messageQueue != null)
        {
            if (!_messageQueue.IsAddingCompleted)
            {
                try
                {
                    _messageQueue.Add(message, _cancellationTokenSource.Token);
                }
                catch
                {
                    // Cancellation token canceled or CompleteAdding called
                }
            }
        }
    }

    private void Start()
    {
        _messageQueue = _queueSize == null
            ? new BlockingCollection<LogMessage>(new ConcurrentQueue<LogMessage>())
            : new BlockingCollection<LogMessage>(new ConcurrentQueue<LogMessage>(), _queueSize.Value);

        _cancellationTokenSource = new CancellationTokenSource();
        _outputTask = Task.Factory.StartNew(
            ProcessLogQueueAsync,
            _cancellationTokenSource.Token,
            TaskCreationOptions.LongRunning,
            TaskScheduler.Default);
    }

    private void StopSynchronous()
    {
        _cancellationTokenSource?.Cancel();
        _messageQueue?.CompleteAdding();

        using var taskContext = new JoinableTaskContext();
        var joinableTaskFactory = new JoinableTaskFactory(taskContext);
        joinableTaskFactory.Run(async () =>
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            await _outputTask?.WaitAsync(_interval);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        });
    }
}