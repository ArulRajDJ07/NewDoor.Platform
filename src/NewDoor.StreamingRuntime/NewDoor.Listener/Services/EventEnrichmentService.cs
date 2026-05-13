namespace NewDoor.Listener.Services;

/// <summary>
/// Service responsible for event enrichment, categorization, and routing decisions
/// Implements: Metadata Enrichment, Event Normalization
/// Maps Device Simulator events to workflow pipelines
/// </summary>
public class EventEnrichmentService : IEventEnrichmentService
{
    private readonly ILogger<EventEnrichmentService> _logger;

    public EventEnrichmentService(ILogger<EventEnrichmentService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Determines the event category based on event type from Device Simulator
    /// Supports: Heartbeat, SmokeDetected, HeatSpike, DeviceOffline, DeviceFailure
    /// </summary>
    public string DetermineEventCategory(string eventType)
    {
        return eventType?.ToUpperInvariant() switch
        {
            // INCIDENT - Fire/Smoke/Heat events requiring immediate response
            "SMOKEDETECTED" => "INCIDENT",
            "HEATSPIKE" => "INCIDENT",

            // WARNING - Device health/connectivity issues
            "DEVICEOFFLINE" => "WARNING",
            "DEVICEFAILURE" => "WARNING",

            // NORMAL - Routine operational events
            "HEARTBEAT" => "NORMAL",

            // Fallback for unknown event types
            _ => "UNKNOWN"
        };
    }

    /// <summary>
    /// Determines which workflow pipeline should process the event
    /// </summary>
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

    /// <summary>
    /// Determines event priority based on Device Simulator event type and telemetry values
    /// Priority Levels: CRITICAL > HIGH > MEDIUM > LOW
    /// </summary>
    public string DeterminePriority(string eventType, double smokeLevel, double temperature)
    {
        var normalizedEventType = eventType?.ToUpperInvariant() ?? string.Empty;

        // 1. HEARTBEAT → LOW Priority
        if (normalizedEventType == "HEARTBEAT")
        {
            return "LOW";
        }

        // 2. SMOKEDETECTED → CRITICAL or HIGH
        if (normalizedEventType == "SMOKEDETECTED")
        {
            // CRITICAL: smokeLevel > 80 OR temperature > 80
            if (smokeLevel > 80 || temperature > 80)
                return "CRITICAL";

            // HIGH: smokeLevel 60-80 AND temperature 45-80
            return "HIGH";
        }

        // 3. HEATSPIKE → CRITICAL, HIGH, or MEDIUM
        if (normalizedEventType == "HEATSPIKE")
        {
            // CRITICAL: temperature > 100
            if (temperature > 100)
                return "CRITICAL";

            // HIGH: temperature >= 80
            if (temperature >= 80)
                return "HIGH";

            // MEDIUM: temperature 60-79 (or lower, for HeatSpike it's still concerning)
            return "MEDIUM";
        }

        // 4. DEVICEOFFLINE → MEDIUM Priority
        if (normalizedEventType == "DEVICEOFFLINE")
        {
            return "MEDIUM";
        }

        // 5. DEVICEFAILURE → MEDIUM Priority
        if (normalizedEventType == "DEVICEFAILURE")
        {
            return "MEDIUM";
        }

        // Default for unknown event types → LOW
        _logger.LogWarning("Unknown event type '{EventType}', defaulting to LOW priority", eventType);
        return "LOW";
    }
}
