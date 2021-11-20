using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace VaccineHub.MockServices.payment.Endpoints.Payment.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CreditCardType
    {
        [EnumMember(Value = "AX")]
        Amex,
        [EnumMember(Value = "CA")]
        MasterCard,
        [EnumMember(Value = "VI")]
        Visa,
        [EnumMember(Value = "DS")]
        Discover,
        [EnumMember(Value = "TP")]
        UniversalAirTravelPlan,
        [EnumMember(Value = "SW")]
        SwitchMaestro,
        [EnumMember(Value = "VD")]
        VisaDebit,
        [EnumMember(Value = "VE")]
        Electron,
        [EnumMember(Value = "DM")]
        MasterCardDebit
    }
}