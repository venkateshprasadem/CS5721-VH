using DinkToPdf;
using DinkToPdf.Contracts;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using VaccineHub.Service.Abstractions;
using VaccineHub.Service.Booking;
using VaccineHub.Service.Center;
using VaccineHub.Service.GenerateCertificates;
using VaccineHub.Service.Inventory;
using VaccineHub.Service.Product;
using VaccineHub.ThirdPartyService.DependencyInjection;

namespace VaccineHub.Service.RegisterDependency
{
    public static class ServiceCollectionExtensions
    {
        [UsedImplicitly]
        public static IServiceCollection AddServicesImplementations(
            [NotNull] this IServiceCollection services)
        {
            
            services.AddSingleton<IProductService, ProductService>();
            services.AddSingleton<ICenterService, CenterService>();
            services.AddSingleton<IInventoryService, InventoryService>();
            services.AddSingleton<IBookingService, BookingService>();
            services.AddThirdPartyService();
            
            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

            services.AddSingleton<IPDFService, PDFService>();
            return services;
        }
    }
}