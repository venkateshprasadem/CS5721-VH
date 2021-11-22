using System;
using Newtonsoft.Json;

namespace VaccineHub.MockServices.Endpoints.NotifyDetails.Models
{
    public class NotifyDetailsRequest
    {
        [JsonProperty(PropertyName = "EmailId")]
        public string EmailId { get; set; }

        [JsonProperty(PropertyName = "ProductId")]
        public string ProductId { get; set; }

        [JsonProperty(PropertyName = "AppointmentDate")]
        public DateTime AppointmentDate { get; set; }

        [JsonProperty(PropertyName = "BookingStatus")]
        public BookingStatus? BookingStatus { get; set; }
    }
}