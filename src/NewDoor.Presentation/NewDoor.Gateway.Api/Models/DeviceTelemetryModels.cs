using NewDoor.Platform.DTO.Common;

namespace NewDoor.Gateway.Api.Models;

public class DeviceTelemetryRequest : BaseDeviceDto
{
    public string EventType { get; set; } = string.Empty;
    public DateTime TimestampUtc { get; set; }
    public string Status { get; set; } = string.Empty;
    public int BatteryLevel { get; set; }
    public double? Temperature { get; set; }
    public double? SmokeLevel { get; set; }
    public string? SignalStrength { get; set; }
}

public class EnrichedTelemetryEvent : BaseEvent
{
    public string EventName { get; set; } = "DeviceTelemetryReceived";
    public string EventType { get; set; } = string.Empty;
    public string DeviceId { get; set; } = string.Empty;
    public string DeviceName { get; set; } = string.Empty;
    public string DeviceType { get; set; } = string.Empty;
    public int BuildingId { get; set; }
    public string BuildingCode { get; set; } = string.Empty;
    public string Floor { get; set; } = string.Empty;
    public string Zone { get; set; } = string.Empty;
    public TelemetryPayload Payload { get; set; } = new();
    public TelemetryMetadata Metadata { get; set; } = new();

    public EnrichedTelemetryEvent() : base()
    {
    }

    public EnrichedTelemetryEvent(string correlationId) : base(correlationId)
    {
    }
}

public class TelemetryPayload
{
    public double Temperature { get; set; }
    public double SmokeLevel { get; set; }
    public int BatteryLevel { get; set; }
    public string SignalStrength { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

public class TelemetryMetadata : BaseMetadata
{
    public TelemetryMetadata() : base("NewDoor.Gateway.Api")
    {
    }

    public TelemetryMetadata(string source) : base(source)
    {
    }
}
