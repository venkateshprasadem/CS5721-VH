using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace VaccineHub.MockServices.Endpoints.NotifyDetails.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum BookingStatus
    {
        [EnumMember(Value = "Book")]
        Book,
        
        [EnumMember(Value = "Cancel")]
        Cancel
    }
}