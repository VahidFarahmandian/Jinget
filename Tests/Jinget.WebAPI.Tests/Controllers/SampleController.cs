using Microsoft.AspNetCore.Mvc;

namespace Jinget.WebAPI.Tests.Controllers;

[ApiController]
[Route("[controller]")]
public class SampleController(ILogger<SampleController> logger) : ControllerBase
{
    public ILogger<SampleController> Logger { get; } = logger;

    [HttpPost]
    public SampleModel Save(SampleModel model)
    {
        Logger.LogDebug("Sample Custom message!");
        return model;
    }

    [HttpGet]
    public void Get() => throw new Exception("Sample Exception!");
}