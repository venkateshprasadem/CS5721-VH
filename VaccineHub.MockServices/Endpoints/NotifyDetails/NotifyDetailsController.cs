using System.Threading;
using Microsoft.AspNetCore.Mvc;
using VaccineHub.MockServices.Endpoints.NotifyDetails.Models;

namespace VaccineHub.MockServices.Endpoints.NotifyDetails
{
    public sealed class NotifyDetailsController : ControllerBase
    {
        [HttpPost]
        [Route("NotifyDetails")]
        [ProducesResponseType(200)]
        public IActionResult CreditPayment([FromBody] NotifyDetailsRequest notifyDetailsRequest,
            CancellationToken token)
        {
            return Ok();
        }
    }
}