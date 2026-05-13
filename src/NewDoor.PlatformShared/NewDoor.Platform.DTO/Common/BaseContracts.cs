namespace NewDoor.Platform.DTO.Common;

/// <summary>
/// Base class for all events with tracking identifiers
/// </summary>
public abstract class BaseEvent
{
    public string EventId { get; set; } = string.Empty;
    public string CorrelationId { get; set; } = string.Empty;
    public DateTime TimestampUtc { get; set; }

    protected BaseEvent()
    {
        EventId = Guid.NewGuid().ToString();
        CorrelationId = Guid.NewGuid().ToString();
        TimestampUtc = DateTime.UtcNow;
    }

    protected BaseEvent(string correlationId) : this()
    {
        CorrelationId = correlationId;
    }
}

/// <summary>
/// Base class for device-related DTOs
/// </summary>
public abstract class BaseDeviceDto
{
    public string DeviceId { get; set; } = string.Empty;
    public string DeviceType { get; set; } = string.Empty;
}

/// <summary>
/// Base class for location-aware entities
/// </summary>
public abstract class BaseLocationDto
{
    public int BuildingId { get; set; }
    public string BuildingCode { get; set; } = string.Empty;
    public string Floor { get; set; } = string.Empty;
    public string Zone { get; set; } = string.Empty;
}

/// <summary>
/// Base class for metadata tracking
/// </summary>
public class BaseMetadata
{
    public string Source { get; set; } = string.Empty;
    public DateTime GeneratedUtc { get; set; }

    public BaseMetadata()
    {
        GeneratedUtc = DateTime.UtcNow;
    }

    public BaseMetadata(string source) : this()
    {
        Source = source;
    }
}
