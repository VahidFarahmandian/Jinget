using System;
using Jinget.Logger.Entities.Log;
using Jinget.Logger.Entities.Log.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Jinget.Logger.ExceptionHandler
{
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
            ErrorLog logEntity = new()
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
                Severity = LogLevel.Error.ToString()
            };
            if (details is LogBaseEntity entity)
            {
                logEntity.RequestId = entity.RequestId;
                logEntity.SubSystem = entity.SubSystem;
                logEntity.When = entity.When;
                logEntity.Url = entity.Url;
            }
            else
            {
                logEntity.RequestId = new Guid(_accessor.HttpContext.Response.Headers["RequestId"].ToString());
                logEntity.SubSystem = AppDomain.CurrentDomain.FriendlyName;
                logEntity.When = DateTime.Now;
            }
            _logger.LogError(JsonConvert.SerializeObject(logEntity));
            return;
        }
    }
}