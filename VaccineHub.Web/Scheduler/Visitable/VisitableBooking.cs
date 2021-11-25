using System;
using VaccineHub.Persistence.Types;
using VaccineHub.Service.Abstractions;

namespace VaccineHub.Web.Scheduler.Visitable
{
    // If we want to visit some other type of Booking in future
    // Like home vaccination booking or distinguish between payment successful or failed booking
    internal class VisitableBooking : IVisitable {
        private readonly Persistence.Entities.Booking _booking;

        public VisitableBooking(Persistence.Entities.Booking booking)
        {
            _booking = booking;
        }

        public void Accept(IVisitor visitor)
        {
            switch (_booking.DosageType)
            {
                case DosageType.First:
                    visitor.VisitProvisionalVaccinationCertificate(_booking);
                    break;
                case DosageType.Second:
                    visitor.VisitFinalVaccinationCertificate(_booking);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}