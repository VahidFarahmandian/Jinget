using Jinget.Core.Utilities.Json;
using System.Text.Json;
using System.Text.Json.Nodes;

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
                        log = item.Description.Deserialize<LogModel>(strictPropertyMatching: true);

                        if (log != null && !string.IsNullOrWhiteSpace(log.Body) && JsonUtility.IsValid(log.Body))
                        {
                            // Parse the JSON string
                            JsonNode? rootNode = JsonNode.Parse(log.Body);

                            // Check if the parsed node is a JSON object
                            if (rootNode != null && rootNode is JsonObject rootObject)
                            {
                                // Check if the "isSuccess" property exists and is a boolean
                                if (rootObject.TryGetPropertyValue("isSuccess", out JsonNode? isSuccessNode) &&
                                    isSuccessNode != null && isSuccessNode is JsonValue isSuccessValue &&
                                    isSuccessValue.TryGetValue(out bool isSuccess))
                                {
                                    if (!isSuccess)
                                        item.Severity = Microsoft.Extensions.Logging.LogLevel.Error;
                                }
                            }
                        }
                    }
                }
                catch (JsonException) //it means that the message is an error message(not error log)
                {
                    log = LogModel.GetNewErrorObject();
                    log.AdditionalData = new
                    {
                        item.Description,
                        item.Exception
                    }.Serialize();
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