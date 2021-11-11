using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace VaccineHub.Web.Types
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum DosageType
    {
        [EnumMember(Value = "First")]
        First,
        [EnumMember(Value = "Second")]
        Second
    }
}