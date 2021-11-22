using System.Threading;
using Microsoft.AspNetCore.Mvc;
using VaccineHub.MockServices.Endpoints.Payment.Models;

namespace VaccineHub.MockServices.Endpoints.Payment
{
    public sealed class PaymentController : ControllerBase
    {
        [HttpPost]
        [Route("Transact")]
        [ProducesResponseType(200)]
        public IActionResult CreditPayment([FromBody] PaymentServiceRequest paymentServiceRequest,
            CancellationToken token)
        {
            if (!paymentServiceRequest.PaymentInformation.CountryCode.Equals("IE"))
            {
                return BadRequest("Country Unsupported");
            }

            if (paymentServiceRequest.PaymentInformation.CardNumber.Equals("4111111111111112"))
            {
                return Forbid();
            }

            return Ok();
        }
    }
}