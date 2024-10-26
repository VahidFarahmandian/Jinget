namespace Jinget.Logger.ExceptionHandler;

public class ExceptionHandler<TCategoryName> : IExceptionHandler<TCategoryName>
{
    private readonly ILogger<TCategoryName> _logger;
    private readonly IHttpContextAccessor _accessor;

    public ExceptionHandler(ILogger<TCategoryName> logger, IHttpContextAccessor accessor)
    {
        _logger = logger;
        _accessor = accessor;
    }

    public void Handle(Exception ex, object details)
    {
        LogModel logEntity = new()
        {
            Description = JsonConvert.SerializeObject(new
            {
                ex.Message,
                Details = JsonConvert.SerializeObject(details),
                ex.StackTrace
            }),

            ParitionKey =
            _accessor.HttpContext.Items["jinget.log.partitionkey"] != null ?
            _accessor.HttpContext.Items["jinget.log.partitionkey"].ToString() :
            "",
            Severity = Microsoft.Extensions.Logging.LogLevel.Error.ToString(),

        };
        if (details is LogModel entity)
        {
            logEntity.RequestId = entity.RequestId;
            logEntity.SubSystem = entity.SubSystem;
            logEntity.Url = entity.Url;
            logEntity.TimeStamp = entity.TimeStamp;
        }
        else
        {
            logEntity.RequestId = new Guid(_accessor.HttpContext.Response.Headers["RequestId"].ToString());
            logEntity.SubSystem = AppDomain.CurrentDomain.FriendlyName;
        }
        _logger.LogError(JsonConvert.SerializeObject(logEntity));
        return;
    }
}