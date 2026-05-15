namespace NewDoor.Web.Models;

public class DashboardAlert
{
    public string AlertId { get; set; } = string.Empty;
    public string DeviceId { get; set; } = string.Empty;
    public string DeviceName { get; set; } = string.Empty;
    public string BuildingCode { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public Dictionary<string, object> AdditionalData { get; set; } = new();
}

public class IncidentNotification
{
    public DashboardAlert Alert { get; set; } = new();
    public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;
    public bool IsNew { get; set; } = true;
}

public class AlarmNotification
{
    public DashboardAlert Alert { get; set; } = new();
    public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;
    public bool IsNew { get; set; } = true;
}

public class AuditHistoryNotification
{
    public int AuditId { get; set; }
    public string CorrelationId { get; set; } = string.Empty;
    public int EventId { get; set; }
    public int DeviceId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string ProcessingResult { get; set; } = string.Empty;
    public string ProcessorName { get; set; } = string.Empty;
    public string Remarks { get; set; } = string.Empty;
    public DateTime ProcessedUtc { get; set; }
    public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;
    public bool IsNew { get; set; } = true;
}

public enum ConnectionState
{
    Disconnected,
    Connecting,
    Connected,
    Reconnecting,
    Failed
}

public class ConnectionStateChangedEventArgs : EventArgs
{
    public ConnectionState State { get; set; }
    public string? Message { get; set; }
}
