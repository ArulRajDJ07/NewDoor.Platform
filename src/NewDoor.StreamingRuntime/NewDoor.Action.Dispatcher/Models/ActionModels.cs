namespace NewDoor.Action.Dispatcher.Models;

public class AlarmEvent
{
    public string AlarmId { get; set; } = string.Empty;
    public string CorrelationId { get; set; } = string.Empty;
    public string IncidentId { get; set; } = string.Empty;
    public string DeviceId { get; set; } = string.Empty;
    public string DeviceName { get; set; } = string.Empty;
    public int BuildingId { get; set; }
    public string BuildingCode { get; set; } = string.Empty;
    public int RuleId { get; set; }
    public string RuleName { get; set; } = string.Empty;
    public string Floor { get; set; } = string.Empty;
    public string Zone { get; set; } = string.Empty;
    public string AlarmType { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime TriggeredAtUtc { get; set; }
    public Dictionary<string, object> Context { get; set; } = new();
}

public class IncidentEvent
{
    public string IncidentId { get; set; } = string.Empty;
    public string CorrelationId { get; set; } = string.Empty;
    public string DeviceId { get; set; } = string.Empty;
    public string DeviceName { get; set; } = string.Empty;
    public int BuildingId { get; set; }
    public string BuildingCode { get; set; } = string.Empty;
    public string IncidentType { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public double ConfidenceScore { get; set; }
    public string RuleTriggered { get; set; } = string.Empty;
    public DateTime DetectedAtUtc { get; set; }
    public Dictionary<string, object> TelemetryData { get; set; } = new();
}

public class UIBroadcastEvent
{
    public string BroadcastId { get; set; } = Guid.NewGuid().ToString();
    public string CorrelationId { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public string AlarmId { get; set; } = string.Empty;
    public string IncidentId { get; set; } = string.Empty;
    public string DeviceId { get; set; } = string.Empty;
    public string DeviceName { get; set; } = string.Empty;
    public int BuildingId { get; set; }
    public string BuildingCode { get; set; } = string.Empty;
    public string Floor { get; set; } = string.Empty;
    public string Zone { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public Dictionary<string, object> Data { get; set; } = new();
}

// Database persistence events
public class IncidentCreatedEvent
{
    public string IncidentCode { get; set; } = string.Empty;
    public string CorrelationId { get; set; } = string.Empty;
    public int BuildingId { get; set; }
    public string BuildingCode { get; set; } = string.Empty;
    public string IncidentType { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string Status { get; set; } = "Active";
    public DateTime StartedUtc { get; set; }
    public string Summary { get; set; } = string.Empty;
    public string RootCause { get; set; } = string.Empty;
    public bool TriggeredByRule { get; set; }
    public int EventCount { get; set; }
    public double ConfidenceScore { get; set; }
    public string DeviceId { get; set; } = string.Empty;
    public string DeviceName { get; set; } = string.Empty;
    public Dictionary<string, object> TelemetryData { get; set; } = new();
}

public class AlarmCreatedEvent
{
    public string AlarmCode { get; set; } = string.Empty;
    public string CorrelationId { get; set; } = string.Empty;
    public string DeviceId { get; set; } = string.Empty;
    public int BuildingId { get; set; }
    public string BuildingCode { get; set; } = string.Empty;
    public int RuleId { get; set; }
    public string IncidentCode { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string AlarmMessage { get; set; } = string.Empty;
    public string AlarmStatus { get; set; } = "Active";
    public DateTime TriggeredUtc { get; set; }
    public string TriggeredBy { get; set; } = "System";
    public string AlarmType { get; set; } = string.Empty;
    public string Floor { get; set; } = string.Empty;
    public string Zone { get; set; } = string.Empty;
    public Dictionary<string, object> Context { get; set; } = new();
}

public class AuditHistoryEvent
{
    public string CorrelationId { get; set; } = string.Empty;
    public string EventIdGuid { get; set; } = string.Empty; // GUID for correlation
    public int EventId { get; set; } // Will be set after Event record creation
    public string DeviceId { get; set; } = string.Empty;
    public int BuildingId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string ProcessingResult { get; set; } = string.Empty;
    public string ProcessorName { get; set; } = "Action.Dispatcher";
    public string Remarks { get; set; } = string.Empty;
    public DateTime ProcessedUtc { get; set; }
    public DateTime EventUtc { get; set; } = DateTime.UtcNow;

    // Telemetry data for Event creation
    public double Temperature { get; set; }
    public double SmokeLevel { get; set; }
    public double BatteryLevel { get; set; }
    public double SignalStrength { get; set; }

    public Dictionary<string, object> Metadata { get; set; } = new();
}
