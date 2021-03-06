using System;
using System.IdentityModel.Tokens.Jwt;
using KafkaFlow;
using KafkaFlow.Serializer;
using KafkaFlow.TypedHandler;
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
using UberPopug.AccountingService.Tasks.Assigned;
using UberPopug.AccountingService.Tasks.Completed;
using UberPopug.AccountingService.Tasks.Created;
using UberPopug.AccountingService.Users;
using UberPopug.Common;
using UberPopug.Common.Constants;
using AutoOffsetReset = KafkaFlow.AutoOffsetReset;

namespace UberPopug.AccountingService
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

            services.AddDbContext<AccountingDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("Accounting")));


            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                })
                .AddCookie(options =>
                {
                    options.Cookie.SameSite = SameSiteMode.Unspecified;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                })
                .AddOpenIdConnect(options =>
                {
                    options.CorrelationCookie = new CookieBuilder
                    {
                        HttpOnly = false,
                        SameSite = SameSiteMode.Unspecified,
                        SecurePolicy = CookieSecurePolicy.Always,
                        Expiration = TimeSpan.FromMinutes(10)
                    };

                    options.RequireHttpsMetadata = false;

                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

                    options.Authority = "https://localhost:5000";
                    options.ClientId = "accounting";
                    options.ClientSecret = "accounting-pwd";

                    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.ResponseType = OpenIdConnectResponseType.CodeIdToken;
                    options.SaveTokens = true;

                    options.Scope.Add("accounting");
                    options.Scope.Add("roles");
                    options.ClaimActions.MapJsonKey("role", "role", "role");
                    options.TokenValidationParameters.RoleClaimType = "role";
                });

            services.AddKafka(
                kafka => kafka
                    .UseConsoleLog()
                    .AddCluster(
                        cluster => cluster
                            .WithBrokers(new[] { Configuration["Kafka:ClientConfigs:BootstrapServers"] })
                            //   .WithSchemaRegistry(config => config.Url = "localhost:8081")
                            .AddConsumer(
                                consumer => consumer
                                    .Topic(KafkaTopics.UsersStream)
                                    .WithGroupId("accounting-group-id")
                                    .WithBufferSize(1)
                                    .WithWorkersCount(1)
                                    .WithAutoOffsetReset(AutoOffsetReset.Latest)
                                    .AddMiddlewares(
                                        middlewares => middlewares
                                            .AddSerializer<NewtonsoftJsonSerializer, KafkaMessageTypeResolver>()
                                            .AddTypedHandlers(
                                                handlers => handlers
                                                    .AddHandler<UserCreatedEventHandler>()
                                                    .WithHandlerLifetime(InstanceLifetime.Scoped)
                                            )
                                    )
                            )
                            .AddConsumer(
                                consumer => consumer
                                    .Topic(KafkaTopics.TasksStream)
                                    .WithGroupId("accounting-group-id")
                                    .WithBufferSize(1)
                                    .WithWorkersCount(1)
                                    .WithAutoOffsetReset(AutoOffsetReset.Latest)
                                    .AddMiddlewares(
                                        middlewares => middlewares
                                            .AddSerializer<NewtonsoftJsonSerializer, KafkaMessageTypeResolver>()
                                            .AddTypedHandlers(
                                                handlers => handlers
                                                    .AddHandler<TaskCreatedCudEventV2Handler>()
                                                    .WithHandlerLifetime(InstanceLifetime.Scoped)
                                            )
                                    )
                            )
                            .AddConsumer(
                                consumer => consumer
                                    .Topic(KafkaTopics.Tasks)
                                    .WithGroupId("accounting-group-id")
                                    .WithBufferSize(1)
                                    .WithWorkersCount(1)
                                    .WithAutoOffsetReset(AutoOffsetReset.Latest)
                                    .AddMiddlewares(
                                        middlewares => middlewares
                                            .AddSerializer<NewtonsoftJsonSerializer, KafkaMessageTypeResolver>()
                                            .AddTypedHandlers(
                                                handlers => handlers
                                                    .AddHandler<TaskCreatedEventV2Handler>()
                                                    .WithHandlerLifetime(InstanceLifetime.Scoped)
                                            )
                                            .AddTypedHandlers(
                                                handlers => handlers
                                                    .AddHandler<TaskCompletedEventHandler>()
                                                    .WithHandlerLifetime(InstanceLifetime.Scoped)
                                            )
                                            .AddTypedHandlers(
                                                handlers => handlers
                                                    .AddHandler<TaskAssignedEventHandler>()
                                                    .WithHandlerLifetime(InstanceLifetime.Scoped)
                                            )
                                    )
                            )
                    ));

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, AccountingDbContext dataContext,
            IHostApplicationLifetime lifetime)
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

            var kafkaBus = app.ApplicationServices.CreateKafkaBus();
            lifetime.ApplicationStarted.Register(() => kafkaBus.StartAsync(lifetime.ApplicationStopped));

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Accounting}/{action=Index}/{id?}");
            });
        }
    }
}