using System.Threading;
using System.Threading.Tasks;

namespace VaccineHub.Service.Booking
{
    public interface IBookingOperationStrategy
    {
        public Task<bool> PerformOperationAsync(string apiUserId, Models.Booking booking,
            CancellationToken cancellationToken);
    }
}