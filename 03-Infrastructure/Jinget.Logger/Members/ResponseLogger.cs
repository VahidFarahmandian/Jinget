using Jinget.ExceptionHandler.Entities.Log;
using Jinget.ExceptionHandler.Extensions;

namespace Jinget.Logger.Members;

public class ResponseLogger<TCategoryName>(
    RequestDelegate next,
    ILogger<TCategoryName> logger,
    IOptions<BlackListHeader> blackListHeaders,
    IOptions<WhiteListHeader> whiteListHeaders) : Log<TCategoryName>(next, logger, blackListHeaders, whiteListHeaders), ILog
{
    public async Task LogAsync(HttpContext context)
    {
        var originalResponseBody = context.Response?.Body;
        var responseBodyStream = new MemoryStream();
        context.Response.Body = responseBodyStream;
        try
        {
            await Next(context);
            string responseBodyText;
            if (context.IsMultipartContentType())
                responseBodyText = "--RESPONSE BODY TRIMMED BY LOGGER-- multipart/form-data";
            else
            {
                // Reset the body position to read it  
                responseBodyStream.Seek(0, SeekOrigin.Begin);
                responseBodyText = await new StreamReader(responseBodyStream).ReadToEndAsync();
                responseBodyStream.Seek(0, SeekOrigin.Begin);
                await responseBodyStream.CopyToAsync(originalResponseBody);
            }

            SetLog(context, responseBodyText);
        }
        finally
        {
            context.Response.Body = originalResponseBody;
        }
    }

    private void SetLog(HttpContext context, string responseBody)
    {
        if (context.Request.Method == "OPTIONS")
            return;
        var model = LogModel.GetNewResponseObject(
            context,
            responseBody,
            context.GetLoggerHeaders(BlackListHeaders, WhiteListHeaders, isRequestHeader: false));

        if (context.Response.StatusCode < 400)
            Logger.LogInformation(JsonConvert.SerializeObject(model));
    }
}