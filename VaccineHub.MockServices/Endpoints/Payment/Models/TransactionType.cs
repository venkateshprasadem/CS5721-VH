using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace VaccineHub.MockServices.Endpoints.Payment.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TransactionType
    {
        [EnumMember(Value = "Credit")]
        Credit,
        
        [EnumMember(Value = "Debit")]
        Debit
    }
}