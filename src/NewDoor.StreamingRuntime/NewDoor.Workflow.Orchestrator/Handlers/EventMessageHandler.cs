using NewDoor.EventBus.Consumers;
using NewDoor.EventBus.Producers;
using NewDoor.Workflow.Orchestrator.Models;

namespace NewDoor.Workflow.Orchestrator.Handlers;

public class EventMessageHandler : IKafkaMessageHandler<EnrichedWorkflowEvent>
{
    #region Fields
    private readonly IKafkaProducer _kafkaProducer;
    private readonly IConfiguration _configuration;
    private readonly ILogger<EventMessageHandler> _logger;
    #endregion

    #region Constructor
    public EventMessageHandler(
        IKafkaProducer kafkaProducer,
        IConfiguration configuration,
        ILogger<EventMessageHandler> logger)
    {
        _kafkaProducer = kafkaProducer;
        _configuration = configuration;
        _logger = logger;
    }
    #endregion

    #region Handler
    public async Task HandleAsync(string key, EnrichedWorkflowEvent message, CancellationToken cancellationToken)
    {
        try
        {
            // Map EnrichedWorkflowEvent to RuntimeTelemetryEvent
            var runtimeEvent = new RuntimeTelemetryEvent
            {
                EventId = message.EventId,
                CorrelationId = message.CorrelationId,
                DeviceId = message.Device.DeviceId,
                DeviceName = message.Device.DeviceName,
                DeviceType = message.Device.DeviceType, // ← THIS WAS MISSING!
                BuildingId = message.Location.BuildingId,
                BuildingCode = message.Location.BuildingCode,
                Floor = message.Location.Floor,
                Zone = message.Location.Zone,
                EventType = message.EventType,
                TimestampUtc = message.Metadata.NormalizedUtc,
                Temperature = message.Telemetry.Temperature,
                SmokeLevel = message.Telemetry.SmokeLevel,
                BatteryLevel = message.Telemetry.BatteryLevel,
                Source = message.Metadata.ReceivedBy
            };

            var processorRequest = new ProcessorRequest
            {
                CorrelationId = runtimeEvent.CorrelationId,
                Event = runtimeEvent,
                RequestedAtUtc = DateTime.UtcNow
            };

            _logger.LogDebug("Sending event to processor - DeviceId: {DeviceId}, DeviceType: {DeviceType}, SmokeLevel: {SmokeLevel}, Temperature: {Temperature}", 
                runtimeEvent.DeviceId, runtimeEvent.DeviceType, runtimeEvent.SmokeLevel, runtimeEvent.Temperature);

            var processingTopic = _configuration["Kafka:RuntimeProcessingTopic"] ?? "newdoor.runtime.processing";
            await _kafkaProducer.PublishAsync(processingTopic, runtimeEvent.DeviceId, processorRequest, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling event");
            throw;
        }
    }
    #endregion
}
