namespace VaccineHub.Service.Abstractions
{
    public interface IVisitor {
        void VisitProvisionalVaccinationCertificate(Persistence.Entities.Booking booking);
        void VisitFinalVaccinationCertificate(Persistence.Entities.Booking obj);
    }
}