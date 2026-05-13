using NewDoor.EventBus.Consumers;
using NewDoor.EventBus.Producers;
using NewDoor.Listener.Models;

namespace NewDoor.Listener.Services;

public class TelemetryMessageHandler : IKafkaMessageHandler<EnrichedTelemetryEvent>
{
    private readonly IIncidentDetectionService _incidentDetectionService;
    private readonly IKafkaProducer _kafkaProducer;
    private readonly IConfiguration _configuration;
    private readonly ILogger<TelemetryMessageHandler> _logger;

    public TelemetryMessageHandler(
        IIncidentDetectionService incidentDetectionService,
        IKafkaProducer kafkaProducer,
        IConfiguration configuration,
        ILogger<TelemetryMessageHandler> logger)
    {
        _incidentDetectionService = incidentDetectionService;
        _kafkaProducer = kafkaProducer;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task HandleAsync(string key, EnrichedTelemetryEvent message, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Processing telemetry: EventId={EventId}, DeviceId={DeviceId}, EventType={EventType}", 
                message.EventId, message.DeviceId, message.EventType);

            var runtimeEvent = new RuntimeTelemetryEvent
            {
                EventId = message.EventId,
                CorrelationId = message.CorrelationId,
                DeviceId = message.DeviceId,
                DeviceName = message.DeviceName,
                DeviceType = message.DeviceType,
                BuildingId = message.BuildingId,
                BuildingCode = message.BuildingCode,
                Floor = message.Floor,
                Zone = message.Zone,
                EventType = message.EventType,
                TimestampUtc = message.TimestampUtc,
                Temperature = message.Payload.Temperature,
                SmokeLevel = message.Payload.SmokeLevel,
                BatteryLevel = message.Payload.BatteryLevel,
                SignalStrength = message.Payload.SignalStrength,
                Status = message.Payload.Status,
                Source = "NewDoor.Listener"
            };

            var runtimeTopic = _configuration["Kafka:RuntimeEventTopic"] ?? "newdoor.runtime.event";
            await _kafkaProducer.PublishAsync(runtimeTopic, runtimeEvent.DeviceId, runtimeEvent, cancellationToken);

            _logger.LogInformation("Published runtime event to Workflow Orchestrator: EventId={EventId}, Topic={Topic}", 
                runtimeEvent.EventId, runtimeTopic);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling telemetry message: EventId={EventId}, DeviceId={DeviceId}", 
                message.EventId, message.DeviceId);
            throw;
        }
    }
}
