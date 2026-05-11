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
using ApplicationSettings = NewDoor.API.Settings.ApplicationSettings;

namespace NewDoor.API
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

            app.UseCors("AllowBlazorClient");
            app.ConfigureApplication();
            // yet to implement NotificationHub

            app.Run();
        }
    }
}
