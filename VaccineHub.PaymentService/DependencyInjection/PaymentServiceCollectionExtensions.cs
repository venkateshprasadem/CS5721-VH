using System;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace VaccineHub.PaymentService.DependencyInjection
{
    public static class RouteServiceCollectionExtensions
    {
        [UsedImplicitly]
        public static IServiceCollection AddPaymentService(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddSingleton<IPaymentService, PaymentService>();
            
            return services;
        }
    }
}