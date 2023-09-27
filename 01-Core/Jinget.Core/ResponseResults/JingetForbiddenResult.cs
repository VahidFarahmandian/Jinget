using Jinget.Core.ResponseResults.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Jinget.Core.ResponseResults
{
    public class JingetForbiddenResult : JsonResult
    {
        public JingetForbiddenResult(string message) : base(new CustomMessageModel(message)) =>
            StatusCode = StatusCodes.Status403Forbidden;
    }
}