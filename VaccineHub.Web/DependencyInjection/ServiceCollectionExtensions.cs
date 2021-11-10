using System;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using VaccineHub.Service.RegisterDependencies;
using VaccineHub.Web.Authentication;
using VaccineHub.Web.Services.Users;

namespace VaccineHub.Web.DependencyInjection
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterServices([NotNull] this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddServicesImplementations();
            services.AddSingleton<IApiUsersDataProvider, ApiUsersDataProvider>();
            services.AddSingleton<AuthenticatorTemplate, BasicAuthenticator>();

            return services;
        }
    }
}