using System;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace VaccineHub.ThirdPartyService.DependencyInjection
{
    public static class ThirdPartyServiceCollectionExtensions
    {
        [UsedImplicitly]
        public static IServiceCollection AddThirdPartyService(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddSingleton<IThirdPartyService, ThirdPartyService>();
            
            return services;
        }
    }
}