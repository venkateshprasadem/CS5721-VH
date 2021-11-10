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

namespace VaccineHub.Service.Center
{
    internal class CenterService : ICenterService
    {
        private static readonly IMapper Mapper = CreateMapConfiguration()
            .CreateMapper();

        private readonly IServiceProvider _serviceProvider;

        public CenterService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<Models.Center> GetCenterAsync(string centerId, CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<IVaccineHubDbContext>();

            var center = await dbContext.Centers.SingleAsync(p => p.Id == centerId, cancellationToken);

            return Mapper.Map<Models.Center>(center);
        }

        public async Task<bool> AddCenterAsync(Models.Center center, CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<IVaccineHubDbContext>();

            var existingCenter = await dbContext.Centers.SingleOrDefaultAsync(c => c.Id == center.Id, cancellationToken);

            if (existingCenter != null)
            {
                throw new InvalidOperationException("Center already exists");
            }

            var dbCenter = Mapper.Map<Persistence.Entities.Center>(center);

            await dbContext.Centers.AddAsync(dbCenter, cancellationToken);

            await dbContext.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<bool> UpdateCenterAsync(Models.Center center, CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<IVaccineHubDbContext>();

            var existingCenter = await dbContext.Centers.SingleOrDefaultAsync(c => c.Id == center.Id, cancellationToken);

            if (existingCenter == null)
            {
                throw new InvalidOperationException("centerId does not exist");
            }

            existingCenter.Name = center.Name;
            existingCenter.Telephone = center.Telephone;
            existingCenter.EirCode = center.Telephone;
            existingCenter.Description = center.Description;

            await dbContext.SaveChangesAsync(cancellationToken);

            return true;
        }

        public Task<List<Models.Center>> GetAllCentersAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<IVaccineHubDbContext>();

            return dbContext.Centers.Select(center => Mapper.Map<Models.Center>(center)).ToListAsync(cancellationToken);
        }

        private static MapperConfiguration CreateMapConfiguration()
        {
            return new MapperConfiguration(expression =>
            {
                expression.CreateMap<Persistence.Entities.Center, Models.Center>()
                    .ReverseMap();
            });
        }
    }
}