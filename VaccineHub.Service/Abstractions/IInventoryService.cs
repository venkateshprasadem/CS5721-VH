using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace VaccineHub.Service.Abstractions
{
    public interface IInventoryService
    {
        Task<bool> AddInventoryAsync(Models.Inventory inventory, CancellationToken cancellationToken);
        Task<bool> UpdateInventoryAsync(Models.Inventory inventory, CancellationToken cancellationToken);
        Task<List<Models.Inventory>> GetAllInventoriesAsync(CancellationToken cancellationToken);
        Task<IEnumerable<Models.Inventory>> GetAllInventoriesByCenterIdAsync(string centerId,
            CancellationToken cancellationToken);
        Task<IEnumerable<Models.Inventory>> GetAllInventoriesByProductIdAsync(string productId, CancellationToken cancellationToken); 
    }
}