using NewDoor.EventBus.Consumers;
using NewDoor.EventBus.Producers;
using NewDoor.Listener.Models;

namespace NewDoor.Listener.Services;

public class TelemetryMessageHandler : IKafkaMessageHandler<EnrichedTelemetryEvent>
{
    #region Fields
    private readonly IEventEnrichmentService _enrichmentService;
    private readonly IIncidentDetectionService _incidentDetectionService;
    private readonly IKafkaProducer _kafkaProducer;
    private readonly IConfiguration _configuration;
    private readonly ILogger<TelemetryMessageHandler> _logger;
    #endregion

    #region Constructor
    public TelemetryMessageHandler(
        IEventEnrichmentService enrichmentService,
        IIncidentDetectionService incidentDetectionService,
        IKafkaProducer kafkaProducer,
        IConfiguration configuration,
        ILogger<TelemetryMessageHandler> logger)
    {
        _enrichmentService = enrichmentService;
        _incidentDetectionService = incidentDetectionService;
        _kafkaProducer = kafkaProducer;
        _configuration = configuration;
        _logger = logger;
    }
    #endregion

    #region Handler
    public async Task HandleAsync(string key, EnrichedTelemetryEvent message, CancellationToken cancellationToken)
    {
        try
        {
            var eventCategory = _enrichmentService.DetermineEventCategory(message.EventType);
            var pipeline = _enrichmentService.DeterminePipeline(eventCategory);
            var priority = _enrichmentService.DeterminePriority(
                message.EventType, 
                message.Payload.SmokeLevel, 
                message.Payload.Temperature);
            var enrichedEvent = new EnrichedWorkflowEvent
            {
                EventId = message.EventId,
                CorrelationId = message.CorrelationId,
                EventType = message.EventType,
                EventCategory = eventCategory,

                Device = new DeviceInfo
                {
                    DeviceId = message.DeviceId,
                    DeviceType = message.DeviceType,
                    DeviceName = message.DeviceName
                },

                Location = new LocationInfo
                {
                    BuildingId = message.BuildingId,
                    BuildingCode = message.BuildingCode,
                    Floor = message.Floor,
                    Zone = message.Zone
                },

                Telemetry = new TelemetryData
                {
                    Temperature = message.Payload.Temperature,
                    SmokeLevel = message.Payload.SmokeLevel,
                    BatteryLevel = message.Payload.BatteryLevel
                },

                Runtime = new RuntimeInfo
                {
                    Pipeline = pipeline,
                    Priority = priority
                },

                Metadata = new EventMetadata
                {
                    ReceivedBy = "NewDoor.Listener",
                    NormalizedUtc = DateTime.UtcNow
                }
            };

            var workflowTopic = _configuration["Kafka:WorkflowEventTopic"] ?? "newdoor.workflow.events";
            await _kafkaProducer.PublishAsync(workflowTopic, enrichedEvent.Device.DeviceId, enrichedEvent, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling telemetry");
            throw;
        }
    }
    #endregion
}
