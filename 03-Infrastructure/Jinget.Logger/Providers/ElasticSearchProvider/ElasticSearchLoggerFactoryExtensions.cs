namespace Jinget.Logger.Providers.ElasticSearchProvider;

/// <summary>
///     Extensions for adding the <see cref="ElasticSearchLoggerProvider" /> to the <see cref="ILoggingBuilder" />
/// </summary>
public static class ElasticSearchLoggerFactoryExtensions
{
    /// <summary>
    ///     Adds a elastic search logger to the factory.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggingBuilder" /> to use.</param>
    /// <param name="configure">Configure an instance of the <see cref="DbLoggerOptions" /> to set logging options</param>
    public static ILoggingBuilder AddElasticSearch(this ILoggingBuilder builder, Action<ElasticSearchLoggerOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(nameof(configure));

        builder.Services.AddSingleton<ILoggerProvider, ElasticSearchLoggerProvider>();
        builder.Services.Configure(configure);
        return builder;
    }
}