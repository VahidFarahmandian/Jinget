using Jinget.Logger.Extensions;

namespace Jinget.Logger.Configuration.ElasticSearch;

/// <summary>
/// Provides extension methods for configuring Elasticsearch logging.
/// </summary>
public static class ElasticSearchConfiguration
{
    /// <summary>
    /// Configures Elasticsearch logging services and client.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure.</param>
    /// <param name="elasticSearchSetting">Optional Elasticsearch settings model.</param>
    public static void ConfigureElasticSearchLogger(this IServiceCollection services,
        ElasticSearchSettingModel? elasticSearchSetting = null)
    {
        // Configures Jinget logger prerequisites.
        services.ConfigureJingetLoggerPrerequisites(elasticSearchSetting);

        // Adds Elasticsearch logging domain service and repository as scoped services.
        services.TryAddScoped<IElasticSearchLoggingDomainService, ElasticSearchLoggingDomainService>();
        services.TryAddScoped<IElasticSearchLoggingRepository, ElasticSearchLoggingRepository>();

        // If Elasticsearch settings are provided, configure the client and index.
        if (elasticSearchSetting != null)
        {
            // Adds Elasticsearch settings model as a singleton.
            services.TryAddSingleton(elasticSearchSetting);

            // Creates and adds Elasticsearch client as a singleton.
            var client = CreateClient(elasticSearchSetting);
            services.AddSingleton(typeof(IElasticClient), client);

            // Creates the index if it doesn't exist.
            CreateIndex(client, elasticSearchSetting);
        }
    }

    /// <summary>
    /// Creates an Elasticsearch client based on the provided settings.
    /// </summary>
    /// <param name="elasticSearchSetting">Elasticsearch settings model.</param>
    /// <returns>An Elasticsearch client instance.</returns>
    private static ElasticClient CreateClient(ElasticSearchSettingModel elasticSearchSetting)
    {
        ConnectionSettings connectionSettings;
        var protocol = elasticSearchSetting.UseSsl ? "https" : "http";
        Uri elasticUrl = new($"{protocol}://{elasticSearchSetting.Url}");

        // Configures connection settings with or without basic authentication.
        if (string.IsNullOrEmpty(elasticSearchSetting.UserName) && string.IsNullOrEmpty(elasticSearchSetting.Password))
        {
            connectionSettings = new ConnectionSettings(elasticUrl)
                .DefaultDisableIdInference(true);
        }
        else
        {
            connectionSettings = new ConnectionSettings(elasticUrl)
                .BasicAuthentication(elasticSearchSetting.UserName, elasticSearchSetting.Password)
                .DefaultDisableIdInference(true);

            // Bypasses certificate validation if specified.
            if (elasticSearchSetting.UseSsl && elasticSearchSetting.BypassCertificateValidation)
            {
                connectionSettings =
                    connectionSettings.ServerCertificateValidationCallback(CertificateValidations.AllowAll);
            }
        }

        return new ElasticClient(connectionSettings);
    }

    /// <summary>
    /// Creates the Elasticsearch index if it doesn't exist.
    /// </summary>
    /// <param name="client">Elasticsearch client instance.</param>
    /// <param name="elasticSearchSetting">Elasticsearch settings model.</param>
    private static void CreateIndex(ElasticClient client, ElasticSearchSettingModel elasticSearchSetting)
    {
        // Creates index only if CreateIndexPerPartition is false.
        if (!elasticSearchSetting.CreateIndexPerPartition)
        {
            // Generates index name from application domain friendly name, sanitizing invalid characters.
            var indexName = $"{AppDomain.CurrentDomain.FriendlyName}".ToLower();
            var invalidChars = @" \*\\<|,>/?".ToCharArray();
            indexName = new string(indexName.Where(c => !invalidChars.Contains(c)).ToArray());

            // Creates index if it doesn't exist.
            if (!client.Indices.Exists(indexName).Exists)
            {
                var result = client.Indices.Create(indexName,
                    index => index.Map(m => m.AutoMap(typeof(LogModel)).NumericDetection(true)));

                // Throws an exception if index creation fails.
                if (!result.IsValid)
                {
                    throw new JingetException("Jinget Says: " + result.OriginalException);
                }
            }
        }
    }
}