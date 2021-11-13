using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shouldly;
using UberPopug.AccountingService;
using UberPopug.AuthService;
using UberPopug.TaskTrackerService;
using Xunit;
using Startup = UberPopug.AuthService.Startup;

namespace UberPopug.IntegartionTests
{
    public class AuthServiceTests : IClassFixture<CustomWebApplicationFactory<Startup, AuthDbContext>>
    {
        private readonly CustomWebApplicationFactory<Startup, AuthDbContext> _authWebApp;

        private readonly AuthDbContext _authDbContext;
        private readonly TaskTrackerDbContext _trackerDbContext;
        private readonly AccountingDbContext _accountingDbContext;

        public AuthServiceTests(CustomWebApplicationFactory<Startup, AuthDbContext> authWebApp)
        {
            _authWebApp = authWebApp;

            var authDbBuilder = new DbContextOptionsBuilder<AuthDbContext>();
            authDbBuilder.UseSqlServer("Data Source=host.docker.internal;Initial Catalog=auth;User Id=sa;Password=Argentum4!;");
            _authDbContext = new AuthDbContext(authDbBuilder.Options);

            var trackerBuilder = new DbContextOptionsBuilder<TaskTrackerDbContext>();
            trackerBuilder.UseSqlServer("Data Source=host.docker.internal;Initial Catalog=tracker;User Id=sa;Password=Argentum4!;");
            _trackerDbContext = new TaskTrackerDbContext(trackerBuilder.Options);

            var accOptionsBuilder = new DbContextOptionsBuilder<AccountingDbContext>();
            accOptionsBuilder.UseSqlServer("Data Source=host.docker.internal;Initial Catalog=accounting;User Id=sa;Password=Argentum4!;");
            _accountingDbContext = new AccountingDbContext(accOptionsBuilder.Options);
        }

        [Fact]
        public async Task Should_Produce_And_Consume_New_Users()
        {
            var client = _authWebApp.WithWebHostBuilder(builder =>
                {
                    builder.ConfigureTestServices(services =>
                    {
                        services.AddAuthentication("Test")
                            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                                "Test", options => { });
                    });
                })
                .CreateClient(new WebApplicationFactoryClientOptions
                {
                    AllowAutoRedirect = false,
                });

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Test");

            await _authDbContext.Database.ExecuteSqlRawAsync("DELETE FROM Users");
            await _trackerDbContext.Database.ExecuteSqlRawAsync("DELETE FROM Tasks");
            await _trackerDbContext.Database.ExecuteSqlRawAsync("DELETE FROM Users");
            await _accountingDbContext.Database.ExecuteSqlRawAsync("DELETE FROM Tasks");
            await _accountingDbContext.Database.ExecuteSqlRawAsync("DELETE FROM Users");

            for (int i = 0; i < 10; i++)
            {
                await client.PostAsync("/Account/CreateRandom", new StringContent(""));
            }


            var authUsers = await _authDbContext.Users.ToListAsync();
            var trackerUsers = await _trackerDbContext.Users.ToListAsync();
            var accountingUsers = await _accountingDbContext.Users.ToListAsync();

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