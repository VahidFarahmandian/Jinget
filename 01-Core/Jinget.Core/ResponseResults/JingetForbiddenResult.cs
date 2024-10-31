namespace Jinget.Core.ResponseResults;

public class JingetForbiddenResult : JsonResult
{
    public JingetForbiddenResult(string message) : base(new CustomMessageModel(message)) =>
        StatusCode = StatusCodes.Status403Forbidden;
}