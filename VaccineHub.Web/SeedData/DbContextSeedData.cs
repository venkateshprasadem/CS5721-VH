using System.Threading.Tasks;
using VaccineHub.Persistence;
using VaccineHub.Persistence.Entities;
using VaccineHub.Persistence.Types;

namespace VaccineHub.Web.SeedData
{
    public class DbContextSeedData
    {
        private readonly IVaccineHubDbContext _context;
        public DbContextSeedData(IVaccineHubDbContext context)
        {
            _context = context;
        }

        public async Task SeedDataAsync()
        {
            _context.ApiUsers.Add(new ApiUser
            {
                EmailId = "admin",
                IsActive = true,
                UserType = UserType.Admin,
                Password = "admin"
            });

            _context.ApiUsers.Add(new ApiUser
            {
                EmailId = "CS5721",
                IsActive = true,
                UserType = UserType.Admin,
                Password = "CS5721"
            });

            await _context.SaveChangesAsync();
        }
    }
}