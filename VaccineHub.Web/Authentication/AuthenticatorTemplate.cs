using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace VaccineHub.Web.Authentication
{
    public abstract class AuthenticatorTemplate
    {
        public abstract string Scheme { get; }

        public async Task<AuthenticateResult> AuthenticateAsync(string header, CancellationToken token)
        {
            if (!header.StartsWith(Scheme, StringComparison.OrdinalIgnoreCase))
            {
                return AuthenticateResult.Fail("Authentication failed.");
            }
            
            var encodedCredentials = header.Substring(Scheme.Length).Trim();

            if (string.IsNullOrEmpty(encodedCredentials))
            {
                return AuthenticateResult.Fail("No credentials provided");
            }

            return await AuthenticateCredentialsAsync(encodedCredentials, token);
        }

        protected abstract Task<AuthenticateResult> AuthenticateCredentialsAsync(string encodedCredentials, CancellationToken token);
    }
}