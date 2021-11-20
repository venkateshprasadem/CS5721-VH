using System.Threading;
using Microsoft.AspNetCore.Mvc;
using VaccineHub.MockServices.payment.Endpoints.Payment.Models;

namespace VaccineHub.MockServices.payment.Endpoints.Payment
{
    public sealed class PaymentController : ControllerBase
    {
        [HttpPost]
        [Route("CreditPayment")]
        [ProducesResponseType(200)]
        public IActionResult CreditPayment([FromBody] PaymentInformation paymentInformation, CancellationToken token)
        {
            if (!paymentInformation.CountryCode.Equals("IE"))
            {
                return BadRequest("Country Unsupported");
            }

            if (paymentInformation.CardNumber.Equals("4111111111111112"))
            {
                return Forbid();
            }

            return Ok();
        }

        [HttpPost]
        [Route("DebitPayment")]
        [ProducesResponseType(200)]
        public IActionResult DebitPayment([FromBody] PaymentInformation paymentInformation, CancellationToken token)
        {
            if (!paymentInformation.CountryCode.Equals("IE"))
            {
                return BadRequest("Country Unsupported");
            }

            if (paymentInformation.CardNumber.Equals("4111111111111112"))
            {
                return Forbid();
            }

            return Ok();
        }
    }
}