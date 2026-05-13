namespace NewDoor.Listener.Models;

public class RuntimeTelemetryEvent
{
    public string EventId { get; set; } = Guid.NewGuid().ToString();
    public string CorrelationId { get; set; } = string.Empty;
    public string DeviceId { get; set; } = string.Empty;
    public string DeviceName { get; set; } = string.Empty;
    public string DeviceType { get; set; } = string.Empty;
    public int BuildingId { get; set; }
    public string BuildingCode { get; set; } = string.Empty;
    public string Floor { get; set; } = string.Empty;
    public string Zone { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public DateTime TimestampUtc { get; set; }
    public double Temperature { get; set; }
    public double SmokeLevel { get; set; }
    public int BatteryLevel { get; set; }
    public string SignalStrength { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Source { get; set; } = "NewDoor.Listener";
}
