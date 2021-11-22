using Newtonsoft.Json;

namespace VaccineHub.MockServices.Endpoints.Payment.Models
{
    public sealed class PaymentInformation
    {
        [JsonProperty(PropertyName = "CreditCardType")]
        public CreditCardType CreditCardType { get; set; }

        [JsonProperty(PropertyName = "CardHolderFirstName")]
        public string CardHolderFirstName { get; set; }

        [JsonProperty(PropertyName = "CardHolderLastName")]
        public string CardHolderLastName { get; set; }

        [JsonProperty(PropertyName = "CardNumber")]
        public string CardNumber { get; set; }

        [JsonProperty(PropertyName = "ExpiryMonth")]
        public int ExpiryMonth { get; set; }

        [JsonProperty(PropertyName = "ExpiryYear")]
        public int ExpiryYear { get; set; }

        [JsonProperty(PropertyName = "Cvv")]
        public string Cvv { get; set; }

        [JsonProperty(PropertyName = "AddressLine1")]
        public string AddressLine1 { get; set; }

        [JsonProperty(PropertyName = "AddressLine2")]
        public string AddressLine2 { get; set; }

        [JsonProperty(PropertyName = "City")]
        public string City { get; set; }

        [JsonProperty(PropertyName = "ProvinceState")]
        public string ProvinceState { get; set; }

        [JsonProperty(PropertyName = "CountryCode")]
        public string CountryCode { get; set; }

        [JsonProperty(PropertyName = "PostalCode")]
        public string PostalCode { get; set; }
    }
}