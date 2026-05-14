using NewDoor.EventBus.Consumers;
using NewDoor.EventBus.Producers;
using NewDoor.Action.Dispatcher.Models;

namespace NewDoor.Action.Dispatcher.Handlers;

public class IncidentMessageHandler : IKafkaMessageHandler<IncidentEvent>
{
    #region Fields
    private readonly IKafkaProducer _kafkaProducer;
    private readonly IConfiguration _configuration;
    private readonly ILogger<IncidentMessageHandler> _logger;
    #endregion

    #region Constructor
    public IncidentMessageHandler(
        IKafkaProducer kafkaProducer,
        IConfiguration configuration,
        ILogger<IncidentMessageHandler> logger)
    {
        _kafkaProducer = kafkaProducer;
        _configuration = configuration;
        _logger = logger;
    }
    #endregion

    #region Handler
    public async Task HandleAsync(string key, IncidentEvent message, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Processing incident {IncidentId}", message.IncidentId);

            // 1. Publish to database persistence topic
            await PublishIncidentCreatedAsync(message, cancellationToken);

            // 2. Publish to audit history
            await PublishAuditHistoryAsync(message, cancellationToken);

            // 3. Broadcast to UI via SignalR
            await PublishToBroadcastAsync(message, cancellationToken);

            _logger.LogInformation("Incident {IncidentId} processed", message.IncidentId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling incident {IncidentId}", message.IncidentId);
            throw;
        }
    }
    #endregion

    #region Private Methods
    private async Task PublishIncidentCreatedAsync(IncidentEvent incident, CancellationToken cancellationToken)
    {
        var floor = incident.TelemetryData.TryGetValue("Floor", out var floorValue) 
            ? floorValue?.ToString() ?? "" 
            : "";
        var zone = incident.TelemetryData.TryGetValue("Zone", out var zoneValue) 
            ? zoneValue?.ToString() ?? "" 
            : "";

        var incidentCreatedEvent = new IncidentCreatedEvent
        {
            IncidentCode = incident.IncidentId,
            CorrelationId = incident.CorrelationId,
            BuildingId = incident.BuildingId,
            BuildingCode = incident.BuildingCode,
            IncidentType = incident.IncidentType,
            Severity = incident.Severity,
            Status = "Active",
            StartedUtc = incident.DetectedAtUtc,
            Summary = $"{incident.IncidentType} detected at {incident.BuildingCode}/{floor}/{zone} via {incident.RuleTriggered}",
            RootCause = incident.RuleTriggered,
            TriggeredByRule = true,
            EventCount = 1,
            ConfidenceScore = incident.ConfidenceScore,
            DeviceId = incident.DeviceId,
            DeviceName = incident.DeviceName,
            TelemetryData = incident.TelemetryData
        };

        var incidentCreatedTopic = _configuration["Kafka:IncidentCreatedTopic"] ?? "newdoor.incident.created";
        await _kafkaProducer.PublishAsync(incidentCreatedTopic, incident.IncidentId, incidentCreatedEvent, cancellationToken);

        _logger.LogInformation("Published to {Topic}", incidentCreatedTopic);
    }

    private async Task PublishAuditHistoryAsync(IncidentEvent incident, CancellationToken cancellationToken)
    {
        var auditEvent = new AuditHistoryEvent
        {
            CorrelationId = incident.CorrelationId,
            EventId = 0, // Will be populated by API consumer
            DeviceId = int.TryParse(incident.DeviceId, out var deviceId) ? deviceId : 0,
            EventType = incident.IncidentType,
            Severity = incident.Severity,
            ProcessingResult = "Incident Created",
            ProcessorName = "Action.Dispatcher.IncidentHandler",
            Remarks = $"Incident {incident.IncidentId} processed with confidence {incident.ConfidenceScore:P2} by rule {incident.RuleTriggered}",
            ProcessedUtc = DateTime.UtcNow,
            Metadata = new Dictionary<string, object>
            {
                ["IncidentId"] = incident.IncidentId,
                ["IncidentType"] = incident.IncidentType,
                ["BuildingCode"] = incident.BuildingCode,
                ["DeviceName"] = incident.DeviceName,
                ["ConfidenceScore"] = incident.ConfidenceScore,
                ["RuleTriggered"] = incident.RuleTriggered,
                ["DetectedAtUtc"] = incident.DetectedAtUtc
            }
        };

        var auditTopic = _configuration["Kafka:AuditHistoryTopic"] ?? "newdoor.audit.history";
        await _kafkaProducer.PublishAsync(auditTopic, incident.CorrelationId, auditEvent, cancellationToken);

        _logger.LogInformation("Published to {Topic}", auditTopic);
    }

    private async Task PublishToBroadcastAsync(IncidentEvent incident, CancellationToken cancellationToken)
    {
        var floor = incident.TelemetryData.TryGetValue("Floor", out var floorValue) 
            ? floorValue?.ToString() ?? "" 
            : "";
        var zone = incident.TelemetryData.TryGetValue("Zone", out var zoneValue) 
            ? zoneValue?.ToString() ?? "" 
            : "";

        var message = $"{incident.Severity} severity {incident.IncidentType} detected at {incident.BuildingCode}";
        if (!string.IsNullOrEmpty(floor) || !string.IsNullOrEmpty(zone))
        {
            message += $" - {floor}/{zone}";
        }

        var broadcastEvent = new UIBroadcastEvent
        {
            CorrelationId = incident.CorrelationId,
            EventType = "IncidentDetected",
            IncidentId = incident.IncidentId,
            DeviceId = incident.DeviceId,
            DeviceName = incident.DeviceName,
            BuildingId = incident.BuildingId,
            BuildingCode = incident.BuildingCode,
            Floor = floor,
            Zone = zone,
            Severity = incident.Severity,
            Message = message,
            Timestamp = DateTime.UtcNow,
            Data = new Dictionary<string, object>
            {
                ["IncidentType"] = incident.IncidentType,
                ["ConfidenceScore"] = incident.ConfidenceScore,
                ["RuleTriggered"] = incident.RuleTriggered,
                ["DetectedAtUtc"] = incident.DetectedAtUtc,
                ["TelemetryData"] = incident.TelemetryData
            }
        };

        var broadcastTopic = _configuration["Kafka:UIBroadcastTopic"] ?? "newdoor.ui.broadcast";
        await _kafkaProducer.PublishAsync(broadcastTopic, broadcastEvent.DeviceId, broadcastEvent, cancellationToken);

        _logger.LogInformation("Published to {Topic}", broadcastTopic);
    }
    #endregion
}
