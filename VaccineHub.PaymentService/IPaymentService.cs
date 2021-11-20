using System.Threading;
using System.Threading.Tasks;
using VaccineHub.PaymentService.Models;

namespace VaccineHub.PaymentService
{
    public interface IPaymentService
    {
        public Task DebitPaymentAsync(PaymentInformation paymentInformation, CancellationToken cancellationToken);

        public Task CreditPaymentAsync(PaymentInformation paymentInformation, CancellationToken cancellationToken);
    }
}