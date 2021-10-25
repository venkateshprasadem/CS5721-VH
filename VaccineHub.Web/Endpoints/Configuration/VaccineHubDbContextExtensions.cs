using System.Threading;
using System.Threading.Tasks;
using VaccineHub.Persistence;

namespace VaccineHub.Web.Endpoints.Configuration
{
    public static class VaccineHubDbContextExtensions
    {
        public static async Task<bool> IsValidEmailId(this IVaccineHubDbContext dbContext, string emailId, CancellationToken cancellationToken)
        {
            var existingUser = await dbContext.ApiUsers.FindAsync(new object[] {emailId}, cancellationToken);
            return existingUser == null;
        }

        public static async Task<bool> IsEmailIdPresent(this IVaccineHubDbContext dbContext, string emailId, CancellationToken cancellationToken)
        {
            var existingUser = await dbContext.ApiUsers.FindAsync(new object[] {emailId}, cancellationToken);
            return existingUser != null;
        }
    }
}