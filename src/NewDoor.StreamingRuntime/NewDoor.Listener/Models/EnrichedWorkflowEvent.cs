namespace NewDoor.Listener.Models;

/// <summary>
/// Enriched and normalized event ready for workflow processing
/// Published to: newdoor.workflow.events
/// </summary>
public class EnrichedWorkflowEvent
{
    public string EventId { get; set; } = string.Empty;
    public string CorrelationId { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public string EventCategory { get; set; } = string.Empty;

    public DeviceInfo Device { get; set; } = new();
    public LocationInfo Location { get; set; } = new();
    public TelemetryData Telemetry { get; set; } = new();
    public RuntimeInfo Runtime { get; set; } = new();
    public EventMetadata Metadata { get; set; } = new();
}

public class DeviceInfo
{
    public string DeviceId { get; set; } = string.Empty;
    public string DeviceType { get; set; } = string.Empty;
    public string DeviceName { get; set; } = string.Empty;
}

public class LocationInfo
{
    public int BuildingId { get; set; }
    public string BuildingCode { get; set; } = string.Empty;
    public string Floor { get; set; } = string.Empty;
    public string Zone { get; set; } = string.Empty;
}

public class TelemetryData
{
    public double Temperature { get; set; }
    public double SmokeLevel { get; set; }
    public int BatteryLevel { get; set; }
}

public class RuntimeInfo
{
    public string Pipeline { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    // Note: RequiresCorrelation and RequiresRuleEvaluation flags will be set by Orchestrator
}

public class EventMetadata
{
    public string ReceivedBy { get; set; } = "NewDoor.Listener";
    public DateTime NormalizedUtc { get; set; } = DateTime.UtcNow;
}
