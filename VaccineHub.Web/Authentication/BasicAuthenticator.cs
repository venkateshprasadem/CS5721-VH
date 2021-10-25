using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using VaccineHub.Web.Services.Users;
using VaccineHub.Web.Services.Users.Models;

namespace VaccineHub.Web.Authentication
{
    internal sealed class BasicAuthenticator : AuthenticatorTemplate
    {
        private readonly IApiUsersDataProvider _userProvider;
        public BasicAuthenticator(IApiUsersDataProvider userProvider)
        {
            _userProvider = userProvider;
        }

        public override string Scheme => "Basic";

        protected override async Task<AuthenticateResult> AuthenticateCredentialsAsync(string encodedCredentials, CancellationToken token)
        {
            if (!TryDecodeBase64(encodedCredentials, out var decodedCredentials))
            {
                return AuthenticateResult.Fail("Invalid credentials, failed to convert from Base64");
            }

            var clientCredentials = decodedCredentials.Split(':', 2);

            if (clientCredentials.Length != 2)
            {
                return AuthenticateResult.Fail("Invalid credentials");
            }

            var emailId = clientCredentials[0];
            var password = clientCredentials[1];

            var (isValid, user) = await TryGetUser(emailId, password, token);

            return isValid
                ? AuthenticateResult.Success(CreateTicket(user))
                : AuthenticateResult.Fail("Authentication Failed, credentials don't match");
        }

        private AuthenticationTicket CreateTicket(ApiUser user)
        {
            var identities = new ClaimsIdentity(CreateClaims(user), Scheme);

            return new AuthenticationTicket(new ClaimsPrincipal(identities), Scheme);
        }

        private static IEnumerable<Claim> CreateClaims(ApiUser user)
        {
            return new List<Claim>
            {
                new (ClaimTypes.Name, user.EmailId),
                new ("EmailId", user.EmailId),
                new (ClaimTypes.Role, user.UserType.ToString())
            };
        }

        private static bool TryDecodeBase64(string encodedString, out string decodedString)
        {
            try
            {
                decodedString = Encoding.UTF8.GetString(Convert.FromBase64String(encodedString));

                return true;
            }
            catch (Exception)
            {
                decodedString = string.Empty;

                return false;
            }
        }
        private async Task<(bool, ApiUser)> TryGetUser(string emailId, string password, CancellationToken token)
        {
            var user = await _userProvider.GetUserAsync(emailId, token);

            return (user != null && user.Password == password && user.IsActive, user);
        }
    }
}