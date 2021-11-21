using System;
using System.IdentityModel.Tokens.Jwt;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
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
using UberPopug.Common;
using UberPopug.Common.Constants;
using UberPopug.SchemaRegistry.Schemas.Tasks;
using UberPopug.SchemaRegistry.Schemas.Tasks.Cud;
using UberPopug.SchemaRegistry.Schemas.Users;
using UberPopug.TaskTrackerService.Tasks;
using UberPopug.TaskTrackerService.Users;
using AutoOffsetReset = KafkaFlow.AutoOffsetReset;

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
                options.UseSqlServer(Configuration.GetConnectionString("TaskTracker")));


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


            services.AddKafka(
                kafka => kafka
                    .UseConsoleLog()
                    .AddCluster(
                        cluster => cluster
                            .WithBrokers(new[] { Configuration["Kafka:ClientConfigs:BootstrapServers"] })
                            .WithSchemaRegistry(config => config.Url = "localhost:8081")
                            .AddProducer<TaskCreatedCudEvent>(
                                producer => producer
                                    .DefaultTopic(KafkaTopics.TasksStream)
                                    .AddMiddlewares(middlewares => middlewares
                                        .Add<KafkaLoggingMiddleware>()
                                        .AddSchemaRegistryJsonSerializer<TaskCreatedCudEvent>(
                                            new JsonSerializerConfig
                                            {
                                                SubjectNameStrategy = SubjectNameStrategy.Record,
                                                AutoRegisterSchemas = false
                                            }))
                            )
                            .AddProducer<TaskCreatedEvent>(
                                producer => producer
                                    .DefaultTopic(KafkaTopics.Tasks)
                                    .AddMiddlewares(middlewares => middlewares
                                        .Add<KafkaLoggingMiddleware>()
                                        .AddTypedSchemaRegistryJsonSerializer(
                                            new JsonSerializerConfig
                                            {
                                                SubjectNameStrategy = SubjectNameStrategy.Record,
                                                AutoRegisterSchemas = false
                                            }))
                            )
                            .AddProducer<TaskCompletedEvent>(
                                producer => producer
                                    .DefaultTopic(KafkaTopics.Tasks)
                                    .AddMiddlewares(middlewares => middlewares
                                        .Add<KafkaLoggingMiddleware>()
                                        .AddTypedSchemaRegistryJsonSerializer(
                                            new JsonSerializerConfig
                                            {
                                                SubjectNameStrategy = SubjectNameStrategy.Record,
                                                AutoRegisterSchemas = false
                                            }))
                            )
                            .AddProducer<TaskAssignedEvent>(
                                producer => producer
                                    .DefaultTopic(KafkaTopics.Tasks)
                                    .AddMiddlewares(middlewares => middlewares
                                        .Add<KafkaLoggingMiddleware>()
                                        .AddTypedSchemaRegistryJsonSerializer(
                                            new JsonSerializerConfig
                                            {
                                                SubjectNameStrategy = SubjectNameStrategy.Record,
                                                AutoRegisterSchemas = false
                                            }))
                            )
                            .AddConsumer(
                                consumer => consumer
                                    .Topic(KafkaTopics.UsersStream)
                                    .WithGroupId("tracker-group-id")
                                    .WithBufferSize(1)
                                    .WithWorkersCount(1)
                                    .WithAutoOffsetReset(AutoOffsetReset.Latest)
                                    .AddMiddlewares(
                                        middlewares => middlewares
                                            .AddSchemaRegistryJsonSerializer<UserCreatedEvent>()
                                            .Add<KafkaLoggingMiddleware>()
                                            .AddTypedHandlers(
                                                handlers => handlers
                                                    .AddHandler<UserCreatedEventHandler>()
                                                    .WithHandlerLifetime(InstanceLifetime.Scoped)
                                            )
                                    )
                            )));

            services.AddScoped<ITasksManager, TasksManager>();
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, TaskTrackerDbContext dataContext,
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
                    pattern: "{controller=Tasks}/{action=Index}/{id?}");
            });
        }
    }
}