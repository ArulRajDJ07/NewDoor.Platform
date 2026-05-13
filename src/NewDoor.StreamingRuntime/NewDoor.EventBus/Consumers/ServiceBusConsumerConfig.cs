namespace NewDoor.EventBus.Consumers;

public class ServiceBusConsumerConfig
{
    public string ConnectionString { get; set; } = string.Empty;
    public string TopicName { get; set; } = string.Empty;
    public string SubscriptionName { get; set; } = string.Empty;
    public int MaxConcurrentCalls { get; set; } = 5;
    public int PrefetchCount { get; set; } = 10;
}
