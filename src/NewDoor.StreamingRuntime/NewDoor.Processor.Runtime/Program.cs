using DoWhatta.Platform.Builder;
using DoWhatta.Platform.Builder.Output;
using DoWhatta.Platform.Builder.Output.Writers;
using DoWhatta.Platform.Core.Common;
using DoWhatta.Platform.Core.Settings;
using DoWhatta.Platform.Data.Extensions;
using DoWhatta.Platform.Infrastructure.HttpClients;
using DoWhatta.Platform.Infrastructure.Messaging.ServiceBus;
using NewDoor.Processor.Runtime.Settings;
using NewDoor.Processor.Runtime.Services;
using NewDoor.EventBus.Consumers;
using NewDoor.EventBus.Producers;
using NewDoor.Processor.Runtime.Models;
using NewDoor.Processor.Runtime.Handlers;
using NewDoor.Processor.Runtime.BackgroundServices;
using Serilog;
using ApplicationSettings = NewDoor.Processor.Runtime.Settings.ApplicationSettings;

namespace NewDoor.Processor.Runtime
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

                builder.Services.AddControllers();
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();

                // Register business services
                builder.Services.AddSingleton<IEventProcessorService, EventProcessorService>();

                // Register Kafka configuration
                builder.Services.AddSingleton<KafkaConsumerConfig>(sp =>
                {
                    var config = sp.GetRequiredService<IConfiguration>();
                    return new KafkaConsumerConfig
                    {
                        BootstrapServers = config["Kafka:BootstrapServers"] ?? "localhost:9092",
                        Username = config["Kafka:Username"] ?? "",
                        Password = config["Kafka:Password"] ?? "",
                        GroupId = config["Kafka:GroupId"] ?? "processor-runtime-group"
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

                // Register Kafka producer
                builder.Services.AddSingleton<IKafkaProducer, KafkaProducer>();

                // Register Kafka consumer and handler
                builder.Services.AddSingleton<IKafkaMessageHandler<ProcessorRequest>, ProcessingRequestMessageHandler>();
                builder.Services.AddSingleton<IKafkaConsumer, KafkaConsumer<ProcessorRequest>>();

                // Register background service
                builder.Services.AddHostedService<ProcessingRequestConsumerService>();

                builder.WebHost.AddApplicationConfiguration<ApplicationSettings>();
                builder.Services.AddPlatformServices(builder.Configuration);

                var app = builder.Build();

                app.UseSwagger();
                app.UseSwaggerUI();

                app.UseSerilogRequestLogging();
                app.UseCors("AllowBlazorClient");
                app.UseHttpsRedirection();
                app.UseAuthorization();
                app.MapControllers();
                app.ConfigureApplication();

                Log.Information("Processor Runtime starting...");
                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Processor Runtime failed to start");
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
