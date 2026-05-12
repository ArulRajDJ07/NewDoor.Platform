namespace NewDoor.EventBus.Producers;

public class ServiceBusProducerConfig
{
    public string ConnectionString { get; set; } = string.Empty;
    public string TelemetryTopic { get; set; } = "newdoor-device-telemetry";
    public string IncidentTopic { get; set; } = "newdoor-incident-detected";
    public string AlarmTopic { get; set; } = "newdoor-alarm-triggered";
    public string UiBroadcastTopic { get; set; } = "newdoor-ui-broadcast";
    public string AuditHistoryTopic { get; set; } = "newdoor-audit-history";
}
