namespace Jinget.Logger;

public class Log<TCategoryName>
{
    protected readonly ILogger<TCategoryName> Logger;
    protected readonly RequestDelegate Next;

    protected List<string> BlackListHeaders;
    protected List<string> WhiteListHeaders;

    protected Log(RequestDelegate next, ILogger<TCategoryName> logger, IOptions<BlackListHeader> blackListHeaders, IOptions<WhiteListHeader> whiteListHeaders)
    {
        Next = next;
        Logger = logger;

        BlackListHeaders = blackListHeaders.Value.Headers?.Where(x => x != null).Select(x => x.ToLower()).ToList();
        BlackListHeaders ??= new List<string>();

        WhiteListHeaders = whiteListHeaders.Value.Headers?.Where(x => x != null).Select(x => x.ToLower()).ToList();
        WhiteListHeaders ??= new List<string>();
    }
}