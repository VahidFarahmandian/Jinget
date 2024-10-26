namespace Jinget.Logger.Providers.FileProvider;

/// <summary>
///     Extensions for adding the <see cref="FileLoggerProvider" /> to the <see cref="ILoggingBuilder" />
/// </summary>
public static class FileLoggerFactoryExtensions
{
    /// <summary>
    ///     Adds a file logger named 'File' to the factory.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggingBuilder" /> to use.</param>
    public static ILoggingBuilder AddFile(this ILoggingBuilder builder)
    {
        builder.Services.AddSingleton<ILoggerProvider, FileLoggerProvider>();
        return builder;
    }

    /// <summary>
    ///     Adds a file logger named 'File' to the factory.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggingBuilder" /> to use.</param>
    /// <param name="configure">Configure an instance of the <see cref="FileLoggerOptions" /> to set logging options</param>
    public static ILoggingBuilder AddFile(this ILoggingBuilder builder, Action<FileLoggerOptions> configure)
    {
        if (configure == null) throw new ArgumentNullException("Jinget Says: " + nameof(configure));
        builder.AddFile();
        builder.Services.Configure(configure);

        return builder;
    }
}