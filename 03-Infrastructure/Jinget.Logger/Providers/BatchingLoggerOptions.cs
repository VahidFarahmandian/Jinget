using Microsoft.Extensions.Logging;
using System;

namespace Jinget.Logger.Providers
{
    public class BatchingLoggerOptions
    {
        private int? _backgroundQueueSize;
        private int? _batchSize = 32;
        private string[] _blacklistStrings = Array.Empty<string>();
        private TimeSpan _flushPeriod = TimeSpan.FromSeconds(1);
        private LogLevel[] allowedLogLevels;

        /// <summary>
        /// Defines what log levels should processed
        /// Defaults to <c>Trace, Debug, Information, Warning, Error, Critical</c>
        /// </summary>
        public LogLevel[] AllowedLogLevels
        {
            get => allowedLogLevels;
            set => allowedLogLevels = value ?? new LogLevel[] {
                LogLevel.Trace,
                LogLevel.Debug,
                LogLevel.Information,
                LogLevel.Warning,
                LogLevel.Error,
                LogLevel.Critical
            };
        }

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
            set => _blacklistStrings = value ?? Array.Empty<string>();
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
}