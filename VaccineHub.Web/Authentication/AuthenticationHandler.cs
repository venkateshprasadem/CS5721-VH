using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace VaccineHub.Web.Authentication
{
    [UsedImplicitly]
    internal sealed class AuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly ILogger _logger;
        private readonly IReadOnlyDictionary<string, AuthenticatorTemplate> _authenticators;

        public AuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory loggerFactory,
            UrlEncoder encoder,
            ISystemClock clock,
            IEnumerable<AuthenticatorTemplate> authenticators) 
            : base(options, NullLoggerFactory.Instance, encoder, clock)
        {
            _logger = loggerFactory.CreateLogger(GetType().FullName);

            _authenticators = authenticators.ToDictionary(
                x => x.Scheme, 
                x => x, 
                StringComparer.OrdinalIgnoreCase);
        }
        
        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            _logger.LogInformation(
                "Issuing authentication challenge for route {0} {1}",
                Request.Method,
                Request.Path);

            Response.Headers["WWW-Authenticate"] = "Basic realm=\"Api\"";
       
            return base.HandleChallengeAsync(properties);
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var authorizationHeader = Request.Headers["Authorization"].FirstOrDefault();
            
            return string.IsNullOrEmpty(authorizationHeader) 
                ? Task.FromResult(AuthenticateResult.NoResult()) 
                : HandleAuthenticationAsync(authorizationHeader);
        }

        private Task<AuthenticateResult> HandleAuthenticationAsync(string authorizationHeader)
        {
            var scheme = authorizationHeader.Split(' ')[0];
            
            return !_authenticators.TryGetValue(scheme, out var authenticator) 
                ? Task.FromResult(AuthenticateResult.Fail($"Unknown scheme: {scheme}")) 
                : authenticator.AuthenticateAsync(authorizationHeader, Context.RequestAborted);
        }
    }
}