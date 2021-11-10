using System.Threading;
using System.Threading.Tasks;
using VaccineHub.Persistence;

namespace VaccineHub.Web.Endpoints.VaccineHubDbContextExtensions
{
    public static class VaccineHubDbContextCenterExtensions
    {
        public static async Task<bool> IsValidCenterId(this IVaccineHubDbContext dbContext, string centerId, CancellationToken cancellationToken)
        {
            var existingCenter = await dbContext.Centers.FindAsync(new object[] {centerId}, cancellationToken);
            return existingCenter == null;
        }

        public static async Task<bool> IsCenterIdPresent(this IVaccineHubDbContext dbContext, string centerId, CancellationToken cancellationToken)
        {
            var existingCenter = await dbContext.Centers.FindAsync(new object[] {centerId}, cancellationToken);
            return existingCenter != null;
        }
    }
}