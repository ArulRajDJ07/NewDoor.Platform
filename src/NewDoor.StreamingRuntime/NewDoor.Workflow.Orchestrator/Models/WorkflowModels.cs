namespace NewDoor.Workflow.Orchestrator.Models;

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
    public string RequestId { get; set; } = Guid.NewGuid().ToString();
    public string CorrelationId { get; set; } = string.Empty;
    public RuntimeTelemetryEvent Event { get; set; } = new();
    public DateTime RequestedAtUtc { get; set; } = DateTime.UtcNow;
}

public class ProcessorResponse
{
    public string ResponseId { get; set; } = string.Empty;
    public string RequestId { get; set; } = string.Empty;
    public string CorrelationId { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty; // incident, alarm, audit, notification, escalation, workflow
    public bool IsIncident { get; set; }
    public bool IsAlarm { get; set; }
    public string IncidentType { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public double ConfidenceScore { get; set; }
    public string RuleTriggered { get; set; } = string.Empty;
    public Dictionary<string, object> AdditionalData { get; set; } = new();
    public DateTime ProcessedAtUtc { get; set; }
}

public class IncidentEvent
{
    public string IncidentId { get; set; } = Guid.NewGuid().ToString();
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

public class AlarmEvent
{
    public string AlarmId { get; set; } = Guid.NewGuid().ToString();
    public string CorrelationId { get; set; } = string.Empty;
    public string IncidentId { get; set; } = string.Empty;
    public string DeviceId { get; set; } = string.Empty;
    public string DeviceName { get; set; } = string.Empty;
    public int BuildingId { get; set; }
    public string BuildingCode { get; set; } = string.Empty;
    public string Floor { get; set; } = string.Empty;
    public string Zone { get; set; } = string.Empty;
    public string AlarmType { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime TriggeredAtUtc { get; set; }
    public Dictionary<string, object> Context { get; set; } = new();
}

public class AuditHistoryEvent
{
    public string AuditId { get; set; } = Guid.NewGuid().ToString();
    public string CorrelationId { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public string DeviceId { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public class ActionDispatchRequest
{
    public string ActionId { get; set; } = Guid.NewGuid().ToString();
    public string CorrelationId { get; set; } = string.Empty;
    public string ActionType { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string IncidentType { get; set; } = string.Empty;
    public string DeviceId { get; set; } = string.Empty;
    public int BuildingId { get; set; }
    public string BuildingCode { get; set; } = string.Empty;
    public string Floor { get; set; } = string.Empty;
    public string Zone { get; set; } = string.Empty;
    public DateTime RequestedAtUtc { get; set; } = DateTime.UtcNow;
    public Dictionary<string, object> Context { get; set; } = new();
}

public class ActionDispatchResponse
{
    public string DispatchId { get; set; } = string.Empty;
    public string ActionId { get; set; } = string.Empty;
    public string CorrelationId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public List<string> ActionsTriggered { get; set; } = new();
    public DateTime DispatchedAtUtc { get; set; }
    public Dictionary<string, object> Results { get; set; } = new();
}

public class RuntimeResultEvent
{
    public string ResultId { get; set; } = Guid.NewGuid().ToString();
    public string WorkflowId { get; set; } = string.Empty;
    public string CorrelationId { get; set; } = string.Empty;
    public string EventId { get; set; } = string.Empty;
    public string DeviceId { get; set; } = string.Empty;
    public string ProcessingStatus { get; set; } = string.Empty;
    public bool IsIncident { get; set; }
    public bool IsAlarm { get; set; }
    public string Severity { get; set; } = string.Empty;
    public bool ActionDispatched { get; set; }
    public DateTime ProcessedAtUtc { get; set; }
    public double ProcessingDurationMs { get; set; }
}

public class WorkflowExecutionResult
{
    public string WorkflowId { get; set; } = string.Empty;
    public string CorrelationId { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public string? ErrorMessage { get; set; }
    public ProcessorResponse ProcessorResponse { get; set; } = new();
    public ActionDispatchResponse? ActionResponse { get; set; }
    public DateTime StartedAtUtc { get; set; }
    public DateTime? ProcessingCompletedAtUtc { get; set; }
    public DateTime? ActionDispatchedAtUtc { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
}
