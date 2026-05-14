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

                #region Kafka Configuration

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

                #endregion



                #region Kafka Producer

                builder.Services.AddSingleton<IKafkaProducer, KafkaProducer>();

                #endregion

                #region Runtime Event Consumer (from Listener)

                builder.Services.AddSingleton<IKafkaMessageHandler<EnrichedWorkflowEvent>, EventMessageHandler>();
                builder.Services.AddKeyedSingleton<IKafkaConsumer>("EventConsumer", (sp, key) =>
                {
                    var consumerConfig = new KafkaConsumerConfig
                    {
                        BootstrapServers = sp.GetRequiredService<IConfiguration>()["Kafka:BootstrapServers"] ?? "localhost:9092",
                        Username = sp.GetRequiredService<IConfiguration>()["Kafka:Username"] ?? "",
                        Password = sp.GetRequiredService<IConfiguration>()["Kafka:Password"] ?? "",
                        GroupId = sp.GetRequiredService<IConfiguration>()["Kafka:RuntimeEventConsumerGroupId"] ?? "workflow-orchestrator-event-group"
                    };
                    var handler = sp.GetRequiredService<IKafkaMessageHandler<EnrichedWorkflowEvent>>();
                    var logger = sp.GetRequiredService<ILogger<KafkaConsumer<EnrichedWorkflowEvent>>>();
                    return new KafkaConsumer<EnrichedWorkflowEvent>(consumerConfig, handler, logger);
                });

                #endregion

                #region Processing Result Consumer (from Processor)

                builder.Services.AddSingleton<IKafkaMessageHandler<ProcessorResponse>, ProcessingResultMessageHandler>();
                builder.Services.AddKeyedSingleton<IKafkaConsumer>("ResultConsumer", (sp, key) =>
                {
                    var consumerConfig = new KafkaConsumerConfig
                    {
                        BootstrapServers = sp.GetRequiredService<IConfiguration>()["Kafka:BootstrapServers"] ?? "localhost:9092",
                        Username = sp.GetRequiredService<IConfiguration>()["Kafka:Username"] ?? "",
                        Password = sp.GetRequiredService<IConfiguration>()["Kafka:Password"] ?? "",
                        GroupId = sp.GetRequiredService<IConfiguration>()["Kafka:ResultConsumerGroupId"] ?? "workflow-orchestrator-result-group"
                    };
                    var handler = sp.GetRequiredService<IKafkaMessageHandler<ProcessorResponse>>();
                    var logger = sp.GetRequiredService<ILogger<KafkaConsumer<ProcessorResponse>>>();
                    return new KafkaConsumer<ProcessorResponse>(consumerConfig, handler, logger);
                });

                #endregion

                #region Background Services

                builder.Services.AddHostedService<EventConsumerService>();
                builder.Services.AddHostedService<ProcessingResultConsumerService>();

                #endregion

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
