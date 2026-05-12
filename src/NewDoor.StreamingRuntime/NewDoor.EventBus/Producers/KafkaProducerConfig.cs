namespace NewDoor.EventBus.Producers;

public class KafkaProducerConfig
{
    public string BootstrapServers { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
