using DoWhatta.Platform.Builder;
using DoWhatta.Platform.Builder.Output;
using DoWhatta.Platform.Builder.Output.Writers;
using DoWhatta.Platform.Core.Common;
using DoWhatta.Platform.Core.Settings;
using DoWhatta.Platform.Data.Extensions;
using DoWhatta.Platform.Infrastructure.HttpClients;
using DoWhatta.Platform.Infrastructure.Messaging.ServiceBus;
using Microsoft.EntityFrameworkCore;
using NewDoor.API.Data;
using NewDoor.API.Features.MetaModel.Mapper;
using NewDoor.API.Features.User.Mapper;
using NewDoor.API.Services;
using NewDoor.API.Settings;
using NewDoor.API.Configuration;
using NewDoor.API.Hubs;
using Serilog;
using ApplicationSettings = NewDoor.API.Settings.ApplicationSettings;

namespace NewDoor.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var builder = WebApplication.CreateBuilder(args);

                Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(builder.Configuration)
                    .Enrich.FromLogContext()
                    .WriteTo.Console()
                    .WriteTo.File(
                        path: "logs/log-.txt",
                        rollingInterval: RollingInterval.Day,
                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                    .CreateLogger();

                builder.Host.UseSerilog();

                builder.Services.AddCors(options =>
                {
                    options.AddPolicy("AllowBlazorClient",
                        policy =>
                        {
                            policy
                                 .WithOrigins(
                                     "https://localhost:7092",
                                     "http://localhost:7092",
                                     "https://dowhatta.azurewebsites.net"
                                 )
                                 .AllowAnyHeader()
                                 .AllowAnyMethod()
                                 .AllowCredentials();
                        });
                });

                builder.Services.AddSignalR()
                    .AddJsonProtocol(options =>
                    {
                        options.PayloadSerializerOptions.PropertyNamingPolicy = null; // Use original property names
                        options.PayloadSerializerOptions.PropertyNameCaseInsensitive = true;
                    });

                // Register all Kafka consumers using keyed service pattern
                builder.Services.AddKafkaConsumers(builder.Configuration);

                builder.WebHost.AddApplicationConfiguration<ApplicationSettings>();

                builder.Services
                       .AddDoWhattaPlatformDatabase<DoWhattaDBContext>(builder.Configuration)
                       .AddDoWhattaProductDatabase<DoWhattaProductDBContext>(builder.Configuration);
                builder.Services.AddExternalServiceClient<InternalServiceSettings>(builder.Configuration);
                builder.Services.AddAutoMapper(typeof(UserMapper));
                builder.Services.AddAutoMapper(typeof(MetaModelMapper));
                builder.Services.AddAutoMapper(typeof(EntityPropertyMetaModelMapper));

                builder.Services.AddScoped<EntityGenerator>();
                builder.Services.AddScoped<CodeOutputOrchestrator>();

                builder.Services.AddPlatformServices(builder.Configuration);

                var app = builder.Build();

                using (var scope = app.Services.CreateScope())
                {
                    var platformDb = scope.ServiceProvider.GetRequiredService<DoWhattaDBContext>();
                    if (!platformDb.Database.IsSqlite())
                    {
                        platformDb.Database.Migrate();
                    }

                    var productDb = scope.ServiceProvider.GetRequiredService<DoWhattaProductDBContext>();
                    if (!productDb.Database.IsSqlite())
                    {
                        productDb.Database.Migrate();
                    }
                }

                app.UseSerilogRequestLogging();
                app.UseCors("AllowBlazorClient");
                app.MapHub<NotificationHub>("/notificationHub");
                app.ConfigureApplication();

                Log.Information("NewDoor API starting...");
                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "NewDoor API failed to start");
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
