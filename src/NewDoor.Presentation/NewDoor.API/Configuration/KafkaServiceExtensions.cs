using NewDoor.EventBus.Consumers;
using NewDoor.API.Models;
using NewDoor.API.Handlers;
using NewDoor.API.BackgroundServices;

namespace NewDoor.API.Configuration;

public static class KafkaServiceExtensions
{
    /// <summary>
    /// Registers all Kafka consumers with their message handlers using the keyed service pattern
    /// </summary>
    public static IServiceCollection AddKafkaConsumers(this IServiceCollection services, IConfiguration configuration)
    {
        // Register Kafka consumer configuration
        services.AddSingleton<KafkaConsumerConfig>(sp =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            return new KafkaConsumerConfig
            {
                BootstrapServers = config["Kafka:BootstrapServers"] ?? "localhost:9092",
                Username = config["Kafka:Username"] ?? "",
                Password = config["Kafka:Password"] ?? "",
                GroupId = config["Kafka:GroupId"] ?? "api-consumer-group"
            };
        });

        // Register topic configurations as keyed services
        RegisterTopicConfiguration(services, configuration, 
            KafkaConsumerKeys.UIBroadcast, 
            "Kafka:UIBroadcastTopic", 
            "newdoor.ui.broadcast",
            "Real-time UI broadcast events");

        RegisterTopicConfiguration(services, configuration, 
            KafkaConsumerKeys.IncidentCreated, 
            "Kafka:IncidentCreatedTopic", 
            "newdoor.incident.created",
            "Incident creation events for persistence");

        RegisterTopicConfiguration(services, configuration, 
            KafkaConsumerKeys.AlarmCreated, 
            "Kafka:AlarmCreatedTopic", 
            "newdoor.alarm.created",
            "Alarm creation events for persistence");

        RegisterTopicConfiguration(services, configuration, 
            KafkaConsumerKeys.AuditHistory, 
            "Kafka:AuditHistoryTopic", 
            "newdoor.audit.history",
            "Audit history events for compliance tracking");

        // Register message handlers and consumers
        RegisterConsumer<UIBroadcastEvent, UIBroadcastMessageHandler, UIBroadcastConsumerService>(
            services, KafkaConsumerKeys.UIBroadcast);

        RegisterConsumer<IncidentCreatedEvent, IncidentCreatedMessageHandler, IncidentCreatedConsumerService>(
            services, KafkaConsumerKeys.IncidentCreated);

        RegisterConsumer<AlarmCreatedEvent, AlarmCreatedMessageHandler, AlarmCreatedConsumerService>(
            services, KafkaConsumerKeys.AlarmCreated);

        RegisterConsumer<AuditHistoryEvent, AuditHistoryMessageHandler, AuditHistoryConsumerService>(
            services, KafkaConsumerKeys.AuditHistory);

        return services;
    }

    private static void RegisterTopicConfiguration(
        IServiceCollection services, 
        IConfiguration configuration,
        string consumerKey,
        string configKey,
        string defaultTopic,
        string description)
    {
        var topicName = configuration[configKey] ?? defaultTopic;
        services.AddKeyedSingleton(consumerKey, new KafkaTopicConfiguration
        {
            TopicName = topicName,
            ConsumerKey = consumerKey,
            Description = description
        });
    }

    private static void RegisterConsumer<TEvent, THandler, THostedService>(
        IServiceCollection services,
        string consumerKey)
        where TEvent : class
        where THandler : class, IKafkaMessageHandler<TEvent>
        where THostedService : class, IHostedService
    {
        // Register message handler
        services.AddSingleton<IKafkaMessageHandler<TEvent>, THandler>();

        // Register consumer with keyed service
        services.AddKeyedSingleton<IKafkaConsumer, KafkaConsumer<TEvent>>(consumerKey);

        // Register background service
        services.AddHostedService<THostedService>();
    }
}
