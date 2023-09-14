using System;
using Jinget.Logger.Entities.Log;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Jinget.Logger.Providers.ElasticSearchProvider
{
    /// <summary>
    ///     Extensions for adding the <see cref="ElasticSearchLoggerProvider" /> to the <see cref="ILoggingBuilder" />
    /// </summary>
    public static class ElasticSearchLoggerFactoryExtensions
    {
        /// <summary>
        ///     Adds a elastic search logger to the factory.
        /// </summary>
        /// <param name="builder">The <see cref="ILoggingBuilder" /> to use.</param>
        public static ILoggingBuilder AddElasticSearch<TOperationalEntity, TErrorEntity, TCustomEntity>(this ILoggingBuilder builder)
            where TOperationalEntity : OperationLog, new()
            where TErrorEntity : ErrorLog, new()
            where TCustomEntity : CustomLog, new()
        {
            builder.Services.AddSingleton<ILoggerProvider, ElasticSearchLoggerProvider<TOperationalEntity, TErrorEntity, TCustomEntity>>();
            return builder;
        }

        /// <summary>
        ///     Adds a elastic search logger to the factory.
        /// </summary>
        /// <param name="builder">The <see cref="ILoggingBuilder" /> to use.</param>
        /// <param name="configure">Configure an instance of the <see cref="DbLoggerOptions" /> to set logging options</param>
        public static ILoggingBuilder AddElasticSearch<TOperationalEntity, TErrorEntity, TCustomEntity>(this ILoggingBuilder builder, Action<ElasticSearchLoggerOptions> configure)
            where TOperationalEntity : OperationLog, new()
            where TErrorEntity : ErrorLog, new()
            where TCustomEntity : CustomLog, new()
        {
            ArgumentNullException.ThrowIfNull(nameof(configure));

            builder.AddElasticSearch<TOperationalEntity, TErrorEntity, TCustomEntity>();
            builder.Services.Configure(configure);
            return builder;
        }
    }
}