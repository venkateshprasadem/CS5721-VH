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
                EmailId = "admin@studentmail.ul.ie",
                IsActive = true,
                UserType = UserType.Admin,
                Password = "admin"
            });

            _context.ApiUsers.Add(new ApiUser
            {
                EmailId = "21004528@studentmail.ul.ie",
                IsActive = true,
                UserType = UserType.Customer,
                Password = "21004528"
            });

            _context.ApiUsers.Add(new ApiUser
            {
                EmailId = "21197091@studentmail.ul.ie",
                IsActive = true,
                UserType = UserType.Customer,
                Password = "21197091"
            });

            _context.ApiUsers.Add(new ApiUser
            {
                EmailId = "21017301@studentmail.ul.ie",
                IsActive = true,
                UserType = UserType.Customer,
                Password = "21017301"
            });

            var center = new Center
            {
                Id = "limerick",
                Name = "Hospital Community Center",
                Description = "Center",
                Telephone = "0123456789",
                EirCode = "V35 X2P1"
            };

            var product = new Product
            {
                Id = "pfizer",
                Name = "pfizer",
                Cost = 10,
                Doses = 2,
                MinIntervalInDays = 17,
                MaxIntervalInDays = 27,
                Currency = Currency.Eur
            };

            _context.Centers.Add(center);

            _context.Products.Add(product);

            _context.Inventories.Add(new Inventory
            {
                Product = product,
                Center = center,
                Stock = 50
            });
            
            await _context.SaveChangesAsync();
        }
    }
}