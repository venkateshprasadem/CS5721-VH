using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using VaccineHub.Service.Abstractions;
using VaccineHub.Service.Booking;
using VaccineHub.Service.Center;
using VaccineHub.Service.Inventory;
using VaccineHub.Service.Product;

namespace VaccineHub.Service.RegisterDependencies
{
    public static class DependencyInjection
    {
        [UsedImplicitly]
        public static IServiceCollection AddServicesImplementations(
            [NotNull] this IServiceCollection services)
        {
            services.AddSingleton<IProductService, ProductService>();
            services.AddSingleton<ICenterService, CenterService>();
            services.AddSingleton<IInventoryService, InventoryService>();
            services.AddSingleton<IBookingService, BookingService>();
            return services;
        }
    }
}