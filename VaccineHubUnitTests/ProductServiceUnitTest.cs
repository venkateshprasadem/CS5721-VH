using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using VaccineHub.Persistence;
using VaccineHub.Service.Abstractions;
using VaccineHub.Service.Models;
using VaccineHub.Service.Product;

namespace VaccineHubUnitTests
{
    public class ProductServiceTest
    {
        private readonly IProductService _sut;
        private readonly Mock<IServiceProvider> _serviceProviderMock = new();


        public ProductServiceTest()
        {
            _sut = new ProductService(_serviceProviderMock.Object);
        }

        private static VaccineHubDbContext CreateDbContext()
        {
            var builder = new DbContextOptionsBuilder<VaccineHubDbContext>();

            builder.UseInMemoryDatabase(Guid.NewGuid().ToString());

            return new VaccineHubDbContext(builder.Options);
        }

        [Test]
        public async Task AddProduct_WhenProductWithSameIdDoesNotExists()
        {
            //Arrange
            var product = new Product
            {
                Id = "pfizer",
                Name = "pfizer",
                Cost = 10,
                Doses = 2,
                MinIntervalInDays = 17,
                MaxIntervalInDays = 27,
                Currency = Currency.Eur,
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
            var result = await _sut.AddProductAsync(product, CancellationToken.None);

            Assert.IsTrue(result);

            var dbProduct =
                await vaccineHubDbContext.Products.FindAsync(new object[] {"pfizer"}, CancellationToken.None);
            Assert.NotNull(dbProduct);

            Assert.AreEqual(dbProduct.Name, product.Name);
            Assert.AreEqual(dbProduct.Cost, product.Cost);
            Assert.AreEqual(dbProduct.Doses, product.Doses);
            Assert.AreEqual(dbProduct.MinIntervalInDays, product.MinIntervalInDays);
            Assert.AreEqual(dbProduct.MaxIntervalInDays, product.MaxIntervalInDays);
        }
        
        [Test]
        public async Task UpdateProduct_WhenProductExists()
        {
            //Arrange
            var updatedProduct = new Product
            {
                Id = "pfizer",
                Name = "pfizer",
                Cost = 20,
                Doses = 2,
                MinIntervalInDays = 17,
                MaxIntervalInDays = 27,
                //Currency = Currency.Eur
            };
            
            var existingProduct = new VaccineHub.Persistence.Entities.Product
            {
                Id = "pfizer",
                Name = "pfizer",
                Cost = 10,
                Doses = 2,
                MinIntervalInDays = 17,
                MaxIntervalInDays = 27,
                //Currency = Currency.Eur
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
            await vaccineHubDbContext.Products.AddAsync(existingProduct, CancellationToken.None);
            await vaccineHubDbContext.SaveChangesAsync();
            
            _serviceProviderMock.Setup(s => s.GetService(typeof(IVaccineHubDbContext))).Returns(vaccineHubDbContext);

            //Act
            var result = await _sut.UpdateProductAsync(updatedProduct, CancellationToken.None);

            Assert.IsTrue(result);

            var dbProduct =
                await vaccineHubDbContext.Products.FindAsync(new object[] {"pfizer"}, CancellationToken.None);
            Assert.NotNull(dbProduct);

            Assert.AreEqual(dbProduct.Name, updatedProduct.Name);
            Assert.AreEqual(dbProduct.Cost, updatedProduct.Cost);
            Assert.AreEqual(dbProduct.Doses, updatedProduct.Doses);
            Assert.AreEqual(dbProduct.MinIntervalInDays, updatedProduct.MinIntervalInDays);
            Assert.AreEqual(dbProduct.MaxIntervalInDays, updatedProduct.MaxIntervalInDays);
        }
        [Test]
        public async Task GetProductById_WhenProductExists()
        {
            //Arrange
            var product1 = new VaccineHub.Persistence.Entities.Product
            {
                Id = "pfizer",
                Name = "pfizer",
                Cost = 20,
                Doses = 2,
                MinIntervalInDays = 17,
                MaxIntervalInDays = 27,
                //Currency = Currency.Eur
            };
            
            var product2 = new VaccineHub.Persistence.Entities.Product
            {
                Id = "AstraZeneca",
                Name = "AstraZeneca",
                Cost = 10,
                Doses = 2,
                MinIntervalInDays = 14,
                MaxIntervalInDays = 28,
                //Currency = Currency.Eur
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
            await vaccineHubDbContext.Products.AddAsync(product1, CancellationToken.None);
            await vaccineHubDbContext.SaveChangesAsync();
            await vaccineHubDbContext.Products.AddAsync(product2, CancellationToken.None);
            await vaccineHubDbContext.SaveChangesAsync();
            
            _serviceProviderMock.Setup(s => s.GetService(typeof(IVaccineHubDbContext))).Returns(vaccineHubDbContext);

            //Act
            var result = await _sut.GetProductAsync("pfizer", CancellationToken.None);

            Assert.NotNull(result);

            var dbProduct =
                await vaccineHubDbContext.Products.FindAsync(new object[] {"pfizer"}, CancellationToken.None);
            Assert.NotNull(dbProduct);

            Assert.AreEqual(dbProduct.Name, product1.Name);
            Assert.AreEqual(dbProduct.Cost, product1.Cost);
            Assert.AreEqual(dbProduct.Doses, product1.Doses);
            Assert.AreEqual(dbProduct.MinIntervalInDays, product1.MinIntervalInDays);
            Assert.AreEqual(dbProduct.MaxIntervalInDays, product1.MaxIntervalInDays);
        }
        
         [Test]
        public async Task GetAllProducts()
        {
            //Arrange
            var product1 = new VaccineHub.Persistence.Entities.Product
            {
                Id = "pfizer",
                Name = "pfizer",
                Cost = 20,
                Doses = 2,
                MinIntervalInDays = 17,
                MaxIntervalInDays = 27,
                //Currency = Currency.Eur
            };
            
            var product2 = new VaccineHub.Persistence.Entities.Product
            {
                Id = "AstraZeneca",
                Name = "AstraZeneca",
                Cost = 10,
                Doses = 2,
                MinIntervalInDays = 14,
                MaxIntervalInDays = 28,
                //Currency = Currency.Eur
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
            await vaccineHubDbContext.Products.AddAsync(product1, CancellationToken.None);
            await vaccineHubDbContext.SaveChangesAsync();
            await vaccineHubDbContext.Products.AddAsync(product2, CancellationToken.None);
            await vaccineHubDbContext.SaveChangesAsync();
            
            _serviceProviderMock.Setup(s => s.GetService(typeof(IVaccineHubDbContext))).Returns(vaccineHubDbContext);

            //Act
            var result = await _sut.GetAllProductsAsync( CancellationToken.None);

            Assert.NotNull(result);
            
        }
    }

}    