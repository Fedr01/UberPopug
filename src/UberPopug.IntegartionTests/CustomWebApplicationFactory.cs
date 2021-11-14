using System;
using System.Data.Common;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using UberPopug.AccountingService;
using UberPopug.AuthService;
using UberPopug.TaskTrackerService;

namespace UberPopug.IntegartionTests
{
    public class CustomWebApplicationFactory<TStartup, TDbContext> : WebApplicationFactory<TStartup>
        where TStartup : class
        where TDbContext : DbContext
    {
        public readonly HttpClient Client;
        
        public readonly AuthDbContext AuthDbContext;
        public readonly TaskTrackerDbContext TrackerDbContext;
        public readonly AccountingDbContext AccountingDbContext;

        public CustomWebApplicationFactory()
        {
            Client = WithWebHostBuilder(builder =>
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

            Client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Test");
            
            var authDbBuilder = new DbContextOptionsBuilder<AuthDbContext>();
            authDbBuilder.UseSqlServer(
                "Data Source=host.docker.internal;Initial Catalog=auth;User Id=sa;Password=Argentum4!;");
            AuthDbContext = new AuthDbContext(authDbBuilder.Options);

            var trackerBuilder = new DbContextOptionsBuilder<TaskTrackerDbContext>();
            trackerBuilder.UseSqlServer(
                "Data Source=host.docker.internal;Initial Catalog=tracker;User Id=sa;Password=Argentum4!;");
            TrackerDbContext = new TaskTrackerDbContext(trackerBuilder.Options);

            var accOptionsBuilder = new DbContextOptionsBuilder<AccountingDbContext>();
            accOptionsBuilder.UseSqlServer(
                "Data Source=host.docker.internal;Initial Catalog=accounting;User Id=sa;Password=Argentum4!;");
            AccountingDbContext = new AccountingDbContext(accOptionsBuilder.Options);
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var sp = services.BuildServiceProvider();

                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<TDbContext>();

                 //   db.Database.EnsureDeleted();
                }
            });
        }
    }
}