using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using VaccineHub.Shared.Tenancy;

namespace VaccineHub.Persistence.Construction
{
    public static class DbContextOptionsBuilderExtensions
    {
        public static DbContextOptionsBuilder UseDatabase(
            [NotNull] this DbContextOptionsBuilder contextOptionsBuilder,
            [NotNull] DatabaseSettings databaseSettings,
            TimeSpan? commandTimeout = default)
        {
            if (contextOptionsBuilder == null)
            {
                throw new ArgumentNullException(nameof(contextOptionsBuilder));
            }

            if (databaseSettings == null)
            {
                throw new ArgumentNullException(nameof(databaseSettings));
            }

            if (databaseSettings.Server == "memory")
            {
                return contextOptionsBuilder.UseInMemoryDatabase("vaccine_hub");
            }

            var connectionStringBuilder = new NpgsqlConnectionStringBuilder
            {
                Host = databaseSettings.Server,
                Port = databaseSettings.Port,
                Database = databaseSettings.Name,
                Username = databaseSettings.Username,
                Password = databaseSettings.Password
            };

            if (databaseSettings.Pooling != ConnectionPooling.Default)
            {
                connectionStringBuilder.Pooling = databaseSettings.Pooling == ConnectionPooling.Always;
            }

            return contextOptionsBuilder.UseNpgsql(
                connectionStringBuilder.ConnectionString,
                opts =>
                {
                    if (commandTimeout != null)
                    {
                        opts.CommandTimeout((int) commandTimeout.Value.TotalSeconds);
                    }
                });
        }
    }
}