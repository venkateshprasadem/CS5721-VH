using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using VaccineHub.Persistence;
using VaccineHub.Persistence.Entities;
using VaccineHub.Service.Abstractions;
using VaccineHub.Service.Inventory;
using Inventory = VaccineHub.Service.Models.Inventory;

namespace Inventoryservice.moq
{
    public class InventoryserviceTest
    {
        private readonly IInventoryService _sut;
        private readonly Mock<IServiceProvider> _serviceproviderMock = new Mock<IServiceProvider>();


        public InventoryserviceTest()
        {
            _sut = new InventoryService(_serviceproviderMock.Object);
        }

        private static VaccineHubDbContext CreateDbContext()
        {
            var builder = new DbContextOptionsBuilder<VaccineHubDbContext>();

            builder.UseInMemoryDatabase(Guid.NewGuid().ToString());

            return new VaccineHubDbContext(builder.Options);
        }

        [Test]
        public async Task AddInventory_WhenInventoryWithSameIdDoesNotExists()
        {
            //Arrange
            var inventory = new Inventory
            {
                ProductId = "limerick",
                CenterId = "pfizer",
                Stock = 50
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
            var result = await _sut.AddInventoryAsync(inventory, CancellationToken.None);

            Assert.IsTrue(result);

            //var dbProduct =
            //  await vaccineHubDbContext.Products.FindAsync(new object[] {"pfizer"}, CancellationToken.None);
            //Assert.NotNull(dbProduct);

            //Assert.AreEqual(dbProduct.Name, product.Name);
            //Assert.AreEqual(dbProduct.Cost, product.Cost);
            //Assert.AreEqual(dbProduct.Doses, product.Doses);
            //Assert.AreEqual(dbProduct.MinIntervalInDays, product.MinIntervalInDays);
            //Assert.AreEqual(dbProduct.MaxIntervalInDays, product.MaxIntervalInDays);
        }

        [Test]
        public async Task UpdateInventory_WhenInventoryExists()
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

            var product = new Product
            {
                Id = "pfizer",
                Name = "pfizer",
                Cost = 10,
                Doses = 2,
                MinIntervalInDays = 17,
                MaxIntervalInDays = 27,
                //Currency = Currency.Eur
            };

            var updatedinventory = new Inventory
            {
                ProductId = "pfizer",
                CenterId = "limerick",
                Stock = 100
            };

            var existinginventory = new VaccineHub.Persistence.Entities.Inventory
            {
                Product = product,
                Center = center,
                Stock = 50
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
            await vaccineHubDbContext.Inventories.AddAsync(existinginventory, CancellationToken.None);
            await vaccineHubDbContext.SaveChangesAsync();
            _serviceproviderMock.Setup(s => s.GetService(typeof(IVaccineHubDbContext))).Returns(vaccineHubDbContext);

            //Act
            var result = await _sut.UpdateInventoryAsync(updatedinventory, CancellationToken.None);

            Assert.IsTrue(result);
        }
        
        
        [Test]
        public async Task GetAllInventories()
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

            var product = new Product
            {
                Id = "pfizer",
                Name = "pfizer",
                Cost = 10,
                Doses = 2,
                MinIntervalInDays = 17,
                MaxIntervalInDays = 27,
                //Currency = Currency.Eur
            };

            var existinginventory = new VaccineHub.Persistence.Entities.Inventory
            {
                Product = product,
                Center = center,
                Stock = 50
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
            await vaccineHubDbContext.Inventories.AddAsync(existinginventory, CancellationToken.None);
            await vaccineHubDbContext.SaveChangesAsync();
            _serviceproviderMock.Setup(s => s.GetService(typeof(IVaccineHubDbContext))).Returns(vaccineHubDbContext);

            //Act
            var result = await _sut.GetAllInventoriesAsync( CancellationToken.None);

            Assert.NotNull(result);
        }
        
        [Test]
        public async Task GetAllInventoriesByProductIdTest()
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

            var product = new Product
            {
                Id = "pfizer",
                Name = "pfizer",
                Cost = 10,
                Doses = 2,
                MinIntervalInDays = 17,
                MaxIntervalInDays = 27,
                //Currency = Currency.Eur
            };

            var existinginventory = new VaccineHub.Persistence.Entities.Inventory
            {
                Product = product,
                Center = center,
                Stock = 50
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
            await vaccineHubDbContext.Inventories.AddAsync(existinginventory, CancellationToken.None);
            await vaccineHubDbContext.SaveChangesAsync();
            _serviceproviderMock.Setup(s => s.GetService(typeof(IVaccineHubDbContext))).Returns(vaccineHubDbContext);

            //Act
            var result = await _sut.GetAllInventoriesByProductIdAsync( "pfizer", CancellationToken.None);

            Assert.NotNull(result);
        }
    }
}    
