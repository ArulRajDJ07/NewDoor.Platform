using NewDoor.EventBus.Consumers;
using NewDoor.EventBus.Producers;
using NewDoor.Workflow.Orchestrator.Models;
using NewDoor.Workflow.Orchestrator.Services;

namespace NewDoor.Workflow.Orchestrator.Handlers;

public class ProcessingResultMessageHandler : IKafkaMessageHandler<ProcessorResponse>
{
    #region Fields
    private readonly IActionDispatcherClient _actionDispatcherClient;
    private readonly IKafkaProducer _kafkaProducer;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ProcessingResultMessageHandler> _logger;
    #endregion

    #region Constructor
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
    #endregion

    #region Handler
    public async Task HandleAsync(string key, ProcessorResponse processorResponse, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Processing result: {EventType}", processorResponse.EventType);
            await ClassifyAndRouteAsync(processorResponse, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling result");
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
        await DispatchActionsAsync(processorResponse, cancellationToken);
    }

    private async Task HandleAlarmEventAsync(ProcessorResponse processorResponse, CancellationToken cancellationToken)
    {
        await PublishAlarmAsync(processorResponse, cancellationToken);
        await DispatchActionsAsync(processorResponse, cancellationToken);
    }

    private async Task HandleNotificationEventAsync(ProcessorResponse processorResponse, CancellationToken cancellationToken)
    {
        await DispatchActionsAsync(processorResponse, cancellationToken);
    }

    private async Task HandleEscalationEventAsync(ProcessorResponse processorResponse, CancellationToken cancellationToken)
    {
        await PublishIncidentAsync(processorResponse, cancellationToken);
        await PublishAlarmAsync(processorResponse, cancellationToken);
        await DispatchActionsAsync(processorResponse, cancellationToken);
    }

    private async Task HandleWorkflowEventAsync(ProcessorResponse processorResponse, CancellationToken cancellationToken)
    {
        await DispatchActionsAsync(processorResponse, cancellationToken);
    }

    private async Task HandleIncidentOrAlarmAsync(ProcessorResponse processorResponse, CancellationToken cancellationToken)
    {
        await DispatchActionsAsync(processorResponse, cancellationToken);

        if (processorResponse.IsIncident)
            await PublishIncidentAsync(processorResponse, cancellationToken);

        if (processorResponse.IsAlarm)
            await PublishAlarmAsync(processorResponse, cancellationToken);
    }
    #endregion

    #region Action Dispatch
    private async Task DispatchActionsAsync(ProcessorResponse processorResponse, CancellationToken cancellationToken)
    {
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

        await _actionDispatcherClient.DispatchActionAsync(actionRequest, cancellationToken);
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
    #endregion

    #region Publishing
    private async Task PublishIncidentAsync(ProcessorResponse processorResponse, CancellationToken cancellationToken)
    {

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
    }

    private async Task PublishAlarmAsync(ProcessorResponse processorResponse, CancellationToken cancellationToken)
    {

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
    }
    #endregion
}
