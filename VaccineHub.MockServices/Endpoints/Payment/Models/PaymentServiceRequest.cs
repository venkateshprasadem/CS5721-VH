using Newtonsoft.Json;

namespace VaccineHub.MockServices.Endpoints.Payment.Models
{
    public class PaymentServiceRequest
    {
        [JsonProperty(PropertyName = "Cost")]
        public decimal? Cost { get; set; }

        [JsonProperty(PropertyName = "TransactionType")]
        public TransactionType? TransactionType { get; set; }

        [JsonProperty(PropertyName = "PaymentInformation")]
        public PaymentInformation PaymentInformation { get; set; }
    }
}