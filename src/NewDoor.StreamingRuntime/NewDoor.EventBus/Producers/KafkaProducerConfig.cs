namespace NewDoor.EventBus.Producers;

public class KafkaProducerConfig
{
    public string BootstrapServers { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Message timeout in milliseconds (default: 300000 = 5 minutes)
    /// </summary>
    public int MessageTimeoutMs { get; set; } = 300000;

    /// <summary>
    /// Request timeout in milliseconds (default: 300000 = 5 minutes)
    /// </summary>
    public int RequestTimeoutMs { get; set; } = 300000;
}
