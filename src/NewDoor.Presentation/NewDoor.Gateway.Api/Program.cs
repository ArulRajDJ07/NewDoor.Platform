using DoWhatta.Platform.Builder;
using DoWhatta.Platform.Builder.Output;
using DoWhatta.Platform.Builder.Output.Writers;
using DoWhatta.Platform.Core.Common;
using DoWhatta.Platform.Core.Settings;
using DoWhatta.Platform.Data.Extensions;
using DoWhatta.Platform.Infrastructure.HttpClients;
using DoWhatta.Platform.Infrastructure.Messaging.ServiceBus;
using NewDoor.Gateway.Api.Settings;
using NewDoor.Gateway.Api.Services;
using NewDoor.EventBus.Extensions;

namespace NewDoor.Gateway.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add CORS policy
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowBlazorClient",
                    policy =>
                    {
                        policy
                             .WithOrigins(
                                 "https://localhost:7092",
                                 "http://localhost:7092",
                                 "https://dowhatta.azurewebsites.net",
                                 "https://newdoor-simulator.azurewebsites.net"
                             )
                             .AllowAnyHeader()
                             .AllowAnyMethod()
                             .AllowCredentials();
                    });
            });

            builder.WebHost.AddApplicationConfiguration<ApplicationSettings>();            
            builder.Services.AddPlatformServices(builder.Configuration);

            // Add Kafka Producer
            builder.Services.AddKafkaProducer(config =>
            {
                config.BootstrapServers = builder.Configuration["Kafka:BootstrapServers"] ?? "pkc-619z3.us-east1.gcp.confluent.cloud:9092";
                config.Username = builder.Configuration["Kafka:Username"] ?? "";
                config.Password = builder.Configuration["Kafka:Password"] ?? "";
                config.MessageTimeoutMs = builder.Configuration.GetValue<int>("Kafka:MessageTimeoutMs", 300000);
                config.RequestTimeoutMs = builder.Configuration.GetValue<int>("Kafka:RequestTimeoutMs", 300000);
            });

            // Add Device Enrichment Service
            builder.Services.AddHttpClient();
            builder.Services.AddSingleton<IDeviceEnrichmentService, DeviceEnrichmentService>();

            var app = builder.Build();

            app.UseCors("AllowBlazorClient");
            app.ConfigureApplication();

            app.Run();
        }
    }
}

