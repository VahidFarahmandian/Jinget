using Jinget.Core.ExtensionMethods.HttpContext;

[assembly: InternalsVisibleTo("Jinget.Logger")]
namespace Jinget.ExceptionHandler.Entities.Log;

[Entity(ElasticSearchEnabled = true)]
public class LogModel : BaseEntity<long>
{
    private string? partitionKey;

    public LogModel()
    {
    }

    internal static LogModel GetNew(HttpContext? context = null)
    {
        var log = new LogModel
        {
            SubSystem = AppDomain.CurrentDomain.FriendlyName,
            TimeStamp = DateTime.UtcNow,
            EnvironmentInfo = new
            {
                Environment.MachineName,
                CLRVersion = Environment.Version,
                OSVersion = Environment.OSVersion.VersionString.Serialize(),
                Environment.ProcessId,
                WorkingThreadUsername = Environment.UserName,
                WorkingThreadUserDomain = Environment.UserDomainName
            }.Serialize()
        };

        if (context != null)
        {
            context.Request.Headers.TryGetValue("Referer", out StringValues pageUrl);
            log.PageUrl = pageUrl.FirstOrDefault();
            log.TraceIdentifier = context.TraceIdentifier;
            log.PartitionKey = context.GetLoggerPartitionKey();
            log.Username = context.User.Identity?.Name;
            log.Method = context.Request.Method;
            log.Url = context.Request.GetDisplayUrl();
            log.IP = context.GetClientIpAddress();

            var requestDateTime = context.GetRequestDateTime();
            log.ElapsedMilliseconds = requestDateTime == null ? 0 : (DateTime.UtcNow - requestDateTime.Value).TotalMilliseconds;
        }

        return log;
    }

    public static LogModel GetNewRequestObject(HttpContext context, string body, string headers)
    {
        var obj = GetNew(context);
        obj.Type = LogType.Request;
        obj.Body = body;
        obj.Headers = headers;
        context.SetRequestDateTime(obj.TimeStamp);
        obj.AdditionalData = context.GetLoggerAdditionalData(isRequestData: true);
        obj.ContentLength = context.GetRequestContentLength(body);
        return obj;
    }

    public static LogModel GetNewCustomObject(HttpContext context)
    {
        var obj = GetNew(context);
        obj.Type = LogType.CustomLog;
        return obj;
    }

    public static LogModel GetNewErrorObject(HttpContext? context = null)
    {
        var obj = GetNew(context);
        obj.Type = LogType.Error;
        obj.Severity = LogLevel.Error.ToString();
        return obj;
    }

    public static LogModel GetNewResponseObject(HttpContext context, string body, string headers)
    {
        var obj = GetNew(context);
        obj.Type = LogType.Response;
        obj.Body = body;
        obj.Headers = headers;
        obj.StatusCode = context.Response.StatusCode;
        obj.AdditionalData = context.GetLoggerAdditionalData(isRequestData: false);
        obj.ContentLength = context.GetResponseContentLength(body);
        return obj;
    }

    public DateTime TimeStamp { get; set; }
    public string? Url { get; set; }
    public string? Description { get; set; }
    public int? StatusCode { get; set; }
    public string? EnvironmentInfo { get; set; }

    /// <summary>
    /// How many milliseconds have passed since the start of the request? 
    /// </summary>
    public double ElapsedMilliseconds { get; set; }

    /// <summary>
    /// <seealso cref="AppDomain.CurrentDomain.FriendlyName"/>
    /// </summary>
    public string? SubSystem { get; set; }

    /// <summary>
    /// if PartitionKey is not set the <see cref="SubSystem"/> value will be used as PartitionKey
    /// this is crucial when CreateIndexPerPartition is set to true
    /// This property is specific to Elasticsearch logging
    /// </summary>
    public string? PartitionKey { get => string.IsNullOrWhiteSpace(partitionKey) ? SubSystem : partitionKey; set => partitionKey = value; }

    /// <summary>
    /// unique identifier for a request and response. value is read from HttpContext.TraceIdentifier
    /// </summary>
    public string? TraceIdentifier { get; set; }

    /// <summary>
    /// Http Method
    /// </summary>
    public string? Method { get; set; }

    /// <summary>
    /// Request or response body
    /// </summary>
    public string? Body { get; set; }

    /// <summary>
    /// Request/Response headers. Blacklist headers will not be logged
    /// </summary>
    public string? Headers { get; set; }

    /// <summary>
    /// Request ip
    /// </summary>
    public string? IP { get; set; }

    /// <summary>
    /// Is the record for request or response?
    /// </summary>
    public LogType Type { get; set; }

    public string? TypeDescription => Type.GetDescription();

    /// <summary>
    /// Page url initiating the request. 
    /// Read page url from 'Referer' header
    /// </summary>
    public string? PageUrl { get; set; }

    /// <summary>
    /// total length of the request or response
    /// </summary>
    public long ContentLength { get; set; }

    /// <summary>
    /// Request or response 'AdditionalData' header data plus HttpContext.Items['AdditionalData'] value
    /// </summary>
    public string? AdditionalData { get; set; }

    /// <summary>
    /// Request username
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// <seealso cref="LogLevel"/>
    /// </summary>
    public string? Severity { get; set; } = LogLevel.Information.ToString();

    /// <summary>
    /// This property only filled whenever calling <seealso cref="ILoggerExtensions.LogCustom"/> method.
    /// </summary>
    public string? CallerFilePath { get; set; } = null;

    /// <summary>
    /// This property only filled whenever calling <seealso cref="ILoggerExtensions.LogCustom"/> method.
    /// </summary>
    public long? CallerLineNumber { get; set; } = null;

    /// <summary>
    /// This property only filled whenever calling <seealso cref="ILoggerExtensions.LogCustom"/> method.
    /// </summary>
    public string? CallerMember { get; set; } = null;
}