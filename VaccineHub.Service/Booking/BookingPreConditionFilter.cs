using System.Threading;
using System.Threading.Tasks;

namespace VaccineHub.Service.Booking
{
    public class BookingPreConditionFilter : IFilter
    {
        private readonly IBookingPreConditionStrategy _bookingPreConditionStrategy;

        public BookingPreConditionFilter(IBookingPreConditionStrategy bookingPreConditionStrategy)
        {
            _bookingPreConditionStrategy = bookingPreConditionStrategy;
        }

        public Task Verify(string apiUserId, Models.Booking booking, CancellationToken cancellationToken)
        {
            return _bookingPreConditionStrategy.Verify(apiUserId, booking, cancellationToken);
        }
    }
}