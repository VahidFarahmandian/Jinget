namespace Jinget.ExceptionHandler.Extensions;

public static class HttpContextExtensions
{
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
    public static string GetLoggerPartitionKey(this HttpContext context) =>
        context.Items["jinget.log.partitionkey"]?.ToString();

    /// <summary>
    /// set the partition key used for logging
    /// </summary>
    public static void SetLoggerPartitionKey(this HttpContext context, string key) =>
        context.Items.Add("jinget.log.partitionkey", key);

    /// <summary>
    /// set additional data used for logging
    /// </summary>
    public static string GetLoggerAdditionalData(this HttpContext context, bool isRequestData)
    {
        var rawHeaders = isRequestData ? context.Request.Headers : context.Response.Headers;
        var additionalData = new List<string>();
        if (!string.IsNullOrWhiteSpace(rawHeaders["AdditionalData"].ToString()))
            additionalData.Add(rawHeaders["AdditionalData"]);

        if (!string.IsNullOrWhiteSpace(context.Items["AdditionalData"]?.ToString()))
            additionalData.Add(context.Items["AdditionalData"]?.ToString());

        return additionalData.Any() ? JsonConvert.SerializeObject(additionalData) : null;
    }

    /// <summary>
    /// get request/response headers used for logging
    /// </summary>
    public static string GetLoggerHeaders(this HttpContext context, List<string> blackListHeaders, List<string> whiteListHeaders, bool isRequestHeader)
    {
        string headers;
        var rawHeaders = isRequestHeader ? context.Request.Headers : context.Response.Headers;
        if (blackListHeaders.Any())
            headers = JsonConvert.SerializeObject(rawHeaders
                .Where(x => !blackListHeaders.Contains(x.Key.ToLower()))
                .Select(x => x.ToString()), Formatting.Indented);
        else if (whiteListHeaders.Any())
            headers = JsonConvert.SerializeObject(rawHeaders
                .Where(x => whiteListHeaders.Contains(x.Key.ToLower()))
                .Select(x => x.ToString()), Formatting.Indented);
        else
            headers = JsonConvert.SerializeObject(rawHeaders.Select(x => x.ToString()), Formatting.Indented);

        return headers;
    }
}