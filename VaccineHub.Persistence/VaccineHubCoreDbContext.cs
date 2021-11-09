﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VaccineHub.Persistence.Entities;
using VaccineHub.Persistence.Types;

namespace VaccineHub.Persistence
{
    internal class VaccineHubDbContext : DbContext, IVaccineHubDbContext
    {
        public VaccineHubDbContext(DbContextOptions<VaccineHubDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<ApiUser>()
                .Property(i => i.UserType)
                .HasConversion(
                    i => i.ToString(),
                    i => (UserType) Enum.Parse(typeof(UserType), i));
        }

        public bool IsDatabaseInMemory()
        {
            return Database.IsInMemory();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            if (Database.IsInMemory())
            {
                // workaround for https://github.com/aspnet/EntityFrameworkCore/issues/13368
                cancellationToken.ThrowIfCancellationRequested();
            }

            var addedAuditedEntities = ChangeTracker.Entries<IAuditEntity>()
                .Where(p => p.State == EntityState.Added)
                .Select(p => p.Entity);

            var now = DateTime.UtcNow;

            foreach (var added in addedAuditedEntities)
            {
                added.CreatedAt = now;
                added.UpdatedAt = now;
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        public DbSet<ApiUser> ApiUsers => Set<ApiUser>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Center> Centers => Set<Center>();
    }
}