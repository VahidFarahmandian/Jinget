namespace Jinget.Logger.Members;

public class ResponseLogger<TCategoryName> : BaseLogger<TCategoryName>, ILog
{
    public ResponseLogger(RequestDelegate next,
        ILogger<TCategoryName> logger,
        IOptions<BlackListHeader> blackListHeaders,
        IOptions<WhiteListHeader> whiteListHeaders)
        : base(next, logger, blackListHeaders, whiteListHeaders)
    {
    }

    public async Task LogAsync(HttpContext context)
    {
        string responseBody;
        Stream responseBodyStream = null;
        Stream bodyStream = null;

        if (IsMultipartContentType(context))
        {
            responseBody = "--RESPONSE BODY TRIMMED BY LOGGER-- multipart/form-data";
            await Next(context);
        }
        else
        {
            bodyStream = context.Response.Body;
            responseBodyStream = new MemoryStream();

            context.Response.Body = responseBodyStream;
            await Next(context);

            responseBodyStream.Seek(0, SeekOrigin.Begin);
            responseBody = await new StreamReader(responseBodyStream).ReadToEndAsync();
        }

        var contentLength = context.Response.ContentLength ??
                            string.Join(",",
                                    context.Response.Headers.Select(x => x.Key + ":" + x.Value)
                                        .ToList())
                                .Length +
                            responseBody.Length;

        context.Request.Headers.TryGetValue("Referer", out StringValues pageUrl);

        var model = new LogModel
        {
            ParitionKey = GetPartitionKey(context),
            Username = context.User.Identity.Name,
            Method = context.Request.Method,
            Body = responseBody,
            Headers = GetHeaders(context, isRequestHeader: false),
            Url = context.Request.GetDisplayUrl(),
            IP = GetIp(context),
            Type = LogType.Response,
            Description = JsonConvert.SerializeObject(new { context.Response.StatusCode }),
            TraceIdentifier = context.TraceIdentifier, //new Guid(context.Response.Headers["RequestId"].ToString()),
            AdditionalData = GetAdditionalData(context, isRequestData: false),
            SubSystem = AppDomain.CurrentDomain.FriendlyName,
            PageUrl = pageUrl.FirstOrDefault(),
            ContentLength = contentLength
        };

        if (context.Request.Method != "OPTIONS")
        {
            if (context.Response.StatusCode < 400)
                Logger.LogInformation(null, JsonConvert.SerializeObject(model));
        }

        if (responseBodyStream != null && context.Response.StatusCode != 204)
        {
            responseBodyStream.Seek(0, SeekOrigin.Begin);
            await responseBodyStream.CopyToAsync(bodyStream);
        }
    }
}