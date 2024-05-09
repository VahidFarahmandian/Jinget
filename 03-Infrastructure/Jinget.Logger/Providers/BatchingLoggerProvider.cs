using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Jinget.Logger.Providers;

public abstract class BatchingLoggerProvider : ILoggerProvider
{
    private readonly LogLevel[] _allowedLogLevels;
    private readonly int? _batchSize;
    private readonly string[] _blacklistStrings;
    private readonly List<LogMessage> _currentBatch = new();
    private readonly TimeSpan _interval;
    private readonly int? _queueSize;
    private CancellationTokenSource _cancellationTokenSource;

    private BlockingCollection<LogMessage> _messageQueue;
    private Task _outputTask;

    protected BatchingLoggerProvider(IOptions<BatchingLoggerOptions> options)
    {
        // NOTE: Only IsEnabled is monitored

        var loggerOptions = options.Value;
        if (loggerOptions.BatchSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(loggerOptions.BatchSize), $"Jinget Says: {nameof(loggerOptions.BatchSize)} must be a positive number.");
        if (loggerOptions.FlushPeriod <= TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(loggerOptions.FlushPeriod), $"Jinget Says: {nameof(loggerOptions.FlushPeriod)} must be longer than zero.");

        _interval = loggerOptions.FlushPeriod;
        _batchSize = loggerOptions.BatchSize;
        _queueSize = loggerOptions.BackgroundQueueSize;
        _blacklistStrings = loggerOptions.BlackListStrings ?? Array.Empty<string>();
        _allowedLogLevels = loggerOptions.AllowedLogLevels ?? new LogLevel[] {
            LogLevel.Trace,
            LogLevel.Debug,
            LogLevel.Information,
            LogLevel.Warning,
            LogLevel.Error,
            LogLevel.Critical
        };

        Start();
    }

    public void Dispose()
    {
        StopAsync().GetAwaiter().GetResult();
        GC.SuppressFinalize(this);
    }

    public ILogger CreateLogger(string categoryName) => new BatchingLogger(this);

    protected abstract Task WriteMessagesAsync(IEnumerable<LogMessage> messages, CancellationToken token);

    private async Task ProcessLogQueueAsync()
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
                    // ignored
                }

                _currentBatch.Clear();
            }

            await IntervalAsync(_interval, _cancellationTokenSource.Token);
        }
    }

    protected virtual Task IntervalAsync(TimeSpan interval, CancellationToken cancellationToken) => Task.Delay(interval, cancellationToken);

    internal void AddMessage(DateTimeOffset timestamp, LogMessage message)
    {
        //if log severity level is not allowed then ignore it
        if (!_allowedLogLevels.Contains(message.Severity))
            return;

        //if log contains blacklist string then ignore it
        if (_blacklistStrings.Any(message.ToString().Contains))
            return;

        if (!_messageQueue.IsAddingCompleted)
            try
            {
                _messageQueue.Add(message, _cancellationTokenSource.Token);
            }
            catch
            {
                //cancellation token canceled or CompleteAdding called
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

    private async Task StopAsync()
    {
        _cancellationTokenSource.Cancel();
        _messageQueue.CompleteAdding();

        try
        {
            await _outputTask.WaitAsync(_interval);
        }
        catch (TaskCanceledException)
        {
        }
        catch (AggregateException ex) when (ex.InnerExceptions.Count == 1 &&
                                            ex.InnerExceptions[0] is TaskCanceledException)
        {
        }
    }
}