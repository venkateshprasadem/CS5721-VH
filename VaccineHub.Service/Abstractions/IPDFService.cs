using System;
using System.Threading.Tasks;

namespace VaccineHub.Service.Abstractions
{
    public interface IPdfService
    {
        Task GenerateCertificateAndSend(string productName, string dosage, string center, string mailId, DateTime ap2pointmentDate);
    }
}