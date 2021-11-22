namespace VaccineHub.ThirdPartyService.Models
{
    public class PaymentServiceRequest
    {
        public decimal? Cost { get; set; }

        public TransactionType? TransactionType { get; set; }
        public PaymentInformation PaymentInformation { get; set; }
    }
}