using Jinget.ExceptionHandler.Extensions;
using Jinget.Logger.Providers;

namespace Jinget.Logger.Middlewares;

/// <summary>
/// Middleware for logging HTTP requests and responses, including headers and body content.
/// </summary>
public class RequestResponseLoggingMiddleware
{
    protected readonly ILogger<RequestResponseLoggingMiddleware> _logger;
    protected readonly RequestDelegate _next;
    protected readonly IOptions<BatchingLoggerOptions> _loggingOptions;
    protected List<string>? _blackListHeaders;
    protected List<string>? _whiteListHeaders;

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestResponseLoggingMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next request delegate in the pipeline.</param>
    /// <param name="logger">The logger for request/response logging.</param>
    /// <param name="loggingOptions">The logging options.</param>
    /// <param name="blackListHeaders">The options containing the blacklisted headers.</param>
    /// <param name="whiteListHeaders">The options containing the whitelisted headers.</param>
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

        // Initialize blacklisted headers, converting to lowercase for case-insensitive comparison.
        _blackListHeaders = blackListHeaders.Value.Headers?.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.ToLower()).ToList();
        // Ensure blacklist is not null.
        _blackListHeaders ??= [];

        // Initialize whitelisted headers, converting to lowercase for case-insensitive comparison.
        _whiteListHeaders = whiteListHeaders.Value.Headers?.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.ToLower()).ToList();
        // Ensure whitelist is not null.
        _whiteListHeaders ??= [];
    }

    /// <summary>
    /// Invokes the middleware to log the request and response.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        // Skip logging for OPTIONS requests.
        if (context.Request.Method == "OPTIONS")
            await _next(context);
        else
        {
            // Log the request.
            await LogRequestAsync(context);

            // Store the original response body stream.
            var originalBodyStream = context.Response.Body;

            // Use a MemoryStream to capture the response body.
            using (var responseBody = new MemoryStream())
            {
                // Replace the response body with the MemoryStream.
                context.Response.Body = responseBody;

                try
                {
                    // Invoke the next middleware.
                    await _next(context);

                    // Log the response.
                    await LogResponseAsync(context);

                    // Restore the original response body.
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
    }

    /// <summary>
    /// Logs the request details.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    private async Task LogRequestAsync(HttpContext context)
    {
        string requestBodyText = "";
        // Handle multipart/form-data requests.
        if (context.IsMultipartContentType())
        {
            requestBodyText = "--REQUEST BODY TRIMMED BY LOGGER-- multipart/form-data";
        }
        // Handle requests with bodies exceeding the maximum size.
        else if (context.Request.ContentLength > _loggingOptions.Value.MaxRequestBodySize)
        {
            requestBodyText = $"--REQUEST BODY TOO LARGE--EXCEEDS {_loggingOptions.Value.MaxRequestBodySize} BYTE";
        }
        // Capture the request body.
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

    /// <summary>
    /// Logs the response details.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    private async Task LogResponseAsync(HttpContext context)
    {
        string responseBodyText = "";
        // Handle multipart/form-data responses.
        if (context.IsMultipartContentType())
        {
            responseBodyText = "--RESPONSE BODY TRIMMED BY LOGGER-- multipart/form-data";
        }
        // Handle responses with bodies exceeding the maximum size.
        else if (context.Request.ContentLength > _loggingOptions.Value.MaxResponseBodySize)
        {
            responseBodyText = $"--RESPONSE BODY TOO LARGE--EXCEEDS {_loggingOptions.Value.MaxResponseBodySize} BYTE";
        }
        // Capture the response body.
        else
        {
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            responseBodyText = await new StreamReader(context.Response.Body).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);
        }
        SetResponseLog(context, responseBodyText);
    }

    /// <summary>
    /// Sets and logs the response log model.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="responseBody"></param>
    private void SetResponseLog(HttpContext context, string responseBody)
    {
        var model = LogModel.GetNewResponseObject(
            context,
            responseBody,
            context.GetLoggerHeaders(_blackListHeaders, _whiteListHeaders, isRequestHeader: false));
        // Log only if status code is successful or 429 (Too Many Requests).
        if (context.Response.StatusCode < 400 || context.Response.StatusCode == 429)
            _logger.LogInformation(model.Serialize());
    }

    /// <summary>
    /// Sets and logs the request log model.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="requestBodyText"></param>
    private void SetRequestLog(HttpContext context, string requestBodyText)
    {
        var model = LogModel.GetNewRequestObject(context, requestBodyText,
            context.GetLoggerHeaders(_blackListHeaders, _whiteListHeaders, isRequestHeader: true));
        _logger.LogInformation(model.Serialize());
    }
}