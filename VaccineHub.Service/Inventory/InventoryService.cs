using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VaccineHub.Persistence;
using VaccineHub.Service.Abstractions;
using VaccineHub.Service.Concurrency;

namespace VaccineHub.Service.Inventory
{
    public class InventoryService : IInventoryService
    {
        private static readonly IMapper Mapper = CreateMapConfiguration()
            .CreateMapper();

        private readonly IServiceProvider _serviceProvider;

        public InventoryService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<bool> AddInventoryAsync(Models.Inventory inventory, CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<IVaccineHubDbContext>();

            var dbInventory = await dbContext.Inventories.Include(i => i.Product)
                .Include(i => i.Center).FirstOrDefaultAsync(
                i => i.Center.Id == inventory.CenterId && i.Product.Id == inventory.ProductId, cancellationToken);

            if (dbInventory != null)
            {
                throw new InvalidOperationException("Inventory already exists");
            }

            dbInventory = Mapper.Map<Persistence.Entities.Inventory>(inventory);

            await dbContext.Inventories.AddAsync(dbInventory, cancellationToken);

            return true;
        }

        public async Task<bool> UpdateInventoryAsync(Models.Inventory inventory, CancellationToken cancellationToken)
        {
            using(await AsyncSimulatedLock.LockAsync(inventory.ProductId + inventory.CenterId, cancellationToken))
            {
                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<IVaccineHubDbContext>();

                var dbInventory = await dbContext.Inventories.Include(i => i.Product)
                    .Include(i => i.Center).FirstOrDefaultAsync(
                    a => a.Center.Id == inventory.CenterId && a.Product.Id == inventory.ProductId, cancellationToken);

                if (dbInventory == null)
                {
                    throw new InvalidOperationException("Inventory does not exits");
                }

                dbInventory.Stock = inventory.Stock;

                await dbContext.SaveChangesAsync(cancellationToken);

                return true;
            }
        }

        public Task<List<Models.Inventory>> GetAllInventoriesAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<IVaccineHubDbContext>();

            return dbContext.Inventories.Include(i => i.Product).Include(i => i.Center)
                .Select(i => Mapper.Map<Models.Inventory>(i)).ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Models.Inventory>> GetAllInventoriesByCenterIdAsync(string centerId,
            CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<IVaccineHubDbContext>();

            var inventories = await dbContext.Inventories.Include(i => i.Product)
                .Include(i => i.Center).Where(i => i.Center.Id == centerId)
                .ToListAsync(cancellationToken);

            return inventories.Select(inventory => Mapper.Map<Models.Inventory>(inventory));
        }

        public async Task<IEnumerable<Models.Inventory>> GetAllInventoriesByProductIdAsync(string productId, CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<IVaccineHubDbContext>();

            var inventories = await dbContext.Inventories.Include(i => i.Product)
                .Include(i => i.Center).Where(i => i.Product.Id == productId)
                .ToListAsync(cancellationToken);

            return inventories.Select(inventory => Mapper.Map<Models.Inventory>(inventory));
        }

        private static MapperConfiguration CreateMapConfiguration()
        {
            return new MapperConfiguration(expression =>
            {
                expression.CreateMap<Persistence.Entities.Inventory, Models.Inventory>()
                    .ForMember(dest => dest.CenterId,
                        config => config.MapFrom(src => src.Center.Id))
                    .ForMember(dest => dest.ProductId,
                        config => config.MapFrom(src => src.Product.Id))
                    .ReverseMap();
            });
        }
    }
}