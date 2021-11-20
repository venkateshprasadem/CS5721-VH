using System;
using VaccineHub.Web.Types;

namespace VaccineHub.Web.Models
{
    public class Booking
    {
        public BookingType? BookingType { get; set; }

        public DosageType? DosageType { get; set; }

        public DateTime AppointmentDate { get; set; }

        public string ProductId { get; set; }
 
        public string CenterId { get; set; }

        public PaymentInformation PaymentInformation { get; set; }
    }
}