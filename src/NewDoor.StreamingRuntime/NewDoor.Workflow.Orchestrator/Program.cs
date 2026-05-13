using DoWhatta.Platform.Builder;
using DoWhatta.Platform.Builder.Output;
using DoWhatta.Platform.Builder.Output.Writers;
using DoWhatta.Platform.Core.Common;
using DoWhatta.Platform.Core.Settings;
using DoWhatta.Platform.Data.Extensions;
using DoWhatta.Platform.Infrastructure.HttpClients;
using DoWhatta.Platform.Infrastructure.Messaging.ServiceBus;
using NewDoor.Workflow.Orchestrator.Settings;
using NewDoor.EventBus.Consumers;
using NewDoor.EventBus.Producers;
using NewDoor.Workflow.Orchestrator.Models;
using NewDoor.Workflow.Orchestrator.Services;
using NewDoor.Workflow.Orchestrator.Handlers;
using NewDoor.Workflow.Orchestrator.BackgroundServices;
using Serilog;
using ApplicationSettings = NewDoor.Workflow.Orchestrator.Settings.ApplicationSettings;

namespace NewDoor.Workflow.Orchestrator
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

                builder.Services.AddSingleton<KafkaConsumerConfig>(sp =>
                {
                    var config = sp.GetRequiredService<IConfiguration>();
                    return new KafkaConsumerConfig
                    {
                        BootstrapServers = config["Kafka:BootstrapServers"] ?? "localhost:9092",
                        Username = config["Kafka:Username"] ?? "",
                        Password = config["Kafka:Password"] ?? "",
                        GroupId = config["Kafka:GroupId"] ?? "workflow-orchestrator-group"
                    };
                });

                builder.Services.AddSingleton<KafkaProducerConfig>(sp =>
                {
                    var config = sp.GetRequiredService<IConfiguration>();
                    return new KafkaProducerConfig
                    {
                        BootstrapServers = config["Kafka:BootstrapServers"] ?? "localhost:9092",
                        Username = config["Kafka:Username"] ?? "",
                        Password = config["Kafka:Password"] ?? "",
                        MessageTimeoutMs = int.Parse(config["Kafka:MessageTimeoutMs"] ?? "30000"),
                        RequestTimeoutMs = int.Parse(config["Kafka:RequestTimeoutMs"] ?? "30000")
                    };
                });

                builder.Services.AddHttpClient<IProcessorClient, ProcessorClient>();
                builder.Services.AddSingleton<IKafkaProducer, KafkaProducer>();
                builder.Services.AddSingleton<IKafkaMessageHandler<RuntimeTelemetryEvent>, RuntimeEventMessageHandler>();
                builder.Services.AddSingleton<IKafkaConsumer, KafkaConsumer<RuntimeTelemetryEvent>>();
                builder.Services.AddHostedService<RuntimeEventConsumerService>();

                builder.WebHost.AddApplicationConfiguration<ApplicationSettings>();
                builder.Services.AddPlatformServices(builder.Configuration);

                var app = builder.Build();

                app.UseSerilogRequestLogging();
                app.UseCors("AllowBlazorClient");
                app.ConfigureApplication();

                Log.Information("Workflow Orchestrator starting...");
                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Workflow Orchestrator failed to start");
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
