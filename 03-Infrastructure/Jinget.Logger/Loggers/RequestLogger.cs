using Jinget.ExceptionHandler.Extensions;
using Jinget.Logger;

namespace Jinget.Logger.Loggers;

public class RequestLogger<TCategoryName>(
    RequestDelegate next,
    ILogger<TCategoryName> logger,
    IOptions<BlackListHeader> blackListHeaders,
    IOptions<WhiteListHeader> whiteListHeaders) : Log<TCategoryName>(next, logger, blackListHeaders, whiteListHeaders), ILog
{
    public async Task LogAsync(HttpContext context)
    {
        string requestBodyText;
        if (context.IsMultipartContentType())
        {
            requestBodyText = "--REQUEST BODY TRIMMED BY LOGGER-- multipart/form-data";
        }
        else
        {
            context.Request.EnableBuffering();
            using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
            requestBodyText = await reader.ReadToEndAsync();
            context.Request.Body.Seek(0, SeekOrigin.Begin);
        }

        SetLog(context, requestBodyText);
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