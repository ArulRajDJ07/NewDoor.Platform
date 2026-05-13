using NewDoor.EventBus.Consumers;
using NewDoor.EventBus.Producers;
using NewDoor.Workflow.Orchestrator.Models;
using NewDoor.Workflow.Orchestrator.Services;

namespace NewDoor.Workflow.Orchestrator.Handlers;

public class RuntimeEventMessageHandler : IKafkaMessageHandler<RuntimeTelemetryEvent>
{
    private readonly IProcessorClient _processorClient;
    private readonly IKafkaProducer _kafkaProducer;
    private readonly IConfiguration _configuration;
    private readonly ILogger<RuntimeEventMessageHandler> _logger;

    public RuntimeEventMessageHandler(
        IProcessorClient processorClient,
        IKafkaProducer kafkaProducer,
        IConfiguration configuration,
        ILogger<RuntimeEventMessageHandler> logger)
    {
        _processorClient = processorClient;
        _kafkaProducer = kafkaProducer;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task HandleAsync(string key, RuntimeTelemetryEvent message, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Orchestrating workflow for: EventId={EventId}, DeviceId={DeviceId}", 
                message.EventId, message.DeviceId);

            var processorRequest = new ProcessorRequest
            {
                CorrelationId = message.CorrelationId,
                Event = message,
                RequestedAtUtc = DateTime.UtcNow
            };

            var processorResponse = await _processorClient.ProcessEventAsync(processorRequest, cancellationToken);

            await PublishAuditHistoryAsync(message, processorResponse, cancellationToken);

            if (processorResponse.IsIncident)
            {
                await PublishIncidentAsync(message, processorResponse, cancellationToken);
            }

            if (processorResponse.IsAlarm)
            {
                await PublishAlarmAsync(message, processorResponse, cancellationToken);
            }

            _logger.LogInformation("Workflow orchestration completed: EventId={EventId}, IsIncident={IsIncident}, IsAlarm={IsAlarm}", 
                message.EventId, processorResponse.IsIncident, processorResponse.IsAlarm);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error orchestrating workflow: EventId={EventId}, DeviceId={DeviceId}", 
                message.EventId, message.DeviceId);
            throw;
        }
    }

    private async Task PublishAuditHistoryAsync(RuntimeTelemetryEvent runtimeEvent, ProcessorResponse processorResponse, CancellationToken cancellationToken)
    {
        var auditEvent = new AuditHistoryEvent
        {
            CorrelationId = runtimeEvent.CorrelationId,
            EventType = runtimeEvent.EventType,
            DeviceId = runtimeEvent.DeviceId,
            EntityType = "TelemetryEvent",
            EntityId = runtimeEvent.EventId,
            Action = "Processed",
            Details = $"Event processed with result: IsIncident={processorResponse.IsIncident}, Severity={processorResponse.Severity}",
            CreatedAtUtc = DateTime.UtcNow,
            Metadata = new Dictionary<string, object>
            {
                { "Temperature", runtimeEvent.Temperature },
                { "SmokeLevel", runtimeEvent.SmokeLevel },
                { "BuildingId", runtimeEvent.BuildingId },
                { "ProcessorResponseId", processorResponse.ResponseId }
            }
        };

        var auditTopic = _configuration["Kafka:AuditHistoryTopic"] ?? "newdoor.audit.history";
        await _kafkaProducer.PublishAsync(auditTopic, auditEvent.DeviceId, auditEvent, cancellationToken);

        _logger.LogInformation("Published audit history: AuditId={AuditId}, Topic={Topic}", 
            auditEvent.AuditId, auditTopic);
    }

    private async Task PublishIncidentAsync(RuntimeTelemetryEvent runtimeEvent, ProcessorResponse processorResponse, CancellationToken cancellationToken)
    {
        var incidentEvent = new IncidentEvent
        {
            CorrelationId = runtimeEvent.CorrelationId,
            DeviceId = runtimeEvent.DeviceId,
            DeviceName = runtimeEvent.DeviceName,
            BuildingId = runtimeEvent.BuildingId,
            BuildingCode = runtimeEvent.BuildingCode,
            IncidentType = processorResponse.IncidentType,
            Severity = processorResponse.Severity,
            ConfidenceScore = processorResponse.ConfidenceScore,
            RuleTriggered = processorResponse.RuleTriggered,
            DetectedAtUtc = DateTime.UtcNow,
            TelemetryData = new Dictionary<string, object>
            {
                { "Temperature", runtimeEvent.Temperature },
                { "SmokeLevel", runtimeEvent.SmokeLevel },
                { "BatteryLevel", runtimeEvent.BatteryLevel },
                { "Floor", runtimeEvent.Floor },
                { "Zone", runtimeEvent.Zone }
            }
        };

        var incidentTopic = _configuration["Kafka:IncidentDetectedTopic"] ?? "newdoor.incident.detected";
        await _kafkaProducer.PublishAsync(incidentTopic, incidentEvent.DeviceId, incidentEvent, cancellationToken);

        _logger.LogInformation("Published incident: IncidentId={IncidentId}, Type={IncidentType}, Topic={Topic}", 
            incidentEvent.IncidentId, incidentEvent.IncidentType, incidentTopic);
    }

    private async Task PublishAlarmAsync(RuntimeTelemetryEvent runtimeEvent, ProcessorResponse processorResponse, CancellationToken cancellationToken)
    {
        var alarmEvent = new AlarmEvent
        {
            CorrelationId = runtimeEvent.CorrelationId,
            DeviceId = runtimeEvent.DeviceId,
            DeviceName = runtimeEvent.DeviceName,
            BuildingId = runtimeEvent.BuildingId,
            BuildingCode = runtimeEvent.BuildingCode,
            Floor = runtimeEvent.Floor,
            Zone = runtimeEvent.Zone,
            AlarmType = processorResponse.IncidentType,
            Severity = processorResponse.Severity,
            Message = $"High severity {processorResponse.IncidentType} detected at {runtimeEvent.BuildingCode} - {runtimeEvent.Floor}/{runtimeEvent.Zone}",
            TriggeredAtUtc = DateTime.UtcNow,
            Context = new Dictionary<string, object>
            {
                { "Temperature", runtimeEvent.Temperature },
                { "SmokeLevel", runtimeEvent.SmokeLevel },
                { "RuleTriggered", processorResponse.RuleTriggered },
                { "ConfidenceScore", processorResponse.ConfidenceScore }
            }
        };

        var alarmTopic = _configuration["Kafka:AlarmTriggeredTopic"] ?? "newdoor.alarm.triggered";
        await _kafkaProducer.PublishAsync(alarmTopic, alarmEvent.DeviceId, alarmEvent, cancellationToken);

        _logger.LogInformation("Published alarm: AlarmId={AlarmId}, Type={AlarmType}, Topic={Topic}", 
            alarmEvent.AlarmId, alarmEvent.AlarmType, alarmTopic);
    }
}
