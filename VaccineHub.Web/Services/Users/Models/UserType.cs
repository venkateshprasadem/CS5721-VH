using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace VaccineHub.Web.Services.Users.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum UserType
    {
        [EnumMember(Value = "Customer")]
        Customer,

        [EnumMember(Value = "VaccineAdministrator")]
        VaccineAdministrator,

        [EnumMember(Value = "Admin")]
        Admin
    }
}