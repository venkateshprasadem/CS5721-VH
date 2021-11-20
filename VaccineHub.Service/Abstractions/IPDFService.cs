using System.IO;
using System.Threading.Tasks;

namespace VaccineHub.Service.Abstractions
{
    public interface IPDFService
    {
        Task<Stream> GenerateCertificate();
        byte[] GeneratePdfReport();
    }
}