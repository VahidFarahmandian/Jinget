namespace Jinget.Logger.Providers.ElasticSearchProvider;

/// <summary>
///     An <see cref="ILoggerProvider" /> that writes logs
/// </summary>
[ProviderAlias("ElasticSearch")]
public class ElasticSearchLoggerProvider : BatchingLoggerProvider
{
    private readonly IServiceProvider _serviceProvider;
    private IElasticSearchLoggingDomainService _logService;

    public ElasticSearchLoggerProvider(
        IOptions<ElasticSearchLoggerOptions> options,
        IServiceProvider serviceProvider) : base(options) => _serviceProvider = serviceProvider;

    protected override async Task WriteMessagesAsync(IEnumerable<LogMessage> messages, CancellationToken cancellationToken)
    {
        _logService = _serviceProvider.GetJingetService<IElasticSearchLoggingDomainService>();


        foreach (var group in messages.GroupBy(GetGrouping))
        {
            foreach (LogMessage item in group)
            {
                LogModel log = null;
                try
                {
                    log = JsonConvert.DeserializeObject<LogModel>(item.Description);
                }
                catch (JsonReaderException) //it means that the message is an error reporting message
                {
                    log = new LogModel
                    {
                        Description = JsonConvert.SerializeObject(new
                        {
                            item.Description,
                            item.Exception
                        })
                    };
                }
                finally
                {
                    log.TimeStamp = item.Timestamp;
                    log.Severity = item.Severity.ToString();
                    var result = await _logService.CreateAsync(log);
                }
            }
        }
    }

    private (int Year, int Month, int Day, int hour) GetGrouping(LogMessage message)
        => (message.Timestamp.Year, message.Timestamp.Month, message.Timestamp.Day, message.Timestamp.Hour);
}