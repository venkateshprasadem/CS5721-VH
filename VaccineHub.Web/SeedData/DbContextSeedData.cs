using System;
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
            var apiUser = new ApiUser
            {
                EmailId = "21004528@studentmail.ul.ie",
                IsActive = true,
                UserType = UserType.Customer,
                Password = "21004528"
            };

            _context.ApiUsers.Add(apiUser);

            _context.ApiUsers.Add(new ApiUser
            {
                EmailId = "admin@studentmail.ul.ie",
                IsActive = true,
                UserType = UserType.Admin,
                Password = "admin"
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

            var inventory = new Inventory
            {
                Product = product,
                Center = center,
                Stock = 50
            };

            var booking = new Booking
            {
                Product = product,
                Center = center,
                ApiUser = apiUser,
                AppointmentDate = DateTime.Today.AddDays(-10),
                BookingType = BookingType.Book,
                DosageType = DosageType.First,
                PaymentInformation = new PaymentInformation
                {
                    CardNumber = "4111111111111111",
                    City = "Limerick",
                    AddressLine1 = "Bru Na Gruadan",
                    AddressLine2 = "Castletroy",
                    CountryCode = "IE",
                    ExpiryMonth = 10,
                    ExpiryYear = 22,
                    PostalCode = "V94 CTP6",
                    ProvinceState = "LI",
                    Cvv = "123",
                    CreditCardType = CreditCardType.Visa,
                    CardHolderFirstName = "Tom",
                    CardHolderLastName = "Cruise"
                }
            };

            _context.Centers.Add(center);

            _context.Products.Add(product);

            _context.Inventories.Add(inventory);

            _context.Bookings.Add(booking);

            await _context.SaveChangesAsync();
        }
    }
}