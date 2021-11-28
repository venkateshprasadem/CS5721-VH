using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using VaccineHub.Persistence;
using VaccineHub.Web.Services.Users;
using VaccineHub.Web.Services.Users.Models; 
//using VaccineHub.Persistence.Entities;
//using VaccineHub.Service.Abstractions;

namespace VaccineHubUnitTests
{
    public class ApiUserServiceTest
    {
        private readonly IApiUsersDataProvider _sut;
        private readonly Mock<IServiceProvider> _serviceProviderMock = new();


        public ApiUserServiceTest()
        {
            _sut = new ApiUsersDataProvider(_serviceProviderMock.Object);
        }

        private static VaccineHubDbContext CreateDbContext()
        {
            var builder = new DbContextOptionsBuilder<VaccineHubDbContext>();

            builder.UseInMemoryDatabase(Guid.NewGuid().ToString());

            return new VaccineHubDbContext(builder.Options);
        }

        [Test]
        public async Task AddApiUser_WhenUserWithSameIdDoesNotExists()
        {
            //Arrange
             var apiUser = new ApiUser
            {
                EmailId = "21004528@studentmail.ul.ie",
                IsActive = true,
                UserType = UserType.Customer,
                Password = "21004528"
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
            _serviceProviderMock.Setup(s => s.GetService(typeof(IVaccineHubDbContext))).Returns(vaccineHubDbContext);

            //Act
            await _sut.AddUserAsync(apiUser, CancellationToken.None);
            //Assert
            var dbApiUser =
                await vaccineHubDbContext.ApiUsers.FindAsync(new object[] {apiUser.EmailId}, CancellationToken.None);
            Assert.NotNull(dbApiUser);
            
        }
        
        [Test]
        public async Task UpdateApiUser_WhenUserExists()
        {
            //Arrange
            var updatedApiUser = new ApiUser
            {
                EmailId = "21197091@studentmail.ul.ie",
                IsActive = true,
                UserType = UserType.Customer,
                Password = "21197091"
            };
            
            var existingApiUser = new VaccineHub.Persistence.Entities.ApiUser
            {
                EmailId = "21197091@studentmail.ul.ie",
                IsActive = true,
                //UserType = UserType.Customer,
                Password = "21197090"
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
            await vaccineHubDbContext.ApiUsers.AddAsync(existingApiUser, CancellationToken.None);
            await vaccineHubDbContext.SaveChangesAsync();
            _serviceProviderMock.Setup(s => s.GetService(typeof(IVaccineHubDbContext))).Returns(vaccineHubDbContext);

            //Act
            await _sut.UpdateApiUserAsync(updatedApiUser, CancellationToken.None);
            var dbApiUser =
                await vaccineHubDbContext.ApiUsers.FindAsync(new object[] {updatedApiUser.EmailId}, CancellationToken.None);
            Assert.NotNull(dbApiUser);
            
            Assert.AreEqual(dbApiUser.EmailId, updatedApiUser.EmailId);
            Assert.AreEqual(dbApiUser.Password,updatedApiUser.Password);
            Assert.AreEqual(dbApiUser.IsActive,updatedApiUser.IsActive);
            
        }
         [Test]
        public async Task GetApiUserByEmailId()
        {
            //Arrange
            var apiUser1 = new VaccineHub.Persistence.Entities.ApiUser
            {
                EmailId = "21197091@studentmail.ul.ie",
                IsActive = true,
                //UserType = UserType.Customer,
                Password = "21197091"
            };
            
            var apiUser2 = new VaccineHub.Persistence.Entities.ApiUser
            {
                EmailId = "21017301@studentmail.ul.ie",
                IsActive = true,
                //UserType = UserType.Customer,
                Password = "21017301"
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
            await vaccineHubDbContext.ApiUsers.AddAsync(apiUser1, CancellationToken.None);
            await vaccineHubDbContext.SaveChangesAsync();
            
            await vaccineHubDbContext.ApiUsers.AddAsync(apiUser2, CancellationToken.None);
            await vaccineHubDbContext.SaveChangesAsync();
            
            _serviceProviderMock.Setup(s => s.GetService(typeof(IVaccineHubDbContext))).Returns(vaccineHubDbContext);

            //Act
            var result = await _sut.GetUserAsync("21197091@studentmail.ul.ie", CancellationToken.None);
            
            Assert.NotNull(result);
            
            Assert.AreEqual(result.EmailId, apiUser1.EmailId);
            Assert.AreEqual(result.Password,apiUser1.Password);
            Assert.AreEqual(result.IsActive,apiUser1.IsActive);
            
        }
        
         [Test]
        public async Task GetAllApiUsers()
        {
            //Arrange
            var apiUser1 = new VaccineHub.Persistence.Entities.ApiUser
            {
                EmailId = "21197091@studentmail.ul.ie",
                IsActive = true,
                //UserType = UserType.Customer,
                Password = "21197091"
            };
            
            var apiUser2 = new VaccineHub.Persistence.Entities.ApiUser
            {
                EmailId = "21017301@studentmail.ul.ie",
                IsActive = true,
                //UserType = UserType.Customer,
                Password = "21017301"
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
            await vaccineHubDbContext.ApiUsers.AddAsync(apiUser1, CancellationToken.None);
            await vaccineHubDbContext.SaveChangesAsync();
            
            await vaccineHubDbContext.ApiUsers.AddAsync(apiUser2, CancellationToken.None);
            await vaccineHubDbContext.SaveChangesAsync();
            
            _serviceProviderMock.Setup(s => s.GetService(typeof(IVaccineHubDbContext))).Returns(vaccineHubDbContext);

            //Act
            var result = await _sut.GetUsersAsync( CancellationToken.None);
            
            Assert.NotNull(result);
            
            //Assert.AreEqual(result.EmailId, apiUser1.EmailId);
            //Assert.AreEqual(result.Password,apiUser1.Password);
            //Assert.AreEqual(result.IsActive,apiUser1.IsActive);
            
        }
        
    }
}