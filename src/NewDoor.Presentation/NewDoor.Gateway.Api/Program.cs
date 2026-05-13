using NewDoor.Gateway.Api.Services;
using NewDoor.EventBus;
using Serilog;

namespace NewDoor.Gateway.Api
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

                // Configure CORS for Blazor clients
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

                // Configure API controllers and Swagger
                builder.Services.AddControllers();
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                    {
                        Title = "NewDoor Gateway API",
                        Version = "v1",
                        Description = "IoT Device Telemetry Ingestion API - Kafka Integration"
                    });
                });

                // Register Event Bus Producer (Kafka/ServiceBus) - automatically selects based on appsettings.json
                builder.Services.AddEventBusProducer(builder.Configuration);

                // Add Device Enrichment Service
                builder.Services.AddSingleton<IDeviceEnrichmentService, DeviceEnrichmentService>();

                var app = builder.Build();

                // Enable Swagger
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "NewDoor Gateway API v1");
                    c.RoutePrefix = "swagger";
                    c.DocumentTitle = "NewDoor Gateway API Documentation";
                });

                // Configure middleware pipeline
                app.UseCors("AllowBlazorClient");
                app.UseHttpsRedirection();
                app.UseAuthorization();
                app.MapControllers();

                // Health check endpoint
                app.MapGet("/health", () => new { status = "healthy", service = "NewDoor.Gateway.Api", timestamp = DateTime.UtcNow })
                   .WithTags("Health")
                   .WithName("GetHealth")
                   .Produces(200);

                // Root endpoint - redirect to Swagger
                app.MapGet("/", (HttpContext context) => 
                {
                    context.Response.Redirect("/swagger");
                    return Task.CompletedTask;
                })
                .ExcludeFromDescription();

                Log.Information("NewDoor Gateway API starting...");

                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application startup failed");
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
