using System;

namespace VaccineHub.Service.Models
{
    public class Booking
    {
        public BookingType? BookingType { get; set; }

        public DosageType? DosageType { get; set; }

        public string ProductId { get; set; }
 
        public string CenterId { get; set; }

        public DateTime AppointmentDate { get; set; }

        public PaymentInformation PaymentInformation { get; set; }
    }
}