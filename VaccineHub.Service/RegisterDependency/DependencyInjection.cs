using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using VaccineHub.Service.Abstractions;
using VaccineHub.Service.Center;
using VaccineHub.Service.Product;

namespace VaccineHub.Service.RegisterDependency
{
    public static class DependencyInjection
    {
        [UsedImplicitly]
        public static IServiceCollection AddServices(
            [NotNull] this IServiceCollection services)
        {
            services.AddSingleton<IProductService, ProductService>();
            services.AddSingleton<ICenterService, CenterService>();
            return services;
        }
    }
}