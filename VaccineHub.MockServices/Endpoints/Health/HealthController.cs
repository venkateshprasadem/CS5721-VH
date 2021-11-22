using Microsoft.AspNetCore.Mvc;

namespace VaccineHub.MockServices.Endpoints.Health
{
    [Route("[controller]")]
    public class HealthController : Controller
    {
        [HttpHead]
        [ApiExplorerSettings(IgnoreApi = true)]
        public void Head()
        {

        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Mock Payment Service Application is up and running...");   
        }
    }
}