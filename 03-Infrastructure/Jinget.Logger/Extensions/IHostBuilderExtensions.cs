using Jinget.Logger.Configuration.File;

namespace Jinget.Logger.Extensions;

/// <summary>
/// Provides extension methods for <see cref="IHostBuilder"/> to configure Jinget logging.
/// </summary>
public static class IHostBuilderExtensions
{
    /// <summary>
    /// Configures the host with default settings and disables scope validation.
    /// </summary>
    /// <param name="args">Command-line arguments.</param>
    /// <returns>The configured <see cref="IHostBuilder"/>.</returns>
    public static IHostBuilder Configure(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseDefaultServiceProvider(provider => provider.ValidateScopes = false);

    /// <summary>
    /// Configures logging to ElasticSearch with specified blacklist and log level.
    /// </summary>
    /// <param name="webHostBuilder">The <see cref="IHostBuilder"/> instance.</param>
    /// <param name="blackList">Array of strings to blacklist from logging.</param>
    /// <param name="blackListUrls">Optional array of URLs to blacklist from logging.</param>
    /// <param name="minAllowedLoglevels">Minimum allowed log level. Defaults to Information.</param>
    /// <returns>The configured <see cref="IHostBuilder"/>.</returns>
    public static IHostBuilder LogToElasticSearch(
        this IHostBuilder webHostBuilder,
        string[] blackList,
        string[]? blackListUrls = null,
        Microsoft.Extensions.Logging.LogLevel minAllowedLoglevels = Microsoft.Extensions.Logging.LogLevel.Information) =>
        webHostBuilder.ConfigureLogging(builder => builder.AddElasticSearch(f =>
        {
            f.BlackListStrings = blackList.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.ToLower()).ToArray();
            f.BlackListUrls = blackListUrls?.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.ToLower()).ToArray();
            f.MinAllowedLogLevel = minAllowedLoglevels;
        }));

    /// <summary>
    /// Configures logging to a file with specified settings, blacklist, and log level.
    /// </summary>
    /// <param name="webHostBuilder">The <see cref="IHostBuilder"/> instance.</param>
    /// <param name="blackList">Array of strings to blacklist from logging.</param>
    /// <param name="fileSettingModel">File logging settings model.</param>
    /// <param name="blackListUrls">Optional array of URLs to blacklist from logging.</param>
    /// <param name="minAllowedLoglevels">Minimum allowed log level. Defaults to Error.</param>
    /// <returns>The configured <see cref="IHostBuilder"/>.</returns>
    public static IHostBuilder LogToFile(
        this IHostBuilder webHostBuilder,
        string[] blackList,
        FileSettingModel fileSettingModel,
        string[]? blackListUrls = null,
        Microsoft.Extensions.Logging.LogLevel minAllowedLoglevels = Microsoft.Extensions.Logging.LogLevel.Error) =>
        webHostBuilder.ConfigureLogging(builder => builder.AddFile(f =>
        {
            f.FileName = fileSettingModel.FileNamePrefix;
            f.LogDirectory = fileSettingModel.LogDirectory;
            f.RetainedFileCountLimit = fileSettingModel.RetainFileCountLimit;
            f.BlackListStrings = blackList.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.ToLower()).ToArray();
            f.BlackListUrls = blackListUrls?.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.ToLower()).ToArray();
            f.FileSizeLimit = fileSettingModel.FileSizeLimitMB;
            f.MinAllowedLogLevel = minAllowedLoglevels;
        }));
}