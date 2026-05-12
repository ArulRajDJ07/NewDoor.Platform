namespace NewDoor.EventBus.Producers;

public interface IKafkaProducer
{
    Task PublishAsync<T>(string topic, string key, T message, CancellationToken cancellationToken = default);
    Task PublishBatchAsync<T>(string topic, IEnumerable<(string Key, T Message)> messages, CancellationToken cancellationToken = default);
}
