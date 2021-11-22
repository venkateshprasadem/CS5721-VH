using System.Threading;
using System.Threading.Tasks;

namespace VaccineHub.Service.Booking
{
    public interface IFilter {
        public Task Verify(string apiUserId, Models.Booking booking, CancellationToken cancellationToken);
    }
}