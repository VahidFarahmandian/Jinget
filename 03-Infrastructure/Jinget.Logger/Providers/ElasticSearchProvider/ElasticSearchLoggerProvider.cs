namespace Jinget.Logger.Providers.ElasticSearchProvider;

/// <summary>
///     An <see cref="ILoggerProvider" /> that writes logs
/// </summary>
[ProviderAlias("ElasticSearch")]
public class ElasticSearchLoggerProvider(IServiceProvider serviceProvider, IOptions<ElasticSearchLoggerOptions> options) : BatchingLoggerProvider(options)
{
    protected override async Task WriteMessagesAsync(IEnumerable<LogMessage> messages,
        CancellationToken cancellationToken)
    {
        var _logService = serviceProvider.GetJingetService<IElasticSearchLoggingDomainService>();
        if (_logService == null) return;
        foreach (var group in messages.GroupBy(GetGrouping))
        {
            foreach (var item in group)
            {
                var log = LogModel.GetNew();
                try
                {
                    if (!string.IsNullOrWhiteSpace(item.Description))
                    {
                        log = JsonConvert.DeserializeObject<LogModel>(item.Description);
                    }
                }
                catch (JsonReaderException) //it means that the message is an error message(not error log)
                {
                    log = LogModel.GetNewErrorObject();
                    log.AdditionalData = JsonConvert.SerializeObject(new
                    {
                        item.Description,
                        item.Exception
                    });
                }
                finally
                {
                    if (log != null)
                    {
                        log.TimeStamp = item.Timestamp;
                        log.Severity = item.Severity.ToString();
                        await _logService.CreateAsync(log);
                    }
                }
            }
        }
    }

    private (int Year, int Month, int Day, int hour) GetGrouping(LogMessage message)
        => (message.Timestamp.Year, message.Timestamp.Month, message.Timestamp.Day, message.Timestamp.Hour);
}