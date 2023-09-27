using Jinget.Core.ResponseResults.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Jinget.Core.ResponseResults
{
    public class JingetUnauthorizedResult : JsonResult
    {
        public JingetUnauthorizedResult(string message) : base(new CustomMessageModel(message)) =>
            StatusCode = StatusCodes.Status401Unauthorized;
    }
}