using System;
using System.Collections.Generic;
using System.Linq;
using Jinget.Core.Attributes;
using Jinget.Core.Exceptions;
using Jinget.Logger.Entities;
using Jinget.Logger.Entities.Log;
using Jinget.Logger.Handlers;
using Jinget.Logger.Handlers.CommandHandlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Nest;

namespace Jinget.Logger.Configuration.Middlewares.ElasticSearch
{
    public static class ElasticSearchConfiguration
    {
        public static void ConfigureElasticSearchLogger<TOperationalEntity, TErrorEntity, TCustomEntity>(this IServiceCollection services, ElasticSearchSettingModel elasticSearchSetting = null)
            where TOperationalEntity : OperationLog, new()
            where TErrorEntity : ErrorLog, new()
            where TCustomEntity : CustomLog, new()
        {
            services.ConfigurePrerequisites();

            services.TryAddScoped<IElasticSearchBaseDomainService<TErrorEntity, int>, ElasticSearchBaseDomainService<TErrorEntity, int>>();
            services.TryAddScoped<IElasticSearchBaseDomainService<TOperationalEntity, int>, ElasticSearchBaseDomainService<TOperationalEntity, int>>();
            services.TryAddScoped<IElasticSearchBaseDomainService<TCustomEntity, int>, ElasticSearchBaseDomainService<TCustomEntity, int>>();

            services.TryAddScoped<IElasticSearchRepository<TErrorEntity, int>, ElasticSearchRepository<TErrorEntity, int>>();
            services.TryAddScoped<IElasticSearchRepository<TOperationalEntity, int>, ElasticSearchRepository<TOperationalEntity, int>>();
            services.TryAddScoped<IElasticSearchRepository<TCustomEntity, int>, ElasticSearchRepository<TCustomEntity, int>>();

            if (elasticSearchSetting != null)
            {
                var client = CreateClient(elasticSearchSetting);
                services.AddSingleton(typeof(IElasticClient), client);
                CreateIndexes(client, elasticSearchSetting);
            }
        }

        static ElasticClient CreateClient(ElasticSearchSettingModel elasticSearchSetting)
        {
            Uri elasticUrl;
            if (string.IsNullOrEmpty(elasticSearchSetting.UserName) && string.IsNullOrEmpty(elasticSearchSetting.Password))
                elasticUrl = new Uri(elasticSearchSetting.Url);
            else
            {
                string protocol = elasticSearchSetting.UseSsl ? "https" : "http";
                elasticUrl =
                    new Uri(
                        $"{protocol}://{elasticSearchSetting.UserName}:{elasticSearchSetting.Password}@{elasticSearchSetting.Url}");
            }

            ConnectionSettings settings = new ConnectionSettings(elasticUrl).DefaultDisableIdInference(true);

            var client = new ElasticClient(settings);

            return client;
        }

        static void CreateIndexes(ElasticClient client, ElasticSearchSettingModel elasticSearchSetting)
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