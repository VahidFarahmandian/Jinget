using Jinget.ExceptionHandler.Extensions;
using Jinget.Logger.Providers;

namespace Jinget.Logger.Middlewares;

public class RequestResponseLoggingMiddleware
{
    protected readonly ILogger<RequestResponseLoggingMiddleware> _logger;
    protected readonly RequestDelegate _next;
    protected readonly IOptions<BatchingLoggerOptions> _loggingOptions;

    protected List<string>? _blackListHeaders;
    protected List<string>? _whiteListHeaders;

    public RequestResponseLoggingMiddleware(
        RequestDelegate next,
        ILogger<RequestResponseLoggingMiddleware> logger,
        IOptions<BatchingLoggerOptions> loggingOptions,
        IOptions<BlackListHeader> blackListHeaders,
        IOptions<WhiteListHeader> whiteListHeaders)
    {
        _next = next;
        _logger = logger;
        _loggingOptions = loggingOptions;

        _blackListHeaders = blackListHeaders.Value.Headers?.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.ToLower()).ToList();
        _blackListHeaders ??= [];

        _whiteListHeaders = whiteListHeaders.Value.Headers?.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.ToLower()).ToList();
        _whiteListHeaders ??= [];
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Method == "OPTIONS")
            return;
        await LogRequestAsync(context);

        var originalBodyStream = context.Response.Body;

        using (var responseBody = new MemoryStream())
        {
            context.Response.Body = responseBody;

            try
            {
                await _next(context);

                await LogResponseAsync(context);

                // Always copy the MemoryStream back to the original body.
                responseBody.Seek(0, SeekOrigin.Begin);
                await responseBody.CopyToAsync(originalBodyStream);
            }
            finally
            {
                // Ensure the original body is restored, even if an exception occurs.
                context.Response.Body = originalBodyStream;
            }
        }
    }

    private async Task LogRequestAsync(HttpContext context)
    {
        string requestBodyText = "";
        if (context.IsMultipartContentType())
        {
            requestBodyText = "--REQUEST BODY TRIMMED BY LOGGER-- multipart/form-data";
        }
        else if (context.Request.ContentLength > _loggingOptions.Value.MaxRequestBodySize)
        {
            requestBodyText = $"--REQUEST BODY TOO LARGE--EXCEEDS {_loggingOptions.Value.MaxRequestBodySize} BYTE";
        }
        else
        {
            context.Request.EnableBuffering();
            var body = context.Request.Body;

            var buffer = new byte[Convert.ToInt64(context.Request.ContentLength)];
            await context.Request.Body.ReadAsync(buffer, 0, buffer.Length);

            requestBodyText = Encoding.UTF8.GetString(buffer);
            body.Seek(0, SeekOrigin.Begin);
            context.Request.Body = body;
        }

        SetRequestLog(context, requestBodyText);
    }

    private async Task LogResponseAsync(HttpContext context)
    {
        string responseBodyText = "";
        if (context.IsMultipartContentType())
        {
            responseBodyText = "--RESPONSE BODY TRIMMED BY LOGGER-- multipart/form-data";
        }
        else if (context.Request.ContentLength > _loggingOptions.Value.MaxResponseBodySize)
        {
            responseBodyText = $"--RESPONSE BODY TOO LARGE--EXCEEDS {_loggingOptions.Value.MaxResponseBodySize} BYTE";
        }
        else
        {
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            responseBodyText = await new StreamReader(context.Response.Body).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);
        }
        SetResponseLog(context, responseBodyText);
    }

    private void SetResponseLog(HttpContext context, string responseBody)
    {
        var model = LogModel.GetNewResponseObject(
            context,
            responseBody,
            context.GetLoggerHeaders(_blackListHeaders, _whiteListHeaders, isRequestHeader: false));

        if (context.Response.StatusCode < 400 || context.Response.StatusCode == 429)
            _logger.LogInformation(model.Serialize());
    }

    private void SetRequestLog(HttpContext context, string requestBodyText)
    {
        var model = LogModel.GetNewRequestObject(context, requestBodyText,
            context.GetLoggerHeaders(_blackListHeaders, _whiteListHeaders, isRequestHeader: true));

        _logger.LogInformation(model.Serialize());
    }
}