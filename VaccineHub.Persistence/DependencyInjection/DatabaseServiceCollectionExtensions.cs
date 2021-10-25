using System;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VaccineHub.Persistence.Construction;
using VaccineHub.Shared.Tenancy;

namespace VaccineHub.Persistence.DependencyInjection
{
    public static class DatabaseServiceCollectionExtensions
    {
        public static IServiceCollection AddDatabase([NotNull] this IServiceCollection services, [NotNull] IConfigurationRoot configuration)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddOptions();
            
            services.Configure<DatabaseSettings>(configuration.GetSection("Database"));

            services.AddDbContext<IVaccineHubDbContext, VaccineHubDbContext>((sp, builder) =>
            {
                var databaseSettingsMonitor = sp.GetRequiredService<IOptionsMonitor<DatabaseSettings>>();
                var databaseSettings = databaseSettingsMonitor.CurrentValue;

                builder.UseDatabase(databaseSettings);

                if (!databaseSettings.IsQueryLoggingEnabled)
                {
                    return;
                }

                var loggerFactory = sp.GetRequiredService<ILoggerFactory>();

                builder.UseLoggerFactory(loggerFactory);
            });

            return services;
        }
    }
}