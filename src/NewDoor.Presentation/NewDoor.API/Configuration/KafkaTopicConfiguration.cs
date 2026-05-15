namespace NewDoor.API.Configuration;

/// <summary>
/// Kafka topic configuration for a specific consumer
/// </summary>
public class KafkaTopicConfiguration
{
    /// <summary>
    /// The Kafka topic name to consume from
    /// </summary>
    public required string TopicName { get; init; }

    /// <summary>
    /// The consumer key identifier (matches the keyed service registration)
    /// </summary>
    public required string ConsumerKey { get; init; }

    /// <summary>
    /// Description of what this consumer handles
    /// </summary>
    public string? Description { get; init; }
}

/// <summary>
/// Well-known consumer keys for Kafka topic registration
/// </summary>
public static class KafkaConsumerKeys
{
    public const string UIBroadcast = "UIBroadcast";
    public const string IncidentCreated = "IncidentCreated";
    public const string AlarmCreated = "AlarmCreated";
    public const string AuditHistory = "AuditHistory";
}
