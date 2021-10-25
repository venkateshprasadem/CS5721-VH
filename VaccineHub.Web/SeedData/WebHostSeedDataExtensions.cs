using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Nito.AsyncEx;
using VaccineHub.Persistence;

namespace VaccineHub.Web.SeedData
{
    public static class WebHostSeedDataExtensions
    {
        public static IWebHost SeedData(this IWebHost host)
        {
            using var scope = host.Services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<IVaccineHubDbContext>();

            if (context.IsDatabaseInMemory())
            {
                AsyncContext.Run(() => new DbContextSeedData(context).SeedDataAsync());
            }

            return host;
        }
    }
}