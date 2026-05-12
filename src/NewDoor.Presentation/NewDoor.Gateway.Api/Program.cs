using DoWhatta.Platform.Builder;
using DoWhatta.Platform.Builder.Output;
using DoWhatta.Platform.Builder.Output.Writers;
using DoWhatta.Platform.Core.Common;
using DoWhatta.Platform.Core.Settings;
using DoWhatta.Platform.Data.Extensions;
using DoWhatta.Platform.Infrastructure.HttpClients;
using DoWhatta.Platform.Infrastructure.Messaging.ServiceBus;
using NewDoor.Gateway.Api.Settings;

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
                                 "https://localhost:7092",                 // Local Blazor
                                 "http://localhost:7092",
                                 "https://dowhatta.azurewebsites.net"      // Azure Blazor App
                             )
                             .AllowAnyHeader()
                             .AllowAnyMethod()
                             .AllowCredentials(); // only if using auth cookies / tokens
                    });
            });

            builder.WebHost.AddApplicationConfiguration<ApplicationSettings>();            
            builder.Services.AddPlatformServices(builder.Configuration);
           
                      

            var app = builder.Build();

           
            app.UseCors("AllowBlazorClient");
            app.ConfigureApplication();
            // yet to implement NotificationHub 

            app.Run();
        }
    }
}
