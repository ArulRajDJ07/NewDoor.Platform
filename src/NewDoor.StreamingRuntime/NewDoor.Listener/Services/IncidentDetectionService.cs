using NewDoor.Listener.Models;

namespace NewDoor.Listener.Services;

public interface IIncidentDetectionService
{
    Task<IncidentDetectedEvent?> DetectIncidentAsync(EnrichedTelemetryEvent telemetryEvent, CancellationToken cancellationToken = default);
}

public class IncidentDetectionService : IIncidentDetectionService
{
    private readonly ILogger<IncidentDetectionService> _logger;
    private readonly IConfiguration _configuration;

    private const double HighTemperatureThreshold = 45.0;
    private const double CriticalTemperatureThreshold = 60.0;
    private const double HighSmokeThreshold = 30.0;
    private const double CriticalSmokeThreshold = 50.0;
    private const int LowBatteryThreshold = 15;
    private const int CriticalBatteryThreshold = 5;

    public IncidentDetectionService(ILogger<IncidentDetectionService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public Task<IncidentDetectedEvent?> DetectIncidentAsync(EnrichedTelemetryEvent telemetryEvent, CancellationToken cancellationToken = default)
    {
        IncidentDetectedEvent? incident = null;

        if (telemetryEvent.Payload.Temperature >= CriticalTemperatureThreshold)
        {
            incident = CreateIncident(telemetryEvent, "CRITICAL_TEMPERATURE", "Critical", 
                $"Critical temperature detected: {telemetryEvent.Payload.Temperature:F2}°C");
        }
        else if (telemetryEvent.Payload.Temperature >= HighTemperatureThreshold)
        {
            incident = CreateIncident(telemetryEvent, "HIGH_TEMPERATURE", "High", 
                $"High temperature detected: {telemetryEvent.Payload.Temperature:F2}°C");
        }
        else if (telemetryEvent.Payload.SmokeLevel >= CriticalSmokeThreshold)
        {
            incident = CreateIncident(telemetryEvent, "CRITICAL_SMOKE", "Critical", 
                $"Critical smoke level detected: {telemetryEvent.Payload.SmokeLevel:F2}%");
        }
        else if (telemetryEvent.Payload.SmokeLevel >= HighSmokeThreshold)
        {
            incident = CreateIncident(telemetryEvent, "HIGH_SMOKE", "High", 
                $"High smoke level detected: {telemetryEvent.Payload.SmokeLevel:F2}%");
        }
        else if (telemetryEvent.Payload.BatteryLevel <= CriticalBatteryThreshold)
        {
            incident = CreateIncident(telemetryEvent, "CRITICAL_BATTERY", "Medium", 
                $"Critical battery level: {telemetryEvent.Payload.BatteryLevel}%");
        }
        else if (telemetryEvent.Payload.BatteryLevel <= LowBatteryThreshold)
        {
            incident = CreateIncident(telemetryEvent, "LOW_BATTERY", "Low", 
                $"Low battery level: {telemetryEvent.Payload.BatteryLevel}%");
        }
        else if (telemetryEvent.Payload.Status.Equals("ERROR", StringComparison.OrdinalIgnoreCase) ||
                 telemetryEvent.Payload.Status.Equals("OFFLINE", StringComparison.OrdinalIgnoreCase))
        {
            incident = CreateIncident(telemetryEvent, "DEVICE_STATUS_ALERT", "Medium", 
                $"Device status alert: {telemetryEvent.Payload.Status}");
        }

        if (incident != null)
        {
            _logger.LogWarning("Incident detected: Type={IncidentType}, Severity={Severity}, DeviceId={DeviceId}, Description={Description}", 
                incident.IncidentType, incident.Severity, incident.DeviceId, incident.Description);
        }

        return Task.FromResult(incident);
    }

    private IncidentDetectedEvent CreateIncident(EnrichedTelemetryEvent telemetryEvent, string incidentType, string severity, string description)
    {
        return new IncidentDetectedEvent
        {
            CorrelationId = telemetryEvent.CorrelationId,
            EventId = telemetryEvent.EventId,
            IncidentType = incidentType,
            Severity = severity,
            DeviceId = telemetryEvent.DeviceId,
            DeviceName = telemetryEvent.DeviceName,
            DeviceType = telemetryEvent.DeviceType,
            BuildingId = telemetryEvent.BuildingId,
            BuildingCode = telemetryEvent.BuildingCode,
            Floor = telemetryEvent.Floor,
            Zone = telemetryEvent.Zone,
            DetectedAtUtc = DateTime.UtcNow,
            Description = description,
            TelemetryData = new IncidentTelemetryData
            {
                Temperature = telemetryEvent.Payload.Temperature,
                SmokeLevel = telemetryEvent.Payload.SmokeLevel,
                BatteryLevel = telemetryEvent.Payload.BatteryLevel,
                Status = telemetryEvent.Payload.Status
            }
        };
    }
}
