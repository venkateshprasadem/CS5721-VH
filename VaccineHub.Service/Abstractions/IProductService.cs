using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace VaccineHub.Service.Abstractions {
    public interface IProductService
    {
        Task<Models.Product> GetProductAsync(string productId, CancellationToken cancellationToken);
        Task<bool> AddProductAsync(Models.Product product, CancellationToken cancellationToken);
        Task<bool> UpdateProductAsync(Models.Product product, CancellationToken cancellationToken);
        Task<List<Models.Product>> GetAllProductsAsync(CancellationToken cancellationToken);
    }
}