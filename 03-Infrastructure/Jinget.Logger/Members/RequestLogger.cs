using System;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Jinget.Core.IOptionTypes.Log;
using Jinget.Logger.Entities.Log;
using Jinget.Logger.ExceptionHandler;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace Jinget.Logger.Members;

public class RequestLogger<TCategoryName> : Log<TCategoryName>, ILog
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

        if (context.Request.GetTypedHeaders() != null &&
            context.Request.GetTypedHeaders().ContentType != null &&
            context.Request.GetTypedHeaders().ContentType.MediaType.Value.ToLower().StartsWith("multipart/form-data"))
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

        context.Response.Headers.Add("RequestId", Guid.NewGuid().ToString());

        var contentLength = context.Request.ContentLength ??
                            string.Join(",", context.Request.Headers.Select(x => x.Key + ":" + x.Value).ToList()).Length +
                            requestBodyText.Length;
        context.Request.Headers.TryGetValue("Referer", out StringValues pageUrl);

        string headers = "";
        if (BlackListHeaders.Any())
            headers = JsonConvert.SerializeObject(context.Response.Headers
                .Where(x => !BlackListHeaders.Contains(x.Key.ToLower()))
                .Select(x => x.ToString()), Formatting.Indented);
        else if (WhiteListHeaders.Any())
            headers = JsonConvert.SerializeObject(context.Response.Headers
                .Where(x => WhiteListHeaders.Contains(x.Key.ToLower()))
                .Select(x => x.ToString()), Formatting.Indented);

        var model = new OperationLog
        {
            ParitionKey = context.Items["jinget.log.partitionkey"] != null ? context.Items["jinget.log.partitionkey"].ToString() : "",
            UserName = context.User.Identity.Name,
            When = DateTime.Now,
            Method = context.Request.Method,
            Body = requestBodyText,
            Headers = headers,
            Url = context.Request.GetDisplayUrl(),
            IP = context.Connection.RemoteIpAddress == null ? "Unknown" : context.Connection.RemoteIpAddress.ToString(),
            IsResponse = false,
            Description = null,
            RequestId = new Guid(context.Response.Headers["RequestId"].ToString()),
            Detail = context.Request.Headers["Detail"],
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
        }

        if (originalRequestBody != null) context.Request.Body = originalRequestBody;
    }
}