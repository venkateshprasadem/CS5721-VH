using System.Threading;
using System.Threading.Tasks;

namespace VaccineHub.ThirdPartyService
{
    public interface IThirdPartyService
    {
        public Task CallAsync<T>(T t, CancellationToken cancellationToken);
    }
}