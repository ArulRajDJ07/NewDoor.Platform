namespace NewDoor.Listener.Settings;

public class ApplicationSettings
{
    public KafkaSettings Kafka { get; set; } = new();
    public string Environment { get; set; } = string.Empty;
}

public class KafkaSettings
{
    public string BootstrapServers { get; set; } = "localhost:9092";
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string GroupId { get; set; } = "newdoor-listener-group";
    public int MessageTimeoutMs { get; set; } = 30000;
    public int RequestTimeoutMs { get; set; } = 30000;
}
