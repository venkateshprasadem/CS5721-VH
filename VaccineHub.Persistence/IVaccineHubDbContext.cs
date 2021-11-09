using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VaccineHub.Persistence.Entities;

namespace VaccineHub.Persistence
{
    public interface IVaccineHubDbContext
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken());

        bool IsDatabaseInMemory();

        DbSet<ApiUser> ApiUsers { get; }

        DbSet<Product> Products { get; }

        DbSet<Center> Centers { get; }

        DbSet<TEntity> Set<TEntity>() where TEntity : class;
    }
}