using System.Threading;
using System.Threading.Tasks;

namespace VaccineHub.Service.Booking
{
    internal class FilterManager {
        private readonly FilterChain _filterChain;

        public FilterManager(IBooking target){
            _filterChain = new FilterChain();
            _filterChain.SetTarget(target);
        }

        public void SetFilter(IFilter filter){
            _filterChain.AddFilter(filter);
        }

        public Task FilterRequest(string apiUserId,
            Models.Booking booking,
            CancellationToken cancellationToken)
        {
            return _filterChain.Execute(apiUserId,
                booking,
                cancellationToken);
        }
    }
}