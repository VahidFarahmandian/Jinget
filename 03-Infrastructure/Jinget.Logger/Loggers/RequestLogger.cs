using Jinget.ExceptionHandler.Extensions;
using Jinget.Logger;

namespace Jinget.Logger.Loggers;

public class RequestLogger<TCategoryName> : Log<TCategoryName>, ILog
{
    public RequestLogger(RequestDelegate next, ILogger<TCategoryName> logger,
        IOptions<BlackListHeader> blackListHeaders,
        IOptions<WhiteListHeader> whiteListHeaders)
        : base(next, logger, blackListHeaders, whiteListHeaders)
    {
    }

    public async Task LogAsync(HttpContext context)
    {
        context.Request.EnableBuffering();
        string requestBodyText;
        bool ismultiPart = false;
        var originalRequestBody = context.Request?.Body;
        if (context.IsMultipartContentType())
        {
            requestBodyText = "--REQUEST BODY TRIMMED BY LOGGER-- multipart/form-data";
            ismultiPart = true;
        }
        else
        {
            var requestBodyStream = new MemoryStream();
            await context.Request?.Body.CopyToAsync(requestBodyStream);
            requestBodyStream.Seek(0, SeekOrigin.Begin);
            requestBodyText = await new StreamReader(requestBodyStream, Encoding.UTF8).ReadToEndAsync();
        }

        SetLog(context, requestBodyText);
        if (!ismultiPart)
            context.Request.Body = originalRequestBody;

        await Next(context);
    }

    private void SetLog(HttpContext context, string requestBodyText)
    {
        var model = LogModel.GetNewRequestObject(context, requestBodyText,
            context.GetLoggerHeaders(BlackListHeaders, WhiteListHeaders, isRequestHeader: true));

        if (context.Request.Method != "OPTIONS")
            Logger.LogInformation(JsonConvert.SerializeObject(model));
    }
}