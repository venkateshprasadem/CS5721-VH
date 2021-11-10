using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace VaccineHub.Service.Abstractions {
    public interface ICenterService
    {
        Task<Models.Center> GetCenterAsync(string centerId, CancellationToken cancellationToken);
        Task<bool> AddCenterAsync(Models.Center center, CancellationToken cancellationToken);
        Task<bool> UpdateCenterAsync(Models.Center center, CancellationToken cancellationToken);
        Task<List<Models.Center>> GetAllCentersAsync(CancellationToken cancellationToken);
    }
}