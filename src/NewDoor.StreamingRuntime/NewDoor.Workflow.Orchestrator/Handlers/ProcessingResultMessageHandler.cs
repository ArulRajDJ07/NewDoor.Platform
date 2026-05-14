using NewDoor.EventBus.Consumers;
using NewDoor.EventBus.Producers;
using NewDoor.Workflow.Orchestrator.Models;

namespace NewDoor.Workflow.Orchestrator.Handlers;

public class ProcessingResultMessageHandler : IKafkaMessageHandler<ProcessorResponse>
{
    #region Fields
    private readonly IKafkaProducer _kafkaProducer;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ProcessingResultMessageHandler> _logger;
    #endregion

    #region Constructor
    public ProcessingResultMessageHandler(
        IKafkaProducer kafkaProducer,
        IConfiguration configuration,
        ILogger<ProcessingResultMessageHandler> logger)
    {
        _kafkaProducer = kafkaProducer;
        _configuration = configuration;
        _logger = logger;
    }
    #endregion

    #region Handler
    public async Task HandleAsync(string key, ProcessorResponse processorResponse, CancellationToken cancellationToken)
    {
        try
        {
            await ClassifyAndRouteAsync(processorResponse, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling ProcessorResponse: {CorrelationId}", processorResponse.CorrelationId);
            throw;
        }
    }
    #endregion

    #region Routing
    private async Task ClassifyAndRouteAsync(ProcessorResponse processorResponse, CancellationToken cancellationToken)
    {
        switch (processorResponse.EventType.ToLowerInvariant())
        {
            case "incident":
                await HandleIncidentEventAsync(processorResponse, cancellationToken);
                break;
            case "alarm":
                await HandleAlarmEventAsync(processorResponse, cancellationToken);
                break;
            case "audit":
                break;
            case "notification":
                await HandleNotificationEventAsync(processorResponse, cancellationToken);
                break;
            case "escalation":
                await HandleEscalationEventAsync(processorResponse, cancellationToken);
                break;
            case "workflow":
                await HandleWorkflowEventAsync(processorResponse, cancellationToken);
                break;
            default:
                if (processorResponse.IsIncident || processorResponse.IsAlarm)
                {
                    await HandleIncidentOrAlarmAsync(processorResponse, cancellationToken);
                }
                break;
        }
    }

    private async Task HandleIncidentEventAsync(ProcessorResponse processorResponse, CancellationToken cancellationToken)
    {
        await PublishIncidentAsync(processorResponse, cancellationToken);
        await PublishAuditHistoryAsync(processorResponse, cancellationToken);
    }

    private async Task HandleAlarmEventAsync(ProcessorResponse processorResponse, CancellationToken cancellationToken)
    {
        await PublishAlarmAsync(processorResponse, cancellationToken);
        await PublishAuditHistoryAsync(processorResponse, cancellationToken);
    }

    private async Task HandleNotificationEventAsync(ProcessorResponse processorResponse, CancellationToken cancellationToken)
    {
        await PublishAuditHistoryAsync(processorResponse, cancellationToken);
    }

    private async Task HandleEscalationEventAsync(ProcessorResponse processorResponse, CancellationToken cancellationToken)
    {
        await PublishIncidentAsync(processorResponse, cancellationToken);
        await PublishAlarmAsync(processorResponse, cancellationToken);
        await PublishAuditHistoryAsync(processorResponse, cancellationToken);
    }

    private async Task HandleWorkflowEventAsync(ProcessorResponse processorResponse, CancellationToken cancellationToken)
    {
        await PublishAuditHistoryAsync(processorResponse, cancellationToken);
    }

    private async Task HandleIncidentOrAlarmAsync(ProcessorResponse processorResponse, CancellationToken cancellationToken)
    {
        if (processorResponse.IsIncident)
            await PublishIncidentAsync(processorResponse, cancellationToken);

        if (processorResponse.IsAlarm)
            await PublishAlarmAsync(processorResponse, cancellationToken);

        await PublishAuditHistoryAsync(processorResponse, cancellationToken);
    }
    #endregion

    #region Publishing
    private async Task PublishIncidentAsync(ProcessorResponse processorResponse, CancellationToken cancellationToken)
    {
        try
        {
            var incidentEvent = new IncidentEvent
            {
                CorrelationId = processorResponse.CorrelationId,
                DeviceId = GetStringValue(processorResponse.AdditionalData, "DeviceId"),
                DeviceName = GetStringValue(processorResponse.AdditionalData, "DeviceName"),
                BuildingId = GetIntValue(processorResponse.AdditionalData, "BuildingId"),
                BuildingCode = GetStringValue(processorResponse.AdditionalData, "BuildingCode"),
                IncidentType = processorResponse.IncidentType,
                Severity = processorResponse.Severity,
                ConfidenceScore = processorResponse.ConfidenceScore,
                RuleTriggered = processorResponse.RuleTriggered ?? "",
                DetectedAtUtc = DateTime.UtcNow,
                TelemetryData = new Dictionary<string, object>
                {
                    { "Temperature", GetDoubleValue(processorResponse.AdditionalData, "Temperature") },
                    { "SmokeLevel", GetDoubleValue(processorResponse.AdditionalData, "SmokeLevel") },
                    { "BatteryLevel", GetDoubleValue(processorResponse.AdditionalData, "BatteryLevel") },
                    { "Floor", GetStringValue(processorResponse.AdditionalData, "Floor") },
                    { "Zone", GetStringValue(processorResponse.AdditionalData, "Zone") }
                }
            };

            var incidentTopic = _configuration["Kafka:IncidentDetectedTopic"] ?? "newdoor.incident.detected";
            await _kafkaProducer.PublishAsync(incidentTopic, incidentEvent.DeviceId, incidentEvent, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing incident for CorrelationId: {CorrelationId}", processorResponse.CorrelationId);
            throw;
        }
    }

    private async Task PublishAlarmAsync(ProcessorResponse processorResponse, CancellationToken cancellationToken)
    {
        try
        {
            var buildingCode = GetStringValue(processorResponse.AdditionalData, "BuildingCode");
            var floor = GetStringValue(processorResponse.AdditionalData, "Floor");
            var zone = GetStringValue(processorResponse.AdditionalData, "Zone");

            // TODO: For production, map RuleTriggered name to RuleId via API/cache lookup
            // For hackathon: use fallback RuleId=1 (create a default "System Rule" in database)
            var ruleId = MapRuleNameToId(processorResponse.RuleTriggered);

            var alarmEvent = new AlarmEvent
            {
                CorrelationId = processorResponse.CorrelationId,
                DeviceId = GetStringValue(processorResponse.AdditionalData, "DeviceId"),
                DeviceName = GetStringValue(processorResponse.AdditionalData, "DeviceName"),
                BuildingId = GetIntValue(processorResponse.AdditionalData, "BuildingId"),
                BuildingCode = buildingCode,
                Floor = floor,
                Zone = zone,
                AlarmType = processorResponse.IncidentType,
                Severity = processorResponse.Severity,
                Message = $"High severity {processorResponse.IncidentType} detected at {buildingCode} - {floor}/{zone}",
                TriggeredAtUtc = DateTime.UtcNow,
                Context = new Dictionary<string, object>
                {
                    { "Temperature", GetDoubleValue(processorResponse.AdditionalData, "Temperature") },
                    { "SmokeLevel", GetDoubleValue(processorResponse.AdditionalData, "SmokeLevel") },
                    { "RuleId", ruleId },
                    { "RuleName", processorResponse.RuleTriggered ?? "Unknown" },
                    { "RuleTriggered", processorResponse.RuleTriggered ?? "" },
                    { "ConfidenceScore", processorResponse.ConfidenceScore }
                }
            };

            var alarmTopic = _configuration["Kafka:AlarmTriggeredTopic"] ?? "newdoor.alarm.triggered";
            await _kafkaProducer.PublishAsync(alarmTopic, alarmEvent.DeviceId, alarmEvent, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing alarm for CorrelationId: {CorrelationId}", processorResponse.CorrelationId);
            throw;
        }
    }

    private int MapRuleNameToId(string? ruleName)
    {
        // Hackathon fallback: Default RuleId = 1
        // TODO: Implement proper lookup via API or cache
        // Example: Fire Detection -> RuleId=1, Smoke Alarm -> RuleId=2, etc.
        if (string.IsNullOrEmpty(ruleName))
            return 1;

        // Simple mapping for demo purposes
        return ruleName.ToLowerInvariant() switch
        {
            "fire detection" => 1,
            "smoke alarm" => 2,
            "temperature threshold" => 3,
            _ => 1 // Default fallback
        };
    }

    private async Task PublishAuditHistoryAsync(ProcessorResponse processorResponse, CancellationToken cancellationToken)
    {
        try
        {
            var auditEvent = new AuditHistoryEvent
            {
                CorrelationId = processorResponse.CorrelationId,
                EventType = processorResponse.EventType,
                DeviceId = GetStringValue(processorResponse.AdditionalData, "DeviceId"),
                EntityType = processorResponse.IsIncident ? "Incident" : (processorResponse.IsAlarm ? "Alarm" : "Event"),
                EntityId = processorResponse.ResponseId,
                Action = "ProcessorResponseReceived",
                Details = $"{processorResponse.EventType} - {processorResponse.IncidentType} - Severity: {processorResponse.Severity}",
                CreatedAtUtc = DateTime.UtcNow,
                Metadata = new Dictionary<string, object>
                {
                    { "BuildingId", GetIntValue(processorResponse.AdditionalData, "BuildingId") },
                    { "BuildingCode", GetStringValue(processorResponse.AdditionalData, "BuildingCode") },
                    { "Floor", GetStringValue(processorResponse.AdditionalData, "Floor") },
                    { "Zone", GetStringValue(processorResponse.AdditionalData, "Zone") },
                    { "IsIncident", processorResponse.IsIncident },
                    { "IsAlarm", processorResponse.IsAlarm },
                    { "Severity", processorResponse.Severity },
                    { "IncidentType", processorResponse.IncidentType },
                    { "RuleTriggered", processorResponse.RuleTriggered ?? "" },
                    { "ConfidenceScore", processorResponse.ConfidenceScore },
                    { "Temperature", GetDoubleValue(processorResponse.AdditionalData, "Temperature") },
                    { "SmokeLevel", GetDoubleValue(processorResponse.AdditionalData, "SmokeLevel") }
                }
            };

            var auditTopic = _configuration["Kafka:AuditHistoryTopic"] ?? "newdoor.audit.history";
            _logger.LogInformation("Publishing audit history to topic: {Topic}, CorrelationId: {CorrelationId}", auditTopic, auditEvent.CorrelationId);
            await _kafkaProducer.PublishAsync(auditTopic, auditEvent.CorrelationId, auditEvent, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing audit history for CorrelationId: {CorrelationId}", processorResponse.CorrelationId);
            throw;
        }
    }

    #region Helper Methods
    private string GetStringValue(Dictionary<string, object>? dict, string key)
    {
        if (dict == null || !dict.ContainsKey(key))
            return "";

        var value = dict[key];
        return value?.ToString() ?? "";
    }

    private int GetIntValue(Dictionary<string, object>? dict, string key)
    {
        if (dict == null || !dict.ContainsKey(key))
            return 0;

        var value = dict[key];
        if (value == null)
            return 0;

        if (value is int intValue)
            return intValue;

        return int.TryParse(value.ToString(), out var result) ? result : 0;
    }

    private double GetDoubleValue(Dictionary<string, object>? dict, string key)
    {
        if (dict == null || !dict.ContainsKey(key))
            return 0.0;

        var value = dict[key];
        if (value == null)
            return 0.0;

        if (value is double doubleValue)
            return doubleValue;

        if (value is int intValue)
            return (double)intValue;

        return double.TryParse(value.ToString(), out var result) ? result : 0.0;
    }
    #endregion
    #endregion
}
