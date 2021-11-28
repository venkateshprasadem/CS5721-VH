using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using VaccineHub.Persistence;
using VaccineHub.Persistence.Entities;
using VaccineHub.Persistence.Types;
using VaccineHub.Service.Abstractions;
using VaccineHub.Service.Booking;
using VaccineHub.ThirdPartyService;
using Booking = VaccineHub.Persistence.Entities.Booking; //using VaccineHub.Service.Models;

namespace VaccineHubUnitTests
{
    public class BookingServiceTest
    {
        private readonly IBookingService _sut;
        private readonly Mock<IServiceProvider> _serviceProviderMock = new();
        private readonly Mock<IThirdPartyService> _thirdPartyServiceMock = new();

        public BookingServiceTest()
        {
            _sut = new BookingService(_serviceProviderMock.Object);
        }

        private static VaccineHubDbContext CreateDbContext()
        {
            var builder = new DbContextOptionsBuilder<VaccineHubDbContext>();

            builder.UseInMemoryDatabase(Guid.NewGuid().ToString());

            return new VaccineHubDbContext(builder.Options);
        }

        [Test]
        public async Task GetAllBookings_ByApiUserId()
        {
            //Arrange
            var apiUser = new ApiUser
            {
                EmailId = "21004528@studentmail.ul.ie",
                IsActive = true,
                UserType = UserType.Customer,
                Password = "21004528"
            };

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

            var booking = new Booking
            {
                Product = product,
                Center = center,
                ApiUser = apiUser,
                AppointmentDate = new DateTime(2021, 11, 22),
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

            var serviceScope = new Mock<IServiceScope>();
            serviceScope.Setup(x => x.ServiceProvider).Returns(_serviceProviderMock.Object);

            var serviceScopeFactory = new Mock<IServiceScopeFactory>();
            serviceScopeFactory
                .Setup(x => x.CreateScope())
                .Returns(serviceScope.Object);
            _serviceProviderMock
                .Setup(x => x.GetService(typeof(IServiceScopeFactory)))
                .Returns(serviceScopeFactory.Object);
            var vaccineHubDbContext = CreateDbContext();
            await vaccineHubDbContext.Bookings.AddAsync(booking, CancellationToken.None);
            await vaccineHubDbContext.SaveChangesAsync();

            _serviceProviderMock.Setup(s => s.GetService(typeof(IVaccineHubDbContext))).Returns(vaccineHubDbContext);


            //Act
            var result = await _sut.GetAllBookingsAsync("21004528@studentmail.ul.ie", CancellationToken.None);

            Assert.NotNull(result);

            Assert.AreEqual(result.Count, 1);
            Assert.AreEqual((int?)result.First().BookingType, (int?)booking.BookingType);
            Assert.AreEqual(result.First().AppointmentDate, booking.AppointmentDate);
            Assert.AreEqual(result.First().CenterId, booking.Center.Id);
            Assert.AreEqual(result.First().ProductId, booking.Product.Id);
            Assert.AreEqual((int?)result.First().BookingType, (int?)booking.BookingType);
            Assert.AreEqual((int?)result.First().DosageType, (int?)booking.DosageType);
            Assert.AreEqual(result.First().PaymentInformation.City, booking.PaymentInformation.City);
            Assert.AreEqual(result.First().PaymentInformation.Cvv, booking.PaymentInformation.Cvv);
            Assert.AreEqual(result.First().PaymentInformation.AddressLine1, booking.PaymentInformation.AddressLine1);
            Assert.AreEqual(result.First().PaymentInformation.AddressLine2, booking.PaymentInformation.AddressLine2);
            Assert.AreEqual(result.First().PaymentInformation.CardNumber, booking.PaymentInformation.CardNumber);
            Assert.AreEqual(result.First().PaymentInformation.CountryCode, booking.PaymentInformation.CountryCode);
            Assert.AreEqual(result.First().PaymentInformation.ExpiryMonth, booking.PaymentInformation.ExpiryMonth);
            Assert.AreEqual(result.First().PaymentInformation.ExpiryYear, booking.PaymentInformation.ExpiryYear);
            Assert.AreEqual(result.First().PaymentInformation.PostalCode, booking.PaymentInformation.PostalCode);
            Assert.AreEqual(result.First().PaymentInformation.ProvinceState, booking.PaymentInformation.ProvinceState);
            Assert.AreEqual((int)result.First().PaymentInformation.CreditCardType, (int)booking.PaymentInformation.CreditCardType);
            Assert.AreEqual(result.First().PaymentInformation.CardHolderFirstName, booking.PaymentInformation.CardHolderFirstName);
            Assert.AreEqual(result.First().PaymentInformation.CardHolderLastName, booking.PaymentInformation.CardHolderLastName);
        }
        
        [Test]
        public async Task MakeBooking()
        {
            //Arrange
            var apiUser = new ApiUser
            {
                EmailId = "21004528@studentmail.ul.ie",
                IsActive = true,
                UserType = UserType.Customer,
                Password = "21004528"
            };

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
                Center = center,
                Product = product,
                Stock = 10
            };

            var vaccineHubDbContext = CreateDbContext();
            await vaccineHubDbContext.ApiUsers.AddAsync(apiUser, CancellationToken.None);
            await vaccineHubDbContext.Centers.AddAsync(center, CancellationToken.None);
            await vaccineHubDbContext.Products.AddAsync(product, CancellationToken.None);
            await vaccineHubDbContext.Inventories.AddAsync(inventory, CancellationToken.None);
            await vaccineHubDbContext.SaveChangesAsync();

            var bookingRequest = new VaccineHub.Service.Models.Booking
            {
                BookingType = VaccineHub.Service.Models.BookingType.Book,
                DosageType = VaccineHub.Service.Models.DosageType.First,
                ProductId = "pfizer",
                CenterId = "limerick",
                AppointmentDate = new DateTime(2021, 11, 22),
                PaymentInformation = new VaccineHub.Service.Models.PaymentInformation
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
                    CreditCardType = VaccineHub.Service.Models.CreditCardType.Visa,
                    CardHolderFirstName = "Tom",
                    CardHolderLastName = "Cruise"
                }
            };

