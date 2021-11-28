using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using VaccineHub.Persistence;
using VaccineHub.Service.Abstractions;
using VaccineHub.Service.Center;
using VaccineHub.Service.Models;

namespace VaccineHubUnitTests
{
    public class CenterServiceTest
    {
        private readonly ICenterService _sut;
        private readonly Mock<IServiceProvider> _serviceProviderMock = new();

        public CenterServiceTest()
        {
            _sut = new CenterService(_serviceProviderMock.Object);
        }

        private static VaccineHubDbContext CreateDbContext()
        {
            var builder = new DbContextOptionsBuilder<VaccineHubDbContext>();

            builder.UseInMemoryDatabase(Guid.NewGuid().ToString());

            return new VaccineHubDbContext(builder.Options);
        }

        [Test]
        public async Task AddCenter_WhenCenterWithSameIdDoesNotExists()
        {
            //Arrange
            var center = new Center
            {
                Id = "limerick",
                Name = "Hospital Community Center",
                Description = "Center",
                Telephone = "0123456789",
                EirCode = "V35 X2P1"
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
            var result = await _sut.AddCenterAsync(center, CancellationToken.None);
                
            Assert.IsTrue(result);

            var dbCenter =
                await vaccineHubDbContext.Centers.FindAsync(new object[] {"limerick"}, CancellationToken.None);
            Assert.NotNull(dbCenter);

            Assert.AreEqual(dbCenter.Name, center.Name);
            Assert.AreEqual(dbCenter.Telephone,center.Telephone);
            Assert.AreEqual(dbCenter.EirCode,center.EirCode);
            Assert.AreEqual(dbCenter.Description,center.Description);
        }

        [Test]
        public async Task UpdateCenter_WhenCenterIdExists()
        {
            //Arrange
            var updatedCenter = new Center
            {
                Id = "limerick",
                Name = "Hospital Community Center",
                Description = "Center1",
                Telephone = "0123456789",
                EirCode = "V35 X2P1"
            };
            
            var existingCenter = new VaccineHub.Persistence.Entities.Center
            {
                Id = "limerick",
                Name = "Hospital Community Center",
                Description = "Center",
                Telephone = "0123456789",
                EirCode = "V35 X2P1"
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
            await vaccineHubDbContext.Centers.AddAsync(existingCenter, CancellationToken.None);
            await vaccineHubDbContext.SaveChangesAsync();

            _serviceProviderMock.Setup(s => s.GetService(typeof(IVaccineHubDbContext))).Returns(vaccineHubDbContext);

            //Act
            var result = await _sut.UpdateCenterAsync(updatedCenter, CancellationToken.None);
                
            Assert.IsTrue(result);

            var dbCenter =
                await vaccineHubDbContext.Centers.FindAsync(new object[] {"limerick"}, CancellationToken.None);
            Assert.NotNull(dbCenter);

            Assert.AreEqual(dbCenter.Name, updatedCenter.Name);
            Assert.AreEqual(dbCenter.Telephone,updatedCenter.Telephone);
            Assert.AreEqual(dbCenter.EirCode,updatedCenter.EirCode);
            Assert.AreEqual(dbCenter.Description,updatedCenter.Description);
        }
        
        
        [Test]
        public async Task GetCenter_ById_WhenCenterExists()
        {
            //Arrange
            var center = new VaccineHub.Persistence.Entities.Center
            {
                Id = "limerick",
                Name = "Hospital Community Center",
                Description = "Center",
                Telephone = "0123456789",
                EirCode = "V35 X2P1"
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
            await vaccineHubDbContext.Centers.AddAsync(center, CancellationToken.None);
            await vaccineHubDbContext.SaveChangesAsync();
            
            _serviceProviderMock.Setup(s => s.GetService(typeof(IVaccineHubDbContext))).Returns(vaccineHubDbContext);
            
            
            //Act
            var result = await _sut.GetCenterAsync("limerick", CancellationToken.None);
            
            Assert.NotNull(result);

            Assert.AreEqual(result.Name, center.Name);
            Assert.AreEqual(result.Telephone,center.Telephone);
            Assert.AreEqual(result.EirCode,center.EirCode);
            Assert.AreEqual(result.Description,center.Description);
        }

        [Test]
        public async Task GetAllCenters()
        {
            //Arrange
            var center1 = new VaccineHub.Persistence.Entities.Center
            {
                Id = "limerick",
                Name = "Hospital Community Center",
                Description = "Center",
                Telephone = "0123456789",
                EirCode = "V35 X2P1"
            };

            var center2 = new VaccineHub.Persistence.Entities.Center
            {
                Id = "limerick01",
                Name = "Private Hospital Center",
                Description = "Center",
                Telephone = "0123456564",
                EirCode = "V35 X2Z1"
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
            await vaccineHubDbContext.Centers.AddAsync(center1, CancellationToken.None);
            await vaccineHubDbContext.Centers.AddAsync(center2, CancellationToken.None);
            await vaccineHubDbContext.SaveChangesAsync();

            _serviceProviderMock.Setup(s => s.GetService(typeof(IVaccineHubDbContext))).Returns(vaccineHubDbContext);


            //Act
            var result = await _sut.GetAllCentersAsync(CancellationToken.None);

            Assert.NotNull(result);

            Assert.AreEqual(result.Count, 2);
        }
    }
}