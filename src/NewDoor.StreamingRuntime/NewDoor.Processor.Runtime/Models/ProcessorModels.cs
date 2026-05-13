namespace NewDoor.Processor.Runtime.Models;

public class RuntimeTelemetryEvent
{
    public string EventId { get; set; } = string.Empty;
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
    public string Source { get; set; } = string.Empty;
}

public class ProcessorRequest
{
    public string RequestId { get; set; } = string.Empty;
    public string CorrelationId { get; set; } = string.Empty;
    public RuntimeTelemetryEvent Event { get; set; } = new();
    public DateTime RequestedAtUtc { get; set; }
}

public class ProcessorResponse
{
    public string ResponseId { get; set; } = Guid.NewGuid().ToString();
    public string RequestId { get; set; } = string.Empty;
    public string CorrelationId { get; set; } = string.Empty;
    public bool IsIncident { get; set; }
    public bool IsAlarm { get; set; }
    public string IncidentType { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public double ConfidenceScore { get; set; }
    public string RuleTriggered { get; set; } = string.Empty;
    public Dictionary<string, object> AdditionalData { get; set; } = new();
    public DateTime ProcessedAtUtc { get; set; } = DateTime.UtcNow;
}

public class AuditHistoryRecord
{
    public string AuditId { get; set; } = Guid.NewGuid().ToString();
    public string RequestId { get; set; } = string.Empty;
    public string CorrelationId { get; set; } = string.Empty;
    public string EventId { get; set; } = string.Empty;
    public string DeviceId { get; set; } = string.Empty;
    public string DeviceName { get; set; } = string.Empty;
    public int BuildingId { get; set; }
    public string BuildingCode { get; set; } = string.Empty;
    public string Floor { get; set; } = string.Empty;
    public string Zone { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public string ProcessingStage { get; set; } = "RuleEvaluation";
    public string Action { get; set; } = string.Empty;
    public string Result { get; set; } = string.Empty;
    public string RuleTriggered { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public bool IsIncident { get; set; }
    public bool IsAlarm { get; set; }
    public string IncidentType { get; set; } = string.Empty;
    public double ConfidenceScore { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
    public DateTime ProcessedAtUtc { get; set; } = DateTime.UtcNow;
}
