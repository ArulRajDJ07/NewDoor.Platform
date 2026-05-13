namespace NewDoor.Listener.Models;

public class IncidentDetectedEvent
{
    public string IncidentId { get; set; } = Guid.NewGuid().ToString();
    public string CorrelationId { get; set; } = string.Empty;
    public string EventId { get; set; } = string.Empty;
    public string IncidentType { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string DeviceId { get; set; } = string.Empty;
    public string DeviceName { get; set; } = string.Empty;
    public string DeviceType { get; set; } = string.Empty;
    public int BuildingId { get; set; }
    public string BuildingCode { get; set; } = string.Empty;
    public string Floor { get; set; } = string.Empty;
    public string Zone { get; set; } = string.Empty;
    public DateTime DetectedAtUtc { get; set; } = DateTime.UtcNow;
    public string Description { get; set; } = string.Empty;
    public IncidentTelemetryData TelemetryData { get; set; } = new();
    public IncidentMetadata Metadata { get; set; } = new();
}

public class IncidentTelemetryData
{
    public double? Temperature { get; set; }
    public double? SmokeLevel { get; set; }
    public int BatteryLevel { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class IncidentMetadata
{
    public string Source { get; set; } = "NewDoor.Listener";
    public DateTime ProcessedUtc { get; set; } = DateTime.UtcNow;
    public string ProcessedBy { get; set; } = "IncidentDetectionService";
}
