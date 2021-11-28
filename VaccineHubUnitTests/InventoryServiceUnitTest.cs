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
using VaccineHub.Service.Inventory;
using Inventory = VaccineHub.Service.Models.Inventory;

namespace VaccineHubUnitTests
{
    public class InventoryServiceTest
    {
        private readonly IInventoryService _sut;
        private readonly Mock<IServiceProvider> _serviceProviderMock = new();

        public InventoryServiceTest()
        {
            _sut = new InventoryService(_serviceProviderMock.Object);
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

            //Arrange
            var inventory = new Inventory
            {
                ProductId = "limerick",
                CenterId = "pfizer",
                Stock = 50
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
            await vaccineHubDbContext.Products.AddAsync(product);
            await vaccineHubDbContext.Centers.AddAsync(center);
            await vaccineHubDbContext.SaveChangesAsync(CancellationToken.None);

            _serviceProviderMock.Setup(s => s.GetService(typeof(IVaccineHubDbContext))).Returns(vaccineHubDbContext);

            //Act
            var result = await _sut.AddInventoryAsync(inventory, CancellationToken.None);

            Assert.IsTrue(result);

            var dbInventory = await vaccineHubDbContext.Inventories.FirstOrDefaultAsync(CancellationToken.None);
            Assert.AreEqual(dbInventory.Stock, inventory.Stock);
            Assert.AreEqual(dbInventory.Product.Id, inventory.ProductId);
            Assert.AreEqual(dbInventory.Center.Id, inventory.CenterId);
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
                Currency = Currency.Eur
            };

            var updatedInventory = new Inventory
            {
                ProductId = "pfizer",
                CenterId = "limerick",
                Stock = 100
            };

            var existingInventory = new VaccineHub.Persistence.Entities.Inventory
            {
                Product = product,
                Center = center,
                Stock = 50
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
            await vaccineHubDbContext.Products.AddAsync(product);
            await vaccineHubDbContext.Centers.AddAsync(center);
            await vaccineHubDbContext.Inventories.AddAsync(existingInventory, CancellationToken.None);
            await vaccineHubDbContext.SaveChangesAsync();

            _serviceProviderMock.Setup(s => s.GetService(typeof(IVaccineHubDbContext))).Returns(vaccineHubDbContext);

            //Act
            var result = await _sut.UpdateInventoryAsync(updatedInventory, CancellationToken.None);

            Assert.IsTrue(result);

            var dbUpdatedInventory = await vaccineHubDbContext.Inventories.FirstOrDefaultAsync();
            Assert.AreEqual(dbUpdatedInventory.Stock, updatedInventory.Stock);
            Assert.AreEqual(dbUpdatedInventory.Product.Id, updatedInventory.ProductId);
            Assert.AreEqual(dbUpdatedInventory.Center.Id, updatedInventory.CenterId);
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

            var existingInventory = new VaccineHub.Persistence.Entities.Inventory
            {
                Product = product,
                Center = center,
                Stock = 50
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
            await vaccineHubDbContext.Products.AddAsync(product, CancellationToken.None);
            await vaccineHubDbContext.Centers.AddAsync(center, CancellationToken.None);
            await vaccineHubDbContext.Inventories.AddAsync(existingInventory, CancellationToken.None);
            await vaccineHubDbContext.SaveChangesAsync();

            _serviceProviderMock.Setup(s => s.GetService(typeof(IVaccineHubDbContext))).Returns(vaccineHubDbContext);

            //Act
            var result = await _sut.GetAllInventoriesAsync( CancellationToken.None);
            Assert.NotNull(result);

            var inventory = result.FirstOrDefault();
            
            Assert.NotNull(inventory);
            Assert.AreEqual(inventory.Stock, existingInventory.Stock);
            Assert.AreEqual(inventory.ProductId, existingInventory.Product.Id);
            Assert.AreEqual(inventory.CenterId, existingInventory.Center.Id);
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

            var existingInventory = new VaccineHub.Persistence.Entities.Inventory
            {
                Product = product,
                Center = center,
                Stock = 50
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
            await vaccineHubDbContext.Products.AddAsync(product);
            await vaccineHubDbContext.Centers.AddAsync(center);
            await vaccineHubDbContext.Inventories.AddAsync(existingInventory, CancellationToken.None);
            await vaccineHubDbContext.SaveChangesAsync();

            _serviceProviderMock.Setup(s => s.GetService(typeof(IVaccineHubDbContext))).Returns(vaccineHubDbContext);

            //Act
            var result = await _sut.GetAllInventoriesByProductIdAsync( "pfizer", CancellationToken.None);

            Assert.NotNull(result);
        }
    }
}    
