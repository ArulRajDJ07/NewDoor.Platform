namespace NewDoor.Listener.Services;

public class EventEnrichmentService : IEventEnrichmentService
{
    #region Fields
    private readonly ILogger<EventEnrichmentService> _logger;
    #endregion

    #region Constructor
    public EventEnrichmentService(ILogger<EventEnrichmentService> logger)
    {
        _logger = logger;
    }
    #endregion

    #region Methods
    public string DetermineEventCategory(string eventType)
    {
        return eventType?.ToUpperInvariant() switch
        {
            "SMOKEDETECTED" => "INCIDENT",
            "HEATSPIKE" => "INCIDENT",
            "DEVICEOFFLINE" => "WARNING",
            "DEVICEFAILURE" => "WARNING",
            "HEARTBEAT" => "NORMAL",
            _ => "UNKNOWN"
        };
    }

    public string DeterminePipeline(string eventCategory)
    {
        return eventCategory switch
        {
            "INCIDENT" => "IncidentPipeline",
            "WARNING" => "AlertPipeline",
            "NORMAL" => "MonitoringPipeline",
            _ => "DefaultPipeline"
        };
    }

    public string DeterminePriority(string eventType, double smokeLevel, double temperature)
    {
        var normalizedEventType = eventType?.ToUpperInvariant() ?? string.Empty;

        if (normalizedEventType == "HEARTBEAT")
            return "LOW";

        if (normalizedEventType == "SMOKEDETECTED")
        {
            if (smokeLevel > 80 || temperature > 80)
                return "CRITICAL";
            return "HIGH";
        }

        if (normalizedEventType == "HEATSPIKE")
        {
            if (temperature > 100)
                return "CRITICAL";
            if (temperature >= 80)
                return "HIGH";
            return "MEDIUM";
        }

        if (normalizedEventType == "DEVICEOFFLINE")
            return "MEDIUM";

        if (normalizedEventType == "DEVICEFAILURE")
            return "MEDIUM";

        return "LOW";
    }
    #endregion
}
