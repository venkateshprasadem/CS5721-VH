using System.Threading;
using System.Threading.Tasks;
using VaccineHub.Persistence;

namespace VaccineHub.Web.Endpoints.Product
{
    public static class VaccineHubDbContextProductExtensions
    {
        public static async Task<bool> IsValidProductId(this IVaccineHubDbContext dbContext, string productId, CancellationToken cancellationToken)
        {
            var existingProduct = await dbContext.Products.FindAsync(new object[] {productId}, cancellationToken);
            return existingProduct == null;
        }

        public static async Task<bool> IsProductIdPresent(this IVaccineHubDbContext dbContext, string productId, CancellationToken cancellationToken)
        {
            var existingProduct = await dbContext.Products.FindAsync(new object[] {productId}, cancellationToken);
            return existingProduct != null;
        }
    }
}