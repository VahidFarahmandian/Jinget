namespace Jinget.Logger.Members;

public class RequestLogger<TCategoryName> : BaseLogger<TCategoryName>, ILog
{
    private readonly IExceptionHandler<TCategoryName> _exception;

    public RequestLogger(RequestDelegate next, ILogger<TCategoryName> logger,
        IExceptionHandler<TCategoryName> exception,
        IOptions<BlackListHeader> blackListHeaders,
        IOptions<WhiteListHeader> whiteListHeaders)
        : base(next, logger, blackListHeaders, whiteListHeaders) => _exception = exception;

    public async Task LogAsync(HttpContext context)
    {
        string requestBodyText;
        Stream originalRequestBody = null;

        if (IsMultipartContentType(context))
        {
            requestBodyText = "--REQUEST BODY TRIMMED BY LOGGER-- multipart/form-data";
        }
        else
        {
            var requestBodyStream = new MemoryStream();
            originalRequestBody = context.Request?.Body;
            await context.Request?.Body.CopyToAsync(requestBodyStream);
            requestBodyStream.Seek(0, SeekOrigin.Begin);
            requestBodyText = await new StreamReader(requestBodyStream).ReadToEndAsync();
            requestBodyStream.Seek(0, SeekOrigin.Begin);
            context.Request.Body = requestBodyStream;
        }

        //context.Response.Headers.Add("RequestId", Guid.NewGuid().ToString());

        var contentLength = context.Request.ContentLength ??
                            string.Join(",", context.Request.Headers.Select(x => x.Key + ":" + x.Value).ToList())
                                .Length +
                            requestBodyText.Length;
        context.Request.Headers.TryGetValue("Referer", out StringValues pageUrl);

        var model = new LogModel
        {
            ParitionKey = GetPartitionKey(context),
            Username = context.User.Identity.Name,
            Method = context.Request.Method,
            Body = requestBodyText,
            Headers = GetHeaders(context, isRequestHeader: true),
            Url = context.Request.GetDisplayUrl(),
            IP = GetIp(context),
            Type = LogType.Request,
            Description = null,
            TraceIdentifier = context.TraceIdentifier,
            AdditionalData = GetAdditionalData(context, isRequestData: true),
            SubSystem = AppDomain.CurrentDomain.FriendlyName,
            PageUrl = pageUrl.FirstOrDefault(),
            ContentLength = contentLength
        };

        try
        {
            if (context.Request.Method != "OPTIONS")
                Logger.LogInformation(JsonConvert.SerializeObject(model));

            await Next(context);
        }
        catch (Exception ex)
        {
            _exception.Handle(ex, model);
            context.Response.StatusCode = 500;
            context.Response.ContentType = MediaTypeNames.Application.Json;
            await context.Response.WriteAsync(JsonConvert.SerializeObject(new { error = ex.Message }));
            throw;
        }

        if (originalRequestBody != null) context.Request.Body = originalRequestBody;
    }
}