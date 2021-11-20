using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace VaccineHub.Service.Abstractions
{
    public interface IBookingService
    {
        Task<bool> MakeOrCancelBookingAsync(string apiUserId, Models.Booking booking, CancellationToken cancellationToken);
        Task<List<Models.Booking>> GetAllBookingsAsync(string apiUserId, CancellationToken cancellationToken);
    }
}