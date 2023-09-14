using Microsoft.AspNetCore.Mvc;

namespace Jinget.WebAPI.Tests.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SampleController : ControllerBase
    {
        public SampleController(ILogger<SampleController> logger)
        {
            Logger = logger;
        }

        public ILogger<SampleController> Logger { get; }

        [HttpPost]
        public SampleModel Save(SampleModel model)
        {
            Logger.LogDebug("Sample Custom message!");
            return model;
        }

        [HttpGet]
        public void Get()
        {
            throw new Exception("Sample Exception!");
        }
    }
}