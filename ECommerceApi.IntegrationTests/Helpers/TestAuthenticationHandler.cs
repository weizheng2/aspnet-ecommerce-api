using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ECommerceApi.IntegrationTests
{
    public interface ITestUserProvider
    {
        ClaimsPrincipal CurrentUser { get; set; }
    }

    public class TestUserProvider : ITestUserProvider
    {
        // Default user
        public ClaimsPrincipal CurrentUser { get; set; } = new ClaimsPrincipal(
            new ClaimsIdentity(
            [
                new Claim(Constants.ClaimTypeEmail, "test@example.com"),
            ], "Test") 
        );
    }

    public class TestAuthenticationSchemeOptions : AuthenticationSchemeOptions 
    { 
    }

    public class TestAuthenticationHandler : AuthenticationHandler<TestAuthenticationSchemeOptions>
    {
        private readonly ITestUserProvider _userProvider;

        public TestAuthenticationHandler(IOptionsMonitor<TestAuthenticationSchemeOptions> options,
        ILoggerFactory logger, UrlEncoder encoder, ITestUserProvider userProvider) : base(options, logger, encoder)
        {
             _userProvider = userProvider;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var principal = _userProvider.CurrentUser;

            if (principal == null)
                return Task.FromResult(AuthenticateResult.Fail("No test user set"));

            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}