            var serviceScope = new Mock<IServiceScope>();
            serviceScope.Setup(x => x.ServiceProvider).Returns(_serviceProviderMock.Object);

            var serviceScopeFactory = new Mock<IServiceScopeFactory>();
            serviceScopeFactory
                .Setup(x => x.CreateScope())
                .Returns(serviceScope.Object);
            _serviceProviderMock
                .Setup(x => x.GetService(typeof(IServiceScopeFactory)))
                .Returns(serviceScopeFactory.Object);

            _serviceProviderMock.Setup(s => s.GetService(typeof(IVaccineHubDbContext))).Returns(vaccineHubDbContext);
            _serviceProviderMock.Setup(s => s.GetService(typeof(IThirdPartyService))).Returns(_thirdPartyServiceMock.Object);

            //Act
            var checkIfSuccessful = await _sut.MakeOrCancelBookingAsync("21004528@studentmail.ul.ie", bookingRequest,
                CancellationToken.None);

            Assert.IsTrue(checkIfSuccessful);

            var booking = new Booking
            {
                Product = product,
                Center = center,
                ApiUser = apiUser,
                AppointmentDate = new DateTime(2021, 11, 22),
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
            
            var result = await vaccineHubDbContext.Bookings.FirstOrDefaultAsync(CancellationToken.None);
            Assert.AreEqual(result.Center, booking.Center);
            Assert.AreEqual(result.Product, booking.Product);
            Assert.AreEqual(result.ApiUser, booking.ApiUser);
            Assert.AreEqual(result.AppointmentDate, booking.AppointmentDate);
            Assert.AreEqual((int?)result.BookingType, (int?)booking.BookingType);
            Assert.AreEqual((int?)result.DosageType, (int?)booking.DosageType);
            Assert.AreEqual(result.IsCertGenerated, booking.IsCertGenerated);
        }
   
