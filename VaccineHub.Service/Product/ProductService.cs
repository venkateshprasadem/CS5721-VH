using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VaccineHub.Persistence;
using VaccineHub.Persistence.Types;
using VaccineHub.Service.Abstractions;

namespace VaccineHub.Service.Product
{
    internal class ProductService : IProductService
    {
        private static readonly IMapper Mapper = CreateMapConfiguration()
            .CreateMapper();

        private readonly IServiceProvider _serviceProvider;

        public ProductService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<Models.Product> GetProductAsync(string productId, CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<IVaccineHubDbContext>();

            var product = await dbContext.Products.FirstAsync(a => a.Id == productId, cancellationToken);

            return Mapper.Map<Models.Product>(product);
        }

        public async Task<bool> AddProductAsync(Models.Product product, CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<IVaccineHubDbContext>();

            var existingProduct = await dbContext.Products
                .FirstOrDefaultAsync(p => p.Id == product.Id, cancellationToken: cancellationToken);

            if (existingProduct != null)
            {
                throw new InvalidOperationException("product already exists");
            }

            var dbProduct = Mapper.Map<Persistence.Entities.Product>(product);

            await dbContext.Products.AddAsync(dbProduct, cancellationToken);

            await dbContext.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<bool> UpdateProductAsync(Models.Product product, CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<IVaccineHubDbContext>();

            var existingProduct = await dbContext.Products
                .FindAsync(new object[] {product.Id}, cancellationToken);
            
            if (existingProduct == null)
            {
                throw new InvalidOperationException("product not found");
            }

            existingProduct.Name =  product.Name;
            existingProduct.Cost = product.Cost;
            existingProduct.Currency = Mapper.Map<Currency>(product.Currency);
            existingProduct.Doses = product.Doses;
            existingProduct.MinIntervalInDays = product.MinIntervalInDays;
            existingProduct.MaxIntervalInDays = product.MaxIntervalInDays;
            existingProduct.UpdatedAt = DateTime.Now;

            await dbContext.SaveChangesAsync(cancellationToken);

            return true;
        }

        public Task<List<Models.Product>> GetAllProductsAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<IVaccineHubDbContext>();

            return dbContext.Products.Select(product => Mapper.Map<Models.Product>(product)).ToListAsync(cancellationToken);
        }

        private static MapperConfiguration CreateMapConfiguration()
        {
            return new MapperConfiguration(expression =>
            {
                expression.CreateMap<Persistence.Entities.Product, Models.Product>().
                    ReverseMap();
            });
        }
    }
}