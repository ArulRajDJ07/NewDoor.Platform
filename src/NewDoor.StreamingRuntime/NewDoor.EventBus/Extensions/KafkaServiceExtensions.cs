using Microsoft.Extensions.DependencyInjection;
using NewDoor.EventBus.Producers;
using NewDoor.EventBus.Consumers;

namespace NewDoor.EventBus.Extensions;

public static class KafkaServiceExtensions
{
    public static IServiceCollection AddKafkaProducer(
        this IServiceCollection services, 
        Action<KafkaProducerConfig> configureOptions)
    {
        var config = new KafkaProducerConfig();
        configureOptions(config);

        services.AddSingleton(config);
        services.AddSingleton<IKafkaProducer, KafkaProducer>();

        return services;
    }

    public static IServiceCollection AddKafkaConsumer<T, THandler>(
        this IServiceCollection services,
        Action<KafkaConsumerConfig> configureOptions)
        where THandler : class, IKafkaMessageHandler<T>
    {
        var config = new KafkaConsumerConfig();
        configureOptions(config);

        services.AddSingleton(config);
        services.AddSingleton<IKafkaMessageHandler<T>, THandler>();
        services.AddSingleton<KafkaConsumer<T>>();

        return services;
    }
}
