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
                Description = $"" +
                    $"Error Message: {ex.Message + Environment.NewLine} " +
                    $"Details: {JsonConvert.SerializeObject(details) + Environment.NewLine} " +
                    $"StackTrace:{ex.StackTrace}"
            };
            if (details is LogBaseEntity entity)
            {
                logEntity.RequestId = entity.RequestId;
                logEntity.Severity = LogLevel.Error.ToString();
                logEntity.SubSystem = entity.SubSystem;
                logEntity.When = entity.When;
                logEntity.Url = entity.Url;
            }
            else
            {
                logEntity.RequestId = new Guid(_accessor.HttpContext.Response.Headers["RequestId"].ToString());
                logEntity.Severity = LogLevel.Error.ToString();
                logEntity.SubSystem = AppDomain.CurrentDomain.FriendlyName;
                logEntity.When = DateTime.Now;
            }
            _logger.LogError(JsonConvert.SerializeObject(logEntity));
            return;
        }
    }
}