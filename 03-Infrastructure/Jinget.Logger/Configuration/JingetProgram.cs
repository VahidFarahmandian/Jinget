using Jinget.Logger.Entities.Log;
using Jinget.Logger.Providers.ElasticSearchProvider;
using Jinget.Logger.Providers.FileProvider;
using Microsoft.Extensions.Hosting;

namespace Jinget.Logger.Configuration
{
    public static class JingetProgram
    {
        public static IHostBuilder Configure(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseDefaultServiceProvider(provider => provider.ValidateScopes = false);

        public static IHostBuilder LogToElasticSearch<TOperationalEntity, TErrorEntity, TCustomEntity>(this IHostBuilder webHostBuilder, string[] blackList)
            where TOperationalEntity : OperationLog, new()
            where TErrorEntity : ErrorLog, new()
            where TCustomEntity : CustomLog, new()
            =>
            webHostBuilder.ConfigureLogging(builder => builder.AddElasticSearch<TOperationalEntity, TErrorEntity, TCustomEntity>(f =>
            {
                f.BlackListStrings = blackList;
            }));

        public static IHostBuilder LogToFile(this IHostBuilder webHostBuilder, string[] blackList, string fileNamePrefix = "Log", string logDirectory = "Logs", int retainFileCountLimit = 5, int fileSizeLimit = 10) =>
            webHostBuilder.ConfigureLogging(builder => builder.AddFile(f =>
            {
                f.FileName = fileNamePrefix;
                f.LogDirectory = logDirectory;
                f.RetainedFileCountLimit = retainFileCountLimit;
                f.BlackListStrings = blackList;
                f.FileSizeLimit = fileSizeLimit;
            }));
    }
}