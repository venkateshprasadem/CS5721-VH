using System;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VaccineHub.Web.Authentication;
using VaccineHub.Web.Services.Users;

namespace VaccineHub.Web.DependencyInjection
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterServices([NotNull] this IServiceCollection services,
            [NotNull] IConfigurationRoot configuration)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddSingleton<IApiUsersDataProvider, ApiUsersDataProvider>();
            services.AddSingleton<AuthenticatorTemplate, BasicAuthenticator>();

            return services;
        }
    }
}