namespace NewDoor.EventBus.Consumers;

public class KafkaConsumerConfig
{
    public string BootstrapServers { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string GroupId { get; set; } = string.Empty;
}
