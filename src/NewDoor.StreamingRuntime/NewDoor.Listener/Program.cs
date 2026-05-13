using NewDoor.EventBus;
using NewDoor.EventBus.Consumers;
using NewDoor.EventBus.Producers;
using NewDoor.Listener.BackgroundServices;
using NewDoor.Listener.Models;
using NewDoor.Listener.Services;
using NewDoor.Listener.Settings;
using Serilog;

namespace NewDoor.Listener
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var builder = WebApplication.CreateBuilder(args);

                // Configure Serilog
                Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(builder.Configuration)
                    .Enrich.FromLogContext()
                    .WriteTo.Console()
                    .CreateLogger();

                builder.Host.UseSerilog();

                // Bind application settings
                var appSettings = new ApplicationSettings();
                builder.Configuration.Bind(appSettings);
                builder.Services.AddSingleton(appSettings);

                // Add CORS policy
                builder.Services.AddCors(options =>
                {
                    options.AddPolicy("AllowBlazorClient",
                        policy =>
                        {
                            policy
                                 .WithOrigins(
                                     "https://localhost:7092",                 // Local Blazor
                                     "http://localhost:7092",
                                     "https://dowhatta.azurewebsites.net"      // Azure Blazor App
                                 )
                                 .AllowAnyHeader()
                                 .AllowAnyMethod()
                                 .AllowCredentials(); // only if using auth cookies / tokens
                        });
                });

                // Add controllers
                builder.Services.AddControllers();
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                    {
                        Title = "NewDoor Listener API",
                        Version = "v1",
                        Description = "NewDoor Telemetry Listener Service"
                    });
                });

                // Register business services
                builder.Services.AddSingleton<IIncidentDetectionService, IncidentDetectionService>();
                builder.Services.AddSingleton<IKafkaMessageHandler<EnrichedTelemetryEvent>, TelemetryMessageHandler>();

                // Register Event Bus (Kafka/ServiceBus) - automatically selects based on appsettings.json
                builder.Services.AddEventBusProducer(builder.Configuration);
                builder.Services.AddEventBusConsumer<EnrichedTelemetryEvent>(builder.Configuration);

                builder.Services.AddHostedService<TelemetryConsumerService>();

                var app = builder.Build();

                // Configure middleware pipeline
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI(c =>
                    {
                        c.SwaggerEndpoint("/swagger/v1/swagger.json", "NewDoor Listener API v1");
                        c.RoutePrefix = "swagger"; // Access Swagger at /swagger
                    });
                }

                app.UseSerilogRequestLogging();
                app.UseCors("AllowBlazorClient");
                app.UseHttpsRedirection();
                app.UseAuthorization();
                app.MapControllers();

                Log.Information("NewDoor Listener starting...");
                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "NewDoor Listener failed to start");
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
