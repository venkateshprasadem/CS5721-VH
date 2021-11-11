using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace VaccineHub.Service.Abstractions
{
    public interface IBookingService
    {
        Task<string> MakeBookingAsync(string apiUserId, Models.Booking booking, CancellationToken token);
        Task<Models.Booking> GetBookingAsync(string apiUserId, string bookingId, CancellationToken token);
        Task<List<Models.Booking>> GetAllBookingsAsync(string apiUserId, CancellationToken token);
        Task CancelBookingAsync(string apiUserId, Models.Booking booking, CancellationToken token);
    }
}