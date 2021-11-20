using DinkToPdf;
using DinkToPdf.Contracts;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using VaccineHub.Service.Abstractions;
using VaccineHub.Service.Center;
using VaccineHub.Service.GenerateCertificates;
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
            
            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

            services.AddSingleton<IPDFService, PDFService>();
            return services;
        }
    }
}