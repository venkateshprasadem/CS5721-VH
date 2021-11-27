using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using VaccineHub.Persistence;
using VaccineHub.Service.Abstractions;
using VaccineHub.Service.Booking;
//using VaccineHub.Service.Models;
using VaccineHub.Persistence.Types;
using VaccineHub.Persistence.Entities;

namespace bookingservice.moq
{
    public class BookingserviceTest
    {
        private readonly IBookingService _sut;
        private readonly Mock<IServiceProvider> _serviceproviderMock = new Mock<IServiceProvider>();


        public BookingserviceTest()
        {
            _sut = new BookingService(_serviceproviderMock.Object);
        }

        private static VaccineHubDbContext CreateDbContext()
        {
            var builder = new DbContextOptionsBuilder<VaccineHubDbContext>();

            builder.UseInMemoryDatabase(Guid.NewGuid().ToString());

            return new VaccineHubDbContext(builder.Options);
        }

        [Test]
        public async Task GetCenter_ById_WhenCenterExists()
        {
            //Arrange
            var apiUser = new ApiUser
            {
                EmailId = "21004528@studentmail.ul.ie",
                IsActive = true,
                UserType = UserType.Customer,
                Password = "21004528"
            };
            var center = new VaccineHub.Persistence.Entities.Center
            {
                Id = "limerick",
                Name = "Hospital Community Center",
                Description = "Center",
                Telephone = "0123456789",
                EirCode = "V35 X2P1"
            };

            var product = new VaccineHub.Persistence.Entities.Product
            {
                Id = "pfizer",
                Name = "pfizer",
                Cost = 10,
                Doses = 2,
                MinIntervalInDays = 17,
                MaxIntervalInDays = 27,
                //Currency = Currency.Eur
            };

            var booking = new VaccineHub.Persistence.Entities.Booking
            {
                Product = product,
                Center = center,
                ApiUser = apiUser,
                AppointmentDate = new DateTime(2021, 11, 22),
                BookingType = VaccineHub.Persistence.Types.BookingType.Book,
                DosageType = VaccineHub.Persistence.Types.DosageType.First,
                PaymentInformation = new VaccineHub.Persistence.Entities.PaymentInformation
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
                    CreditCardType = VaccineHub.Persistence.Types.CreditCardType.Visa,
                    CardHolderFirstName = "Tom",
                    CardHolderLastName = "Cruise"
                }
            };
            var serviceScope = new Mock<IServiceScope>();
            serviceScope.Setup(x => x.ServiceProvider).Returns(_serviceproviderMock.Object);

            var serviceScopeFactory = new Mock<IServiceScopeFactory>();
            serviceScopeFactory
                .Setup(x => x.CreateScope())
                .Returns(serviceScope.Object);
            _serviceproviderMock
                .Setup(x => x.GetService(typeof(IServiceScopeFactory)))
                .Returns(serviceScopeFactory.Object);
            var vaccineHubDbContext = CreateDbContext();
            await vaccineHubDbContext.Bookings.AddAsync(booking, CancellationToken.None);
            await vaccineHubDbContext.SaveChangesAsync();

            _serviceproviderMock.Setup(s => s.GetService(typeof(IVaccineHubDbContext))).Returns(vaccineHubDbContext);


            //Act
            var result = await _sut.GetAllBookingsAsync("21004528@studentmail.ul.ie", CancellationToken.None);

            Assert.NotNull(result);

            //Assert.AreEqual(result., center.Name);
            //Assert.AreEqual(result.Telephone, center.Telephone);
            //Assert.AreEqual(result.EirCode, center.EirCode);
            //Assert.AreEqual(result.Description, center.Description);
        }
        
        [Test]
        public async Task MakeOrCancelBooking()
        {
            //Arrange
           

            var booking = new VaccineHub.Service.Models.Booking
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
            serviceScope.Setup(x => x.ServiceProvider).Returns(_serviceproviderMock.Object);

            var serviceScopeFactory = new Mock<IServiceScopeFactory>();
            serviceScopeFactory
                .Setup(x => x.CreateScope())
                .Returns(serviceScope.Object);
            _serviceproviderMock
                .Setup(x => x.GetService(typeof(IServiceScopeFactory)))
                .Returns(serviceScopeFactory.Object);
            var vaccineHubDbContext = CreateDbContext();

            _serviceproviderMock.Setup(s => s.GetService(typeof(IVaccineHubDbContext))).Returns(vaccineHubDbContext);


            //Act
            var result = await _sut.MakeOrCancelBookingAsync("21004528@studentmail.ul.ie", booking,  CancellationToken.None);

            Assert.IsTrue(result);

            //Assert.AreEqual(result., center.Name);
            //Assert.AreEqual(result.Telephone, center.Telephone);
            //Assert.AreEqual(result.EirCode, center.EirCode);
            //Assert.AreEqual(result.Description, center.Description);
        }
    }
}   