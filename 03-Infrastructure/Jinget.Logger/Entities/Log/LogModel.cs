using Jinget.Core.ExtensionMethods.Enums;

namespace Jinget.Logger.Entities.Log;

[Entity(ElasticSearchEnabled = true)]
public class LogModel : BaseEntity<long> //LogBaseEntity
{
    public DateTime TimeStamp { get; set; }
    public string Url { get; set; }
    public string Description { get; set; }

    /// <summary>
    /// <seealso cref="AppDomain.CurrentDomain.FriendlyName"/>
    /// </summary>
    public string SubSystem { get; set; }

    /// <summary>
    /// This is used when the CreateIndexPerPartition is set to true.
    /// This property is specific to Elasticsearch logging
    /// </summary>
    public string ParitionKey { get; set; }

    /// <summary>
    /// unique identifier for a request and response. value is read from HttpContext.TraceIdentifier
    /// </summary>
    public string TraceIdentifier { get; set; }

    /// <summary>
    /// Http Method
    /// </summary>
    public string Method { get; set; }

    /// <summary>
    /// Request or response body
    /// </summary>
    public string Body { get; set; }

    /// <summary>
    /// Request/Response headers. Blacklist headers will not be logged
    /// </summary>
    public string Headers { get; set; }

    /// <summary>
    /// Request ip
    /// </summary>
    public string IP { get; set; }

    /// <summary>
    /// Is the record for request or response?
    /// </summary>
    public LogType Type { get; set; }

    public string TypeDescription => Type.GetDescription();

    /// <summary>
    /// Page url initiating the request. 
    /// Read page url from 'Referer' header
    /// </summary>
    public string PageUrl { get; set; }

    /// <summary>
    /// total length of the request or response
    /// </summary>
    public long ContentLength { get; set; }

    /// <summary>
    /// Request or response 'AdditionalData' header data plus HttpContext.Items['AdditionalData'] value
    /// </summary>
    public string AdditionalData { get; set; }

    /// <summary>
    /// Request username
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// <seealso cref="LogLevel"/>
    /// </summary>
    public string Severity { get; set; } = Microsoft.Extensions.Logging.LogLevel.Information.ToString();

    /// <summary>
    /// This property only filled whenever calling <seealso cref="ILoggerExtensions.LogCustom"/> method.
    /// </summary>
    public string CallerFilePath { get; set; } = null;

    /// <summary>
    /// This property only filled whenever calling <seealso cref="ILoggerExtensions.LogCustom"/> method.
    /// </summary>
    public long? CallerLineNumber { get; set; } = null;

    /// <summary>
    /// This property only filled whenever calling <seealso cref="ILoggerExtensions.LogCustom"/> method.
    /// </summary>
    public string CallerMember { get; set; } = null;
}