using NewDoor.EventBus.Consumers;
using NewDoor.EventBus.Producers;
using NewDoor.Workflow.Orchestrator.Models;
using NewDoor.Workflow.Orchestrator.Services;

namespace NewDoor.Workflow.Orchestrator.Handlers;

/// <summary>
/// Handles processing results from Processor service
/// Continues the workflow after receiving analysis results
/// </summary>
public class ProcessingResultMessageHandler : IKafkaMessageHandler<ProcessorResponse>
{
    private readonly IActionDispatcherClient _actionDispatcherClient;
    private readonly IKafkaProducer _kafkaProducer;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ProcessingResultMessageHandler> _logger;

    public ProcessingResultMessageHandler(
        IActionDispatcherClient actionDispatcherClient,
        IKafkaProducer kafkaProducer,
        IConfiguration configuration,
        ILogger<ProcessingResultMessageHandler> logger)
    {
        _actionDispatcherClient = actionDispatcherClient;
        _kafkaProducer = kafkaProducer;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task HandleAsync(string key, ProcessorResponse processorResponse, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("=== Processing Result Received === ResponseId={ResponseId}, CorrelationId={CorrelationId}",
                processorResponse.ResponseId, processorResponse.CorrelationId);

            // Step 1: Publish audit history
            await PublishAuditHistoryAsync(processorResponse, cancellationToken);

            // Step 2: Dispatch actions if incident or alarm detected
            if (processorResponse.IsIncident || processorResponse.IsAlarm)
            {
                await DispatchActionsAsync(processorResponse, cancellationToken);

                // Step 3: Publish incident event
                if (processorResponse.IsIncident)
                {
                    await PublishIncidentAsync(processorResponse, cancellationToken);
                }

                // Step 4: Publish alarm event
                if (processorResponse.IsAlarm)
                {
                    await PublishAlarmAsync(processorResponse, cancellationToken);
                }
            }

            _logger.LogInformation("=== Processing Result Handling Completed === ResponseId={ResponseId}, IsIncident={IsIncident}, IsAlarm={IsAlarm}",
                processorResponse.ResponseId, processorResponse.IsIncident, processorResponse.IsAlarm);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling processing result: ResponseId={ResponseId}, CorrelationId={CorrelationId}",
                processorResponse.ResponseId, processorResponse.CorrelationId);
            throw;
        }
    }

    private async Task DispatchActionsAsync(ProcessorResponse processorResponse, CancellationToken cancellationToken)
    {
        _logger.LogInformation("→ Dispatching actions - IncidentType={IncidentType}, Severity={Severity}",
            processorResponse.IncidentType, processorResponse.Severity);

        var actionRequest = new ActionDispatchRequest
        {
            CorrelationId = processorResponse.CorrelationId,
            ActionType = DetermineActionType(processorResponse),
            Severity = processorResponse.Severity,
            IncidentType = processorResponse.IncidentType,
            DeviceId = processorResponse.AdditionalData.GetValueOrDefault("DeviceId", "").ToString() ?? "",
            BuildingId = int.TryParse(processorResponse.AdditionalData.GetValueOrDefault("BuildingId", 0).ToString(), out var buildingId) ? buildingId : 0,
            BuildingCode = processorResponse.AdditionalData.GetValueOrDefault("BuildingCode", "").ToString() ?? "",
            Floor = processorResponse.AdditionalData.GetValueOrDefault("Floor", "").ToString() ?? "",
            Zone = processorResponse.AdditionalData.GetValueOrDefault("Zone", "").ToString() ?? "",
            Context = new Dictionary<string, object>
            {
                { "Temperature", processorResponse.AdditionalData.GetValueOrDefault("Temperature", 0) },
                { "SmokeLevel", processorResponse.AdditionalData.GetValueOrDefault("SmokeLevel", 0) },
                { "RuleTriggered", processorResponse.RuleTriggered },
                { "ConfidenceScore", processorResponse.ConfidenceScore }
            }
        };

        var response = await _actionDispatcherClient.DispatchActionAsync(actionRequest, cancellationToken);

        _logger.LogInformation("← Actions dispatched - DispatchId={DispatchId}, Status={Status}",
            response.DispatchId, response.Status);
    }

    private string DetermineActionType(ProcessorResponse processorResponse)
    {
        if (processorResponse.IsAlarm && processorResponse.Severity == "Critical")
        {
            return "EmergencyAlert";
        }
        else if (processorResponse.IsAlarm)
        {
            return "StandardAlert";
        }
        else if (processorResponse.IsIncident)
        {
            return "IncidentNotification";
        }

        return "Notification";
    }

    private async Task PublishAuditHistoryAsync(ProcessorResponse processorResponse, CancellationToken cancellationToken)
    {
        _logger.LogInformation("→ Publishing Audit History - ResponseId={ResponseId}", processorResponse.ResponseId);

        var auditEvent = new AuditHistoryEvent
        {
            CorrelationId = processorResponse.CorrelationId,
            EventType = "ProcessingResult",
            DeviceId = processorResponse.AdditionalData.GetValueOrDefault("DeviceId", "").ToString() ?? "",
            EntityType = "ProcessorResponse",
            EntityId = processorResponse.ResponseId,
            Action = "Processed",
            Details = $"Processing result received: IsIncident={processorResponse.IsIncident}, Severity={processorResponse.Severity}",
            CreatedAtUtc = DateTime.UtcNow,
            Metadata = new Dictionary<string, object>
            {
                { "IncidentType", processorResponse.IncidentType },
                { "Severity", processorResponse.Severity },
                { "ConfidenceScore", processorResponse.ConfidenceScore },
                { "RuleTriggered", processorResponse.RuleTriggered }
            }
        };

        var auditTopic = _configuration["Kafka:AuditHistoryTopic"] ?? "newdoor.audit.history";
        await _kafkaProducer.PublishAsync(auditTopic, auditEvent.DeviceId, auditEvent, cancellationToken);

        _logger.LogInformation("← Audit History Published - AuditId={AuditId}, Topic={Topic}",
            auditEvent.AuditId, auditTopic);
    }

    private async Task PublishIncidentAsync(ProcessorResponse processorResponse, CancellationToken cancellationToken)
    {
        _logger.LogInformation("→ Publishing Incident Event - IncidentType={IncidentType}", processorResponse.IncidentType);

        var incidentEvent = new IncidentEvent
        {
            CorrelationId = processorResponse.CorrelationId,
            DeviceId = processorResponse.AdditionalData.GetValueOrDefault("DeviceId", "").ToString() ?? "",
            DeviceName = processorResponse.AdditionalData.GetValueOrDefault("DeviceName", "").ToString() ?? "",
            BuildingId = int.TryParse(processorResponse.AdditionalData.GetValueOrDefault("BuildingId", 0).ToString(), out var buildingId) ? buildingId : 0,
            BuildingCode = processorResponse.AdditionalData.GetValueOrDefault("BuildingCode", "").ToString() ?? "",
            IncidentType = processorResponse.IncidentType,
            Severity = processorResponse.Severity,
            ConfidenceScore = processorResponse.ConfidenceScore,
            RuleTriggered = processorResponse.RuleTriggered,
            DetectedAtUtc = DateTime.UtcNow,
            TelemetryData = new Dictionary<string, object>
            {
                { "Temperature", processorResponse.AdditionalData.GetValueOrDefault("Temperature", 0) },
                { "SmokeLevel", processorResponse.AdditionalData.GetValueOrDefault("SmokeLevel", 0) },
                { "BatteryLevel", processorResponse.AdditionalData.GetValueOrDefault("BatteryLevel", 0) },
                { "Floor", processorResponse.AdditionalData.GetValueOrDefault("Floor", "") },
                { "Zone", processorResponse.AdditionalData.GetValueOrDefault("Zone", "") }
            }
        };

        var incidentTopic = _configuration["Kafka:IncidentDetectedTopic"] ?? "newdoor.incident.detected";
        await _kafkaProducer.PublishAsync(incidentTopic, incidentEvent.DeviceId, incidentEvent, cancellationToken);

        _logger.LogInformation("← Incident Event Published - IncidentId={IncidentId}, Topic={Topic}",
            incidentEvent.IncidentId, incidentTopic);
    }

    private async Task PublishAlarmAsync(ProcessorResponse processorResponse, CancellationToken cancellationToken)
    {
        _logger.LogInformation("→ Publishing Alarm Event - AlarmType={AlarmType}", processorResponse.IncidentType);

        var buildingCode = processorResponse.AdditionalData.GetValueOrDefault("BuildingCode", "").ToString() ?? "";
        var floor = processorResponse.AdditionalData.GetValueOrDefault("Floor", "").ToString() ?? "";
        var zone = processorResponse.AdditionalData.GetValueOrDefault("Zone", "").ToString() ?? "";

        var alarmEvent = new AlarmEvent
        {
            CorrelationId = processorResponse.CorrelationId,
            DeviceId = processorResponse.AdditionalData.GetValueOrDefault("DeviceId", "").ToString() ?? "",
            DeviceName = processorResponse.AdditionalData.GetValueOrDefault("DeviceName", "").ToString() ?? "",
            BuildingId = int.TryParse(processorResponse.AdditionalData.GetValueOrDefault("BuildingId", 0).ToString(), out var buildingId) ? buildingId : 0,
            BuildingCode = buildingCode,
            Floor = floor,
            Zone = zone,
            AlarmType = processorResponse.IncidentType,
            Severity = processorResponse.Severity,
            Message = $"High severity {processorResponse.IncidentType} detected at {buildingCode} - {floor}/{zone}",
            TriggeredAtUtc = DateTime.UtcNow,
            Context = new Dictionary<string, object>
            {
                { "Temperature", processorResponse.AdditionalData.GetValueOrDefault("Temperature", 0) },
                { "SmokeLevel", processorResponse.AdditionalData.GetValueOrDefault("SmokeLevel", 0) },
                { "RuleTriggered", processorResponse.RuleTriggered },
                { "ConfidenceScore", processorResponse.ConfidenceScore }
            }
        };

        var alarmTopic = _configuration["Kafka:AlarmTriggeredTopic"] ?? "newdoor.alarm.triggered";
        await _kafkaProducer.PublishAsync(alarmTopic, alarmEvent.DeviceId, alarmEvent, cancellationToken);

        _logger.LogInformation("← Alarm Event Published - AlarmId={AlarmId}, Topic={Topic}",
            alarmEvent.AlarmId, alarmTopic);
    }
}
