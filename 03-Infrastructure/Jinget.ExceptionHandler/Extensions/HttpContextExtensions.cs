namespace Jinget.ExceptionHandler.Extensions;

public static class HttpContextExtensions
{
    public static long GetRequestContentLength(this HttpContext context, string body)
    {
        return context.Request.ContentLength ??
                           string.Join(",", context.Request.Headers.Select(x => x.Key + ":" + x.Value).ToList())
                               .Length +
                           body.Length;
    }

    public static long GetResponseContentLength(this HttpContext context, string body)
    {
        return context.Response.ContentLength ??
                            string.Join(",",
                                    context.Response.Headers.Select(x => x.Key + ":" + x.Value)
                                        .ToList())
                                .Length +
                            body.Length;
    }

    /// <summary>
    /// set current datetime to HttpContext.Items. <seealso cref="GetRequestDateTime(HttpContext)"/>
    /// </summary>
    public static void SetRequestDateTime(this HttpContext context, DateTime dateTime) =>
        context.Items.Add("jinget.log.request.datetime", dateTime);

    /// <summary>
    /// get saved datetime from HttpContext.Items. <seealso cref="SetRequestDateTime(HttpContext, DateTime)"/>
    /// </summary>
    public static DateTime? GetRequestDateTime(this HttpContext context) =>
        context.Items.ContainsKey("jinget.log.request.datetime") ? Convert.ToDateTime(context.Items["jinget.log.request.datetime"]) : null;

    /// <summary>
    /// Get the partition key used for logging
    /// </summary>
    public static string? GetLoggerPartitionKey(this HttpContext context) =>
        context.Items["jinget.log.partitionkey"]?.ToString();

    /// <summary>
    /// set the partition key used for logging
    /// </summary>
    public static void SetLoggerPartitionKey(this HttpContext context, string key) =>
        context.Items.Add("jinget.log.partitionkey", key);

    /// <summary>
    /// set additional data used for logging
    /// </summary>
    public static string? GetLoggerAdditionalData(this HttpContext context, bool isRequestData)
    {
        var rawHeaders = isRequestData ? context.Request.Headers : context.Response.Headers;
        var additionalData = new List<string>();

        if (rawHeaders != null && rawHeaders.TryGetValue("AdditionalData", out StringValues headerValues) && !StringValues.IsNullOrEmpty(headerValues))
        {
            foreach (var value in headerValues)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    additionalData.Add(value);
                }
            }
        }

        if (context.Items["AdditionalData"] is string itemValue && !string.IsNullOrEmpty(itemValue))
        {
            additionalData.Add(itemValue);
        }

        return additionalData.Any() ? additionalData.Serialize() : null;
    }

    /// <summary>
    /// get request/response headers used for logging
    /// </summary>
    public static string GetLoggerHeaders(this HttpContext context, List<string>? blackListHeaders, List<string>? whiteListHeaders, bool isRequestHeader)
    {
        string headers;
        var rawHeaders = isRequestHeader ? context.Request.Headers : context.Response.Headers;
        if (blackListHeaders != null && blackListHeaders.Any())
            headers = rawHeaders
                .Where(x => !blackListHeaders.Contains(x.Key.ToLower()))
                .Select(x => x.ToString()).Serialize(new System.Text.Json.JsonSerializerOptions() { WriteIndented = true });
        else if (whiteListHeaders != null && whiteListHeaders.Any())
            headers = rawHeaders
                .Where(x => whiteListHeaders.Contains(x.Key.ToLower()))
                .Select(x => x.ToString()).Serialize(new System.Text.Json.JsonSerializerOptions() { WriteIndented = true });
        else
            headers = rawHeaders.Select(x => x.ToString()).Serialize(new System.Text.Json.JsonSerializerOptions() { WriteIndented = true });

        return headers;
    }
}