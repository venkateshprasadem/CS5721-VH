using System.Threading;
using System.Threading.Tasks;

namespace VaccineHub.Service.Booking
{
    public interface IBookingPreConditionStrategy
    {
        public Task Verify(string apiUserId, Models.Booking booking, CancellationToken cancellationToken);
    }
}