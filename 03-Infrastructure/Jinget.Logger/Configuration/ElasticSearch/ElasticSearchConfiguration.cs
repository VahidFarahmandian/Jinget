using System;
using System.Collections.Generic;
using System.Linq;
using Elasticsearch.Net;
using Jinget.Core.Attributes;
using Jinget.Core.Exceptions;
using Jinget.Logger.Entities;
using Jinget.Logger.Entities.Log;
using Jinget.Logger.Handlers;
using Jinget.Logger.Handlers.CommandHandlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Nest;

namespace Jinget.Logger.Configuration.ElasticSearch;

public static class ElasticSearchConfiguration
{
    public static void ConfigureElasticSearchLogger<TOperationalEntity, TErrorEntity, TCustomEntity>(this IServiceCollection services, ElasticSearchSettingModel elasticSearchSetting = null)
        where TOperationalEntity : OperationLog, new()
        where TErrorEntity : ErrorLog, new()
        where TCustomEntity : CustomLog, new()
    {
        services.ConfigureJingetLoggerPrerequisites();

        services.TryAddScoped<IElasticSearchBaseDomainService<TErrorEntity>, ElasticSearchBaseDomainService<TErrorEntity>>();
        services.TryAddScoped<IElasticSearchBaseDomainService<TOperationalEntity>, ElasticSearchBaseDomainService<TOperationalEntity>>();
        services.TryAddScoped<IElasticSearchBaseDomainService<TCustomEntity>, ElasticSearchBaseDomainService<TCustomEntity>>();

        services.TryAddScoped<IElasticSearchRepository<TErrorEntity>, ElasticSearchRepository<TErrorEntity>>();
        services.TryAddScoped<IElasticSearchRepository<TOperationalEntity>, ElasticSearchRepository<TOperationalEntity>>();
        services.TryAddScoped<IElasticSearchRepository<TCustomEntity>, ElasticSearchRepository<TCustomEntity>>();

        if (elasticSearchSetting != null)
        {
            services.TryAddSingleton(elasticSearchSetting);

            var client = CreateClient(elasticSearchSetting);
            services.AddSingleton(typeof(IElasticClient), client);
            CreateIndexes(client, elasticSearchSetting);
        }
    }

    public static void ConfigureElasticSearchLogger(this IServiceCollection services, ElasticSearchSettingModel elasticSearchSetting = null)
    {
        services.ConfigureJingetLoggerPrerequisites();

        services.TryAddScoped<IElasticSearchBaseDomainService<ErrorLog>, ElasticSearchBaseDomainService<ErrorLog>>();
        services.TryAddScoped<IElasticSearchBaseDomainService<OperationLog>, ElasticSearchBaseDomainService<OperationLog>>();
        services.TryAddScoped<IElasticSearchBaseDomainService<CustomLog>, ElasticSearchBaseDomainService<CustomLog>>();

        services.TryAddScoped<IElasticSearchRepository<ErrorLog>, ElasticSearchRepository<ErrorLog>>();
        services.TryAddScoped<IElasticSearchRepository<OperationLog>, ElasticSearchRepository<OperationLog>>();
        services.TryAddScoped<IElasticSearchRepository<CustomLog>, ElasticSearchRepository<CustomLog>>();

        if (elasticSearchSetting != null)
        {
            services.TryAddSingleton(elasticSearchSetting);

            var client = CreateClient(elasticSearchSetting);
            services.AddSingleton(typeof(IElasticClient), client);
            CreateIndexes(client, elasticSearchSetting);
        }
    }

    static ElasticClient CreateClient(ElasticSearchSettingModel elasticSearchSetting)
    {
        ConnectionSettings connectionSettings;
        string protocol = elasticSearchSetting.UseSsl ? "https" : "http";
        Uri elasticUrl = new($"{protocol}://{elasticSearchSetting.Url}");
        if (string.IsNullOrEmpty(elasticSearchSetting.UserName) && string.IsNullOrEmpty(elasticSearchSetting.Password))
            connectionSettings = new ConnectionSettings(elasticUrl)
            .DefaultDisableIdInference(true);
        else
        {
            if (elasticSearchSetting.UseSsl && elasticSearchSetting.BypassCertificateValidation)
            {
                connectionSettings = new ConnectionSettings(elasticUrl)
                    .BasicAuthentication(elasticSearchSetting.UserName, elasticSearchSetting.Password)
                    .ServerCertificateValidationCallback(CertificateValidations.AllowAll)
                    .DefaultDisableIdInference(true);
            }
            else
                connectionSettings = new ConnectionSettings(elasticUrl)
                    .BasicAuthentication(elasticSearchSetting.UserName, elasticSearchSetting.Password)
                    .DefaultDisableIdInference(true);
        }

        var client = new ElasticClient(connectionSettings);

        return client;
    }

    static void CreateIndexes(ElasticClient client, ElasticSearchSettingModel elasticSearchSetting)
    {
        if (!elasticSearchSetting.CreateIndexPerPartition)
        {
            List<Type> indexes = new();
            if (elasticSearchSetting.RegisterDefaultLogModels)
            {
                indexes = typeof(BaseEntity<>).Assembly
                    .GetTypes()
                    .Where(x => x.GetCustomAttributes(typeof(EntityAttribute), false).Any(y => ((EntityAttribute)y).ElasticSearchEnabled))
                    .ToList();
            }
            if (elasticSearchSetting.DiscoveryTypes != null && elasticSearchSetting.DiscoveryTypes.Any())
            {
                foreach (var item in elasticSearchSetting.DiscoveryTypes)
                {
                    indexes.AddRange(item.Assembly.GetTypes()
                        .Where(x => x.GetCustomAttributes(typeof(EntityAttribute), false).Any(y => ((EntityAttribute)y).ElasticSearchEnabled))
                        .ToList());
                }
            }

            foreach (var item in indexes)
            {
                string indexName = $"{AppDomain.CurrentDomain.FriendlyName}.{item.Name}".ToLower();
                if (!client.Indices.Exists(indexName).Exists)
                {
                    var result = client.Indices.Create(indexName, index => index.Map(m => m.AutoMap(item).NumericDetection(true)));
                    if (!result.IsValid)
                        throw new JingetException("Jinget Says: " + result.OriginalException);
                }
            }
        }
    }
}