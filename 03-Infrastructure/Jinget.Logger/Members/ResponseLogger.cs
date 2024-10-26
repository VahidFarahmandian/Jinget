﻿namespace Jinget.Logger.Members;

public class ResponseLogger<TCategoryName> : Log<TCategoryName>, ILog
{
    public ResponseLogger(RequestDelegate next,
        ILogger<TCategoryName> logger,
        IOptions<BlackListHeader> blackListHeaders,
        IOptions<WhiteListHeader> whiteListHeaders)
        : base(next, logger, blackListHeaders, whiteListHeaders) { }

    public async Task LogAsync(HttpContext context)
    {
        string responseBody;
        Stream responseBodyStream = null;
        Stream bodyStream = null;

        if (context.Request?.GetTypedHeaders() != null &&
        context.Request.GetTypedHeaders().ContentType != null &&
        context.Request.GetTypedHeaders().ContentType.MediaType.Value.ToLower().StartsWith("multipart/form-data"))
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

        string headers = "";
        if (BlackListHeaders.Any())
            headers = JsonConvert.SerializeObject(
                context.Response.Headers
                .Where(x => !BlackListHeaders.Contains(x.Key.ToLower()))
                .Select(x => x.ToString()), Formatting.Indented);
        else if (WhiteListHeaders.Any())
            headers = JsonConvert.SerializeObject(context.Response.Headers
                .Where(x => WhiteListHeaders.Contains(x.Key.ToLower()))
                .Select(x => x.ToString()), Formatting.Indented);

        var model = new LogModel
        {
            ParitionKey = context.Items["jinget.log.partitionkey"] != null ? context.Items["jinget.log.partitionkey"].ToString() : "",
            Username = context.User.Identity.Name,
            Method = context.Request.Method,
            Body = responseBody,
            Headers = headers,
            Url = context.Request.GetDisplayUrl(),
            IP = context.Connection.RemoteIpAddress == null
                ? "Unknown"
                : context.Connection.RemoteIpAddress.ToString(),
            IsResponse = true,
            Description = JsonConvert.SerializeObject(new { context.Response.StatusCode }),
            RequestId = new Guid(context.Response.Headers["RequestId"].ToString()),
            AdditionalData = JsonConvert.SerializeObject(new
            {
                AdditionalDataInHeader = context.Response.Headers["AdditionalData"],
                AdditionalDataInCtx = context.Items["AdditionalData"]?.ToString()
            }),
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