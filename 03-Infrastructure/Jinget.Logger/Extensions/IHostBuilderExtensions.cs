namespace Jinget.Logger.Extensions;

public static class IHostBuilderExtensions
{
    public static IHostBuilder Configure(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseDefaultServiceProvider(provider => provider.ValidateScopes = false);

    public static IHostBuilder LogToElasticSearch(
        this IHostBuilder webHostBuilder,
        string[] blackList,
        Microsoft.Extensions.Logging.LogLevel minAllowedLoglevels = Microsoft.Extensions.Logging.LogLevel.Information)
        =>
        webHostBuilder.ConfigureLogging(builder => builder.AddElasticSearch(f =>
        {
            f.BlackListStrings = blackList.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.ToLower()).ToArray();
            f.MinAllowedLogLevel = minAllowedLoglevels;
        }));

    public static IHostBuilder LogToFile(
        this IHostBuilder webHostBuilder,
        string[] blackList,
        string fileNamePrefix = "Log",
        string logDirectory = "Logs",
        int retainFileCountLimit = 5,
        int fileSizeLimit = 10,
        Microsoft.Extensions.Logging.LogLevel minAllowedLoglevels = Microsoft.Extensions.Logging.LogLevel.Error) =>
        webHostBuilder.ConfigureLogging(builder => builder.AddFile(f =>
        {
            f.FileName = fileNamePrefix;
            f.LogDirectory = logDirectory;
            f.RetainedFileCountLimit = retainFileCountLimit;
            f.BlackListStrings = blackList.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.ToLower()).ToArray();
            f.FileSizeLimit = fileSizeLimit;
            f.MinAllowedLogLevel = minAllowedLoglevels;
        }));
}