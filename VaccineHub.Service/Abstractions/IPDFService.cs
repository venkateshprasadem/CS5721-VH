using System;
using System.IO;
using System.Threading.Tasks;

namespace VaccineHub.Service.Abstractions
{
    public interface IPdfService
    {
        void GenerateCertificate(string ProductName, string dosage, string center, string mailId, DateTime AppoinmentData);
    }
}