        [Test]
        public async Task CancelBooking()
        {
            //Arrange
            var apiUser = new ApiUser
            {
                EmailId = "21004528@studentmail.ul.ie",
                IsActive = true,
                UserType = UserType.Customer,
                Password = "21004528"
            };

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
                Center = center,
                Product = product,
                Stock = 10
            };

            var existingBooking = new Booking
            {
                Product = product,
                Center = center,
                ApiUser = apiUser,
                AppointmentDate = new DateTime(2021, 11, 22),
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

            var vaccineHubDbContext = CreateDbContext();
            await vaccineHubDbContext.ApiUsers.AddAsync(apiUser, CancellationToken.None);
            await vaccineHubDbContext.Centers.AddAsync(center, CancellationToken.None);
            await vaccineHubDbContext.Products.AddAsync(product, CancellationToken.None);
            await vaccineHubDbContext.Inventories.AddAsync(inventory, CancellationToken.None);
            await vaccineHubDbContext.Bookings.AddAsync(existingBooking, CancellationToken.None); 
            await vaccineHubDbContext.SaveChangesAsync(CancellationToken.None);

            var cancelBookingRequest = new VaccineHub.Service.Models.Booking
            {
                BookingType = VaccineHub.Service.Models.BookingType.Cancel,
                DosageType = VaccineHub.Service.Models.DosageType.First,
                ProductId = "pfizer",
                CenterId = "limerick",
                AppointmentDate = new DateTime(2021, 11, 22),
                PaymentInformation = new VaccineHub.Service.Models.PaymentInformation
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
                    CreditCardType = VaccineHub.Service.Models.CreditCardType.Visa,
                    CardHolderFirstName = "Tom",
                    CardHolderLastName = "Cruise"
                }
            };

            var serviceScope = new Mock<IServiceScope>();
            serviceScope.Setup(x => x.ServiceProvider).Returns(_serviceProviderMock.Object);

            var serviceScopeFactory = new Mock<IServiceScopeFactory>();
            serviceScopeFactory
                .Setup(x => x.CreateScope())
                .Returns(serviceScope.Object);
            _serviceProviderMock
                .Setup(x => x.GetService(typeof(IServiceScopeFactory)))
                .Returns(serviceScopeFactory.Object);

            _serviceProviderMock.Setup(s => s.GetService(typeof(IVaccineHubDbContext))).Returns(vaccineHubDbContext);
            _serviceProviderMock.Setup(s => s.GetService(typeof(IThirdPartyService))).Returns(_thirdPartyServiceMock.Object);

            //Act
            var checkIfSuccessful = await _sut.MakeOrCancelBookingAsync("21004528@studentmail.ul.ie", cancelBookingRequest,
                CancellationToken.None);

            Assert.IsTrue(checkIfSuccessful);

            var booking = new Booking
            {
                Product = product,
                Center = center,
                ApiUser = apiUser,
                AppointmentDate = new DateTime(2021, 11, 22),
                BookingType = BookingType.Cancel,
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
            
            var result = await vaccineHubDbContext.Bookings.FirstOrDefaultAsync(CancellationToken.None);
            Assert.AreEqual(result.Center, booking.Center);
            Assert.AreEqual(result.Product, booking.Product);
            Assert.AreEqual(result.ApiUser, booking.ApiUser);
            Assert.AreEqual(result.AppointmentDate, booking.AppointmentDate);
            Assert.AreEqual((int?)result.BookingType, (int?)booking.BookingType);
            Assert.AreEqual((int?)result.DosageType, (int?)booking.DosageType);
            Assert.AreEqual(result.IsCertGenerated, booking.IsCertGenerated);
        }
    }
}   