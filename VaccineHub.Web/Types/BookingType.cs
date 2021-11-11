using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace VaccineHub.Web.Types
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum BookingType
    {
        [EnumMember(Value = "Book")]
        Book,
        [EnumMember(Value = "Cancel")]
        Cancel
    }
}