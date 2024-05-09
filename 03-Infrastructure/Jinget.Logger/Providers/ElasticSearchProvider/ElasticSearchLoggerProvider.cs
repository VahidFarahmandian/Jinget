using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Jinget.Logger.Entities.Log;
using Jinget.Logger.Handlers.CommandHandlers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Jinget.Core.ExtensionMethods;

namespace Jinget.Logger.Providers.ElasticSearchProvider;

/// <summary>
///     An <see cref="ILoggerProvider" /> that writes logs
/// </summary>
[ProviderAlias("ElasticSearch")]
public class ElasticSearchLoggerProvider<TOperationalEntity, TErrorEntity, TCustomEntity> : BatchingLoggerProvider
    where TOperationalEntity : OperationLog, new()
    where TErrorEntity : ErrorLog, new()
    where TCustomEntity : CustomLog, new()
{
    private readonly IServiceProvider _serviceProvider;
    private IElasticSearchBaseDomainService<TOperationalEntity> _operationLogService;
    private IElasticSearchBaseDomainService<TErrorEntity> _errorLogService;
    private IElasticSearchBaseDomainService<TCustomEntity> _customLogService;

    public ElasticSearchLoggerProvider(
        IOptions<ElasticSearchLoggerOptions> options,
        IServiceProvider serviceProvider) : base(options) => _serviceProvider = serviceProvider;

    protected override async Task WriteMessagesAsync(IEnumerable<LogMessage> messages, CancellationToken cancellationToken)
    {
        _errorLogService = _serviceProvider.GetJingetService<IElasticSearchBaseDomainService<TErrorEntity>>();
        _operationLogService = _serviceProvider.GetJingetService<IElasticSearchBaseDomainService<TOperationalEntity>>();
        _customLogService = _serviceProvider.GetJingetService<IElasticSearchBaseDomainService<TCustomEntity>>();

        foreach (var group in messages.GroupBy(GetGrouping))
        {
            foreach (LogMessage item in group.Where(x => x.Severity == LogLevel.Information))
            {
                try
                {
                    var result = await _operationLogService.CreateAsync(JsonConvert.DeserializeObject<TOperationalEntity>(item.Description));
                }
                catch (JsonReaderException) //it means that the message is an error reporting message
                {
                }
            }

            //log errors
            foreach (var item in group.Where(x => x.Severity > LogLevel.Information))
            {
                TErrorEntity model = null;
                try
                {
                    model = JsonConvert.DeserializeObject<TErrorEntity>(item.Description);
                }
                //if the item.Description is a custom message then 
                //a JsonReaderException will be thrown because item.Description is not deserializable to TErrorEntity
                //so in this case the exception will be caught and model will be constructed manually
                catch (JsonReaderException)
                {
                    model = new TErrorEntity()
                    {
                        Description = item.Description
                    };
                }
                model.When = item.Timestamp;
                model.Severity = item.Severity.ToString();

                var result = await _errorLogService.CreateAsync(model);
            }

            //log custom data
            foreach (var item in group.Where(x => x.Severity < LogLevel.Information))
            {
                TCustomEntity model = null;
                try
                {
                    model = JsonConvert.DeserializeObject<TCustomEntity>(item.Description);
                }
                //if the item.Description is a custom message then 
                //a JsonReaderException will be thrown because item.Description is not deserializable to TErrorEntity
                //so in this case the exception will be caught and model will be constructed manually
                catch (JsonReaderException)
                {
                    model = new TCustomEntity()
                    {
                        Description = item.Description
                    };
                }
                model.SubSystem = string.IsNullOrEmpty(model.SubSystem)
                    ? AppDomain.CurrentDomain.FriendlyName
                    : model.SubSystem;
                model.When = item.Timestamp;

                var result = await _customLogService.CreateAsync(model);
            }
        }
    }

    private (int Year, int Month, int Day) GetGrouping(LogMessage message) => (message.Timestamp.Year, message.Timestamp.Month, message.Timestamp.Day);
}