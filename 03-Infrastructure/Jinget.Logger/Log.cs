//using Jinget.Logger.Providers;

//namespace Jinget.Logger;

//public class Log<TCategoryName>
//{
//    protected readonly ILogger<TCategoryName> Logger;
//    protected readonly RequestDelegate Next;
//    protected readonly IOptions<BatchingLoggerOptions> LoggingOptions;

//    protected List<string>? BlackListHeaders;
//    protected List<string>? WhiteListHeaders;

//    protected Log(RequestDelegate next, ILogger<TCategoryName> logger,
//        IOptions<BatchingLoggerOptions> loggingOptions,
//        IOptions<BlackListHeader> blackListHeaders, IOptions<WhiteListHeader> whiteListHeaders)
//    {
//        Next = next;
//        Logger = logger;
//        LoggingOptions = loggingOptions;
        
//        BlackListHeaders = blackListHeaders.Value.Headers?.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.ToLower()).ToList();
//        BlackListHeaders ??= [];

//        WhiteListHeaders = whiteListHeaders.Value.Headers?.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.ToLower()).ToList();
//        WhiteListHeaders ??= [];
//    }
//}