using NewDoor.EventBus.Consumers;
using NewDoor.EventBus.Producers;
using NewDoor.Listener.Models;

namespace NewDoor.Listener.Services;

/// <summary>
/// Handles incoming telemetry messages with:
/// - Metadata Enrichment
/// - Event Transformation
/// - Event Normalization
/// </summary>
public class TelemetryMessageHandler : IKafkaMessageHandler<EnrichedTelemetryEvent>
{
    private readonly IEventEnrichmentService _enrichmentService;
    private readonly IIncidentDetectionService _incidentDetectionService;
    private readonly IKafkaProducer _kafkaProducer;
    private readonly IConfiguration _configuration;
    private readonly ILogger<TelemetryMessageHandler> _logger;

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

    public async Task HandleAsync(string key, EnrichedTelemetryEvent message, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Processing telemetry: EventId={EventId}, DeviceId={DeviceId}, EventType={EventType}", 
                message.EventId, message.DeviceId, message.EventType);

            // Step 1: Event Categorization
            var eventCategory = _enrichmentService.DetermineEventCategory(message.EventType);

            // Step 2: Metadata Enrichment - Determine pipeline routing and priority
            var pipeline = _enrichmentService.DeterminePipeline(eventCategory);
            var priority = _enrichmentService.DeterminePriority(
                message.EventType, 
                message.Payload.SmokeLevel, 
                message.Payload.Temperature);

            // Step 3: Event Transformation & Normalization - Create enriched workflow event
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

            // Step 4: Publish to workflow events topic
            var workflowTopic = _configuration["Kafka:WorkflowEventTopic"] ?? "newdoor.workflow.events";
            await _kafkaProducer.PublishAsync(workflowTopic, enrichedEvent.Device.DeviceId, enrichedEvent, cancellationToken);

            _logger.LogInformation(
                "Published enriched event: EventId={EventId}, Category={Category}, Pipeline={Pipeline}, Priority={Priority}, Topic={Topic}", 
                enrichedEvent.EventId, enrichedEvent.EventCategory, enrichedEvent.Runtime.Pipeline, 
                enrichedEvent.Runtime.Priority, workflowTopic);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "Error handling telemetry message: EventId={EventId}, DeviceId={DeviceId}", 
                message.EventId, message.DeviceId);
            throw;
        }
    }
}
