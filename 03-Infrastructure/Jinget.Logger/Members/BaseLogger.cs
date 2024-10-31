namespace Jinget.Logger.Members;

public abstract class BaseLogger<TCategoryName> : Log<TCategoryName>
{
    protected BaseLogger(RequestDelegate next, ILogger<TCategoryName> logger,
        IOptions<BlackListHeader> blackListHeaders,
        IOptions<WhiteListHeader> whiteListHeaders)
        : base(next, logger, blackListHeaders, whiteListHeaders)
    {
    }

    protected bool IsMultipartContentType(HttpContext context) =>
        context.Request.GetTypedHeaders() != null &&
        context.Request.GetTypedHeaders().ContentType != null &&
        context.Request.GetTypedHeaders().ContentType.MediaType.Value.ToLower().StartsWith("multipart/form-data");

    protected string GetHeaders(HttpContext context, bool isRequestHeader)
    {
        var headers = "";
        var rawHeaders = isRequestHeader ? context.Request.Headers : context.Response.Headers;
        if (BlackListHeaders.Any())
            headers = JsonConvert.SerializeObject(rawHeaders
                .Where(x => !BlackListHeaders.Contains(x.Key.ToLower()))
                .Select(x => x.ToString()), Formatting.Indented);
        else if (WhiteListHeaders.Any())
            headers = JsonConvert.SerializeObject(rawHeaders
                .Where(x => WhiteListHeaders.Contains(x.Key.ToLower()))
                .Select(x => x.ToString()), Formatting.Indented);
        else
            headers = JsonConvert.SerializeObject(rawHeaders.Select(x => x.ToString()), Formatting.Indented);

        return headers;
    }

    protected string GetPartitionKey(HttpContext context) =>
        context.Items["jinget.log.partitionkey"] != null
            ? context.Items["jinget.log.partitionkey"].ToString()
            : "";

    protected string GetIp(HttpContext context) =>
        context.Connection.RemoteIpAddress == null
            ? "Unknown"
            : context.Connection.RemoteIpAddress.ToString();

    protected string GetAdditionalData(HttpContext context, bool isRequestData)
    {
        var rawHeaders = isRequestData ? context.Request.Headers : context.Response.Headers;
        return JsonConvert.SerializeObject(new
        {
            AdditionalDataInHeader = rawHeaders["AdditionalData"],
            AdditionalDataInCtx = context.Items["AdditionalData"]?.ToString()
        });
    }
}