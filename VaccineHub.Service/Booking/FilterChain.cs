using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace VaccineHub.Service.Booking
{
    internal class FilterChain
    {
        private readonly List<IFilter> _filters = new();
        private IBooking _target;

        public void AddFilter(IFilter filter)
        {
            _filters.Add(filter);
        }

        public async Task Execute(string apiUserId, Models.Booking booking, CancellationToken cancellationToken)
        {
            foreach (var filter in _filters)
            {
                await filter.Verify(apiUserId, booking, cancellationToken);
            }

            await _target.PerformOperationAsync(apiUserId, booking, cancellationToken);
        }

        public void SetTarget(IBooking target){
            _target = target;
        }
    }
}