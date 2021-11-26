using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using VaccineHub.Persistence;
//using VaccineHub.Persistence.Entities;
//using VaccineHub.Service.Abstractions;
using VaccineHub.Web.Services.Users.Models;
using VaccineHub.Web.Services.Users;

namespace ApiUserService.moq
{
    public class ApiUserserviceTest
    {
        private readonly IApiUsersDataProvider _sut;
        private readonly Mock<IServiceProvider> _serviceproviderMock = new Mock<IServiceProvider>();


        public ApiUserserviceTest()
        {
            _sut = new ApiUsersDataProvider(_serviceproviderMock.Object);
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
             var apiuser = new ApiUser
            {
                EmailId = "21004528@studentmail.ul.ie",
                IsActive = true,
                UserType = UserType.Customer,
                Password = "21004528"
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
            await _sut.AddUserAsync(apiuser, CancellationToken.None);
            //Assert
            var dbApiUser =
                await vaccineHubDbContext.ApiUsers.FindAsync(new object[] {apiuser.EmailId}, CancellationToken.None);
            Assert.NotNull(dbApiUser);
            
        }
        
        [Test]
        public async Task UpdateApiUser_WhenUserExists()
        {
            //Arrange
            var updatedapiuser = new ApiUser
            {
                EmailId = "21197091@studentmail.ul.ie",
                IsActive = true,
                UserType = UserType.Customer,
                Password = "21197091"
            };
            
            var existingapiuser = new VaccineHub.Persistence.Entities.ApiUser
            {
                EmailId = "21197091@studentmail.ul.ie",
                IsActive = true,
                //UserType = UserType.Customer,
                Password = "21197090"
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
            await vaccineHubDbContext.ApiUsers.AddAsync(existingapiuser, CancellationToken.None);
            await vaccineHubDbContext.SaveChangesAsync();
            _serviceproviderMock.Setup(s => s.GetService(typeof(IVaccineHubDbContext))).Returns(vaccineHubDbContext);

            //Act
            await _sut.UpdateApiUserAsync(updatedapiuser, CancellationToken.None);
            var dbApiUser =
                await vaccineHubDbContext.ApiUsers.FindAsync(new object[] {updatedapiuser.EmailId}, CancellationToken.None);
            Assert.NotNull(dbApiUser);
            
            Assert.AreEqual(dbApiUser.EmailId, updatedapiuser.EmailId);
            Assert.AreEqual(dbApiUser.Password,updatedapiuser.Password);
            Assert.AreEqual(dbApiUser.IsActive,updatedapiuser.IsActive);
            
        }
         [Test]
        public async Task GetApiUserByEmailId()
        {
            //Arrange
            var apiuser1 = new VaccineHub.Persistence.Entities.ApiUser
            {
                EmailId = "21197091@studentmail.ul.ie",
                IsActive = true,
                //UserType = UserType.Customer,
                Password = "21197091"
            };
            
            var apiuser2 = new VaccineHub.Persistence.Entities.ApiUser
            {
                EmailId = "21017301@studentmail.ul.ie",
                IsActive = true,
                //UserType = UserType.Customer,
                Password = "21017301"
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
            await vaccineHubDbContext.ApiUsers.AddAsync(apiuser1, CancellationToken.None);
            await vaccineHubDbContext.SaveChangesAsync();
            
            await vaccineHubDbContext.ApiUsers.AddAsync(apiuser2, CancellationToken.None);
            await vaccineHubDbContext.SaveChangesAsync();
            
            _serviceproviderMock.Setup(s => s.GetService(typeof(IVaccineHubDbContext))).Returns(vaccineHubDbContext);

            //Act
            var result = await _sut.GetUserAsync("21197091@studentmail.ul.ie", CancellationToken.None);
            
            Assert.NotNull(result);
            
            Assert.AreEqual(result.EmailId, apiuser1.EmailId);
            Assert.AreEqual(result.Password,apiuser1.Password);
            Assert.AreEqual(result.IsActive,apiuser1.IsActive);
            
        }
        
         [Test]
        public async Task GetAllApiUsers()
        {
            //Arrange
            var apiuser1 = new VaccineHub.Persistence.Entities.ApiUser
            {
                EmailId = "21197091@studentmail.ul.ie",
                IsActive = true,
                //UserType = UserType.Customer,
                Password = "21197091"
            };
            
            var apiuser2 = new VaccineHub.Persistence.Entities.ApiUser
            {
                EmailId = "21017301@studentmail.ul.ie",
                IsActive = true,
                //UserType = UserType.Customer,
                Password = "21017301"
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
            await vaccineHubDbContext.ApiUsers.AddAsync(apiuser1, CancellationToken.None);
            await vaccineHubDbContext.SaveChangesAsync();
            
            await vaccineHubDbContext.ApiUsers.AddAsync(apiuser2, CancellationToken.None);
            await vaccineHubDbContext.SaveChangesAsync();
            
            _serviceproviderMock.Setup(s => s.GetService(typeof(IVaccineHubDbContext))).Returns(vaccineHubDbContext);

            //Act
            var result = await _sut.GetUsersAsync( CancellationToken.None);
            
            Assert.NotNull(result);
            
            //Assert.AreEqual(result.EmailId, apiuser1.EmailId);
            //Assert.AreEqual(result.Password,apiuser1.Password);
            //Assert.AreEqual(result.IsActive,apiuser1.IsActive);
            
        }
        
    }
}