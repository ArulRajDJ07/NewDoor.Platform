using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NewDoor.EventBus.Consumers;
using NewDoor.EventBus.Producers;
using System.Runtime.InteropServices;

namespace NewDoor.EventBus;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Register Event Bus consumer based on configuration and platform
    /// </summary>
    public static IServiceCollection AddEventBusConsumer<TMessage>(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<IServiceProvider, ILogger>? onRegistration = null)
    {
        var messagingProvider = configuration["Messaging:Provider"] ?? "Kafka";
        var isArm64 = RuntimeInformation.ProcessArchitecture == Architecture.Arm64;

        // Register message handler (must be done by calling application)
        
        if (messagingProvider.Equals("ServiceBus", StringComparison.OrdinalIgnoreCase))
        {
            // Azure Service Bus - Works on ARM64!
            services.AddSingleton<ServiceBusConsumerConfig>(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                return new ServiceBusConsumerConfig
                {
                    ConnectionString = config["ServiceBus:ConnectionString"] ?? 
                        throw new InvalidOperationException("ServiceBus:ConnectionString is required"),
                    TopicName = config["ServiceBus:TelemetryTopic"] ?? "telemetry-events",
                    SubscriptionName = config["ServiceBus:SubscriptionName"] ?? "listener-subscription",
                    MaxConcurrentCalls = int.Parse(config["ServiceBus:MaxConcurrentCalls"] ?? "5"),
                    PrefetchCount = int.Parse(config["ServiceBus:PrefetchCount"] ?? "10")
                };
            });
            
            services.AddSingleton<IKafkaConsumer, ServiceBusConsumer<TMessage>>();
            
            services.AddSingleton<ILogger<EventBusRegistration>>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<EventBusRegistration>>();
                logger.LogInformation("Event Bus: Using Azure Service Bus consumer (ARM64 compatible)");
                onRegistration?.Invoke(sp, logger);
                return logger;
            });
        }
        else if (isArm64)
        {
            // ARM64 + Kafka = Use Mock
            services.AddSingleton<IKafkaConsumer, MockKafkaConsumer<TMessage>>();
            
            services.AddSingleton<ILogger<EventBusRegistration>>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<EventBusRegistration>>();
                logger.LogWarning("Event Bus: ARM64 detected with Kafka provider - Using MockKafkaConsumer. Consider switching to ServiceBus for ARM64 support.");
                onRegistration?.Invoke(sp, logger);
                return logger;
            });
        }
        else
        {
            // x64 + Kafka = Use Real Kafka
            services.AddSingleton<KafkaConsumerConfig>(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                return new KafkaConsumerConfig
                {
                    BootstrapServers = config["Kafka:BootstrapServers"] ?? "localhost:9092",
                    Username = config["Kafka:Username"] ?? "",
                    Password = config["Kafka:Password"] ?? "",
                    GroupId = config["Kafka:GroupId"] ?? "default-group"
                };
            });
            
            services.AddSingleton<IKafkaConsumer, KafkaConsumer<TMessage>>();
            
            services.AddSingleton<ILogger<EventBusRegistration>>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<EventBusRegistration>>();
                logger.LogInformation("Event Bus: Using Kafka consumer");
                onRegistration?.Invoke(sp, logger);
                return logger;
            });
        }

        return services;
    }

    /// <summary>
    /// Register Event Bus producer based on configuration
    /// </summary>
    public static IServiceCollection AddEventBusProducer(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var messagingProvider = configuration["Messaging:Provider"] ?? "Kafka";

        if (messagingProvider.Equals("ServiceBus", StringComparison.OrdinalIgnoreCase))
        {
            // Azure Service Bus Producer
            services.AddSingleton<ServiceBusProducerConfig>(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                return new ServiceBusProducerConfig
                {
                    ConnectionString = config["ServiceBus:ConnectionString"] ?? 
                        throw new InvalidOperationException("ServiceBus:ConnectionString is required")
                };
            });
            
            services.AddSingleton<IKafkaProducer, ServiceBusProducer>();
            
            services.AddSingleton<ILogger<EventBusRegistration>>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<EventBusRegistration>>();
                logger.LogInformation("Event Bus: Using Azure Service Bus producer");
                return logger;
            });
        }
        else
        {
            // Kafka Producer
            services.AddSingleton<KafkaProducerConfig>(sp =>
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
            
            services.AddSingleton<IKafkaProducer, KafkaProducer>();
            
            services.AddSingleton<ILogger<EventBusRegistration>>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<EventBusRegistration>>();
                logger.LogInformation("Event Bus: Using Kafka producer");
                return logger;
            });
        }

        return services;
    }
}

// Marker class for logging
internal class EventBusRegistration { }
