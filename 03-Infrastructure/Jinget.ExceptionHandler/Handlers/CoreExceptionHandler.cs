using Jinget.ExceptionHandler.Entities.Log;

namespace Jinget.ExceptionHandler.Handlers;

public abstract class CoreExceptionHandler(ILogger<CoreExceptionHandler> logger, IHostEnvironment env, bool useGlobalExceptionHandler)
{
    protected async ValueTask<bool> HandleAsync(HttpContext httpContext, Exception exception, int statusCode, CancellationToken cancellationToken)
    {
        var logEntity = LogModel.GetNewErrorObject(httpContext);
        logEntity.Description = JsonConvert.SerializeObject(new
        {
            exception.Message,
            exception.StackTrace,
            exception.Data
        });
        logger.LogError(JsonConvert.SerializeObject(logEntity));
        if (useGlobalExceptionHandler)
        {
            httpContext.Response.StatusCode = statusCode;
            var problemDetails = new ResponseResult<ProblemDetails>(CreateProblemDetails(httpContext, exception, statusCode));
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
            return true;
        }
        else
            return false;
    }

    protected virtual ProblemDetails CreateProblemDetails(in HttpContext context, in Exception exception, int statusCode)
    {
        var reasonPhrase = ReasonPhrases.GetReasonPhrase(statusCode);
        if (string.IsNullOrEmpty(reasonPhrase))
        {
            reasonPhrase = "Unhandled Exception!";
        }

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = reasonPhrase
        };
        problemDetails.Extensions.Add("message", exception.Message);

        if (env.IsProduction())
        {
            return problemDetails;
        }

        problemDetails.Detail = exception.ToString();
        problemDetails.Extensions.Add("traceId", context.TraceIdentifier);
        problemDetails.Extensions.Add("data", exception.Data);
        problemDetails.Extensions.Add("nodeId", Environment.MachineName);

        return problemDetails;
    }
}