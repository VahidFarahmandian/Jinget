namespace Jinget.Core.ResponseResults;

public class JingetUnauthorizedResult : JsonResult
{
    public JingetUnauthorizedResult(string message) : base(new CustomMessageModel(message)) =>
        StatusCode = StatusCodes.Status401Unauthorized;
}