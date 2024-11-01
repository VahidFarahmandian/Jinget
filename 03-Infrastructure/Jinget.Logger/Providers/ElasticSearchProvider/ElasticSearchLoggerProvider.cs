namespace Jinget.Logger.Providers.ElasticSearchProvider;

/// <summary>
///     An <see cref="ILoggerProvider" /> that writes logs
/// </summary>
[ProviderAlias("ElasticSearch")]
public class ElasticSearchLoggerProvider : BatchingLoggerProvider
{
    private readonly IServiceProvider _serviceProvider;

    public ElasticSearchLoggerProvider(
        IOptions<ElasticSearchLoggerOptions> options,
        IServiceProvider serviceProvider) : base(options)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task WriteMessagesAsync(IEnumerable<LogMessage> messages,
        CancellationToken cancellationToken)
    {
        var _logService = _serviceProvider.GetJingetService<IElasticSearchLoggingDomainService>();

        foreach (var group in messages.GroupBy(GetGrouping))
        {
            foreach (var item in group)
            {
                var log = LogModel.GetNew(null);
                try
                {
                    log = JsonConvert.DeserializeObject<LogModel>(item.Description);
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
                    log.TimeStamp = item.Timestamp;
                    log.Severity = item.Severity.ToString();
                    await _logService.CreateAsync(log);
                }
            }
        }
    }

    private (int Year, int Month, int Day, int hour) GetGrouping(LogMessage message)
        => (message.Timestamp.Year, message.Timestamp.Month, message.Timestamp.Day, message.Timestamp.Hour);
}