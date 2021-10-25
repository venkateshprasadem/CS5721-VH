using Microsoft.AspNetCore.Mvc;

namespace VaccineHub.Web.Endpoints.Health
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
            return Ok("Vaccine Hub Application is up and running...");   
        }
    }
}