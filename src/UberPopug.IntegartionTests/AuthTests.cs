using System.Net.Http;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shouldly;
using UberPopug.AuthService;
using Xunit;
using Startup = UberPopug.AuthService.Startup;

namespace UberPopug.IntegartionTests
{
    public class AuthServiceTests : IClassFixture<CustomWebApplicationFactory<Startup, AuthDbContext>>
    {
        private readonly CustomWebApplicationFactory<Startup, AuthDbContext> _authWebApp;

        public AuthServiceTests(CustomWebApplicationFactory<Startup, AuthDbContext> authWebApp)
        {
            _authWebApp = authWebApp;
        }

        [Fact]
        public async Task Should_Produce_And_Consume_New_Users()
        {
            await _authWebApp.AuthDbContext.Database.ExecuteSqlRawAsync("DELETE FROM Users");
            await _authWebApp.TrackerDbContext.Database.ExecuteSqlRawAsync("DELETE FROM Tasks");
            await _authWebApp.TrackerDbContext.Database.ExecuteSqlRawAsync("DELETE FROM Users");
            await _authWebApp.AccountingDbContext.Database.ExecuteSqlRawAsync("DELETE FROM Tasks");
            await _authWebApp.AccountingDbContext.Database.ExecuteSqlRawAsync("DELETE FROM Users");

            for (int i = 0; i < 10; i++)
            {
                await _authWebApp.Client.PostAsync("/Account/CreateRandom", new StringContent(""));
            }

            await Task.Delay(2000);
            
            var authUsers = await _authWebApp.AuthDbContext.Users.ToListAsync();
            var trackerUsers = await _authWebApp.TrackerDbContext.Users.ToListAsync();
            var accountingUsers = await _authWebApp.AccountingDbContext.Users.ToListAsync();

            
            authUsers.Count.ShouldBe(10);
            trackerUsers.Count.ShouldBe(authUsers.Count);
            accountingUsers.Count.ShouldBe(authUsers.Count);
        }
    }

    public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new[] { new Claim(ClaimTypes.Name, "Test user") };
            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "Test");

            var result = AuthenticateResult.Success(ticket);

            return Task.FromResult(result);
        }
    }
}