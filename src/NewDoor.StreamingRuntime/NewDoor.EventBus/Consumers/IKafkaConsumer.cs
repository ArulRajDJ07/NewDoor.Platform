namespace NewDoor.EventBus.Consumers;

public interface IKafkaConsumer
{
    Task StartConsumingAsync(string topic, CancellationToken cancellationToken);
    Task StopConsumingAsync();
}

public interface IKafkaMessageHandler<T>
{
    Task HandleAsync(string key, T message, CancellationToken cancellationToken);
}
