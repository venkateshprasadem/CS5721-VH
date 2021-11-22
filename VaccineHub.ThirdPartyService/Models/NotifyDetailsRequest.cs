using System;

namespace VaccineHub.ThirdPartyService.Models
{
    public class NotifyDetailsRequest
    {
        public string EmailId { get; set; }
        public string ProductId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public BookingType BookingType { get; set; }
    }
}