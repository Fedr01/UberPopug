using System;
using System.IdentityModel.Tokens.Jwt;
using Confluent.Kafka;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using UberPopug.Common.Consumer;
using UberPopug.Common.Interfaces;
using UberPopug.Common.Producer;
using UberPopug.TaskTrackerService.Users;

namespace UberPopug.TaskTrackerService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddDbContext<TaskTrackerDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));


            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                })
                .AddCookie(options =>
                {
                    options.Cookie.SameSite = SameSiteMode.Unspecified;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.AccessDeniedPath = "";
                    options.LoginPath = "";
                })
                .AddOpenIdConnect(options =>
                {
                    options.CorrelationCookie = new Microsoft.AspNetCore.Http.CookieBuilder
                    {
                        HttpOnly = false,
                        SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Unspecified,
                        SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.Always,
                        Expiration = TimeSpan.FromMinutes(10)
                    };

                    options.RequireHttpsMetadata = false;

                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

                    options.Authority = "https://localhost:5000";
                    options.ClientId = "tracker";
                    options.ClientSecret = "tracker-pwd";

                    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.ResponseType = OpenIdConnectResponseType.CodeIdToken;
                    options.SaveTokens = true;

                    options.Scope.Add("tracker");
                    options.Scope.Add("roles");
                    options.ClaimActions.MapJsonKey("role", "role", "role");
                    options.TokenValidationParameters.RoleClaimType = "role";
                });

            var clientConfig = new ClientConfig()
            {
                BootstrapServers = Configuration["Kafka:ClientConfigs:BootstrapServers"]
            };

            var producerConfig = new ProducerConfig(clientConfig);
            var consumerConfig = new ConsumerConfig(clientConfig)
            {
                GroupId = "SourceApp",
                EnableAutoCommit = true,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                StatisticsIntervalMs = 5000,
                SessionTimeoutMs = 6000
            };

            services.AddSingleton(producerConfig);
            services.AddSingleton(consumerConfig);

            services.AddSingleton(typeof(IKafkaProducer<,>), typeof(KafkaProducer<,>));

            services.AddScoped<IKafkaHandler<string, CreateUserCommand>, RegisterUserHandler>();
            services.AddSingleton(typeof(IKafkaConsumer<,>), typeof(KafkaConsumer<,>));
            services.AddHostedService<RegisterUserConsumer>();

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, TaskTrackerDbContext dataContext)
        {
            app.UseCookiePolicy(new CookiePolicyOptions
            {
                MinimumSameSitePolicy = SameSiteMode.Unspecified,
                Secure = CookieSecurePolicy.Always
            });

            dataContext.Database.Migrate();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseHsts();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Tasks}/{action=Index}/{id?}");
            });
        }
    }
}