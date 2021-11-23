using System.Threading;
using System.Threading.Tasks;

namespace VaccineHub.Service.Booking
{
    public class Booking : IBooking
    {
        private readonly IBookingOperationStrategy _bookingOperationStrategy;

        public Booking(IBookingOperationStrategy bookingOperationStrategy)
        {
            _bookingOperationStrategy = bookingOperationStrategy;
        }

        public Task<bool> PerformOperationAsync(string apiUserId, Models.Booking booking, CancellationToken cancellationToken)
        {
            return _bookingOperationStrategy.PerformOperationAsync(apiUserId, booking, cancellationToken);
        }
    }
}