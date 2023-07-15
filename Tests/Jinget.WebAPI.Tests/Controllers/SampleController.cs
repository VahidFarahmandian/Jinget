using Microsoft.AspNetCore.Mvc;

namespace Jinget.WebAPI.Tests.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SampleController : ControllerBase
    {
        [HttpPost]
        public SampleModel Save(SampleModel model)
        {
            return model;
        }
    }
}