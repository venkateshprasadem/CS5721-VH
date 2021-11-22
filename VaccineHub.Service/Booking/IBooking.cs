using System.Threading;
using System.Threading.Tasks;

namespace VaccineHub.Service.Booking
{
    internal interface IBooking
    {
        public Task<bool> PerformOperationAsync(string apiUserId, Models.Booking booking,
            CancellationToken cancellationToken);
    }
}