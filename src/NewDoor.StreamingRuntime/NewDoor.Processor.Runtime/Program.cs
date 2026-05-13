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

                builder.Services.AddSingleton<IEventProcessorService, EventProcessorService>();

                builder.WebHost.AddApplicationConfiguration<ApplicationSettings>();
                builder.Services.AddPlatformServices(builder.Configuration);

                var app = builder.Build();

                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

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
