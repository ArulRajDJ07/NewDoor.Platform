using NewDoor.EventBus.Consumers;
using NewDoor.EventBus.Producers;
using NewDoor.Workflow.Orchestrator.Models;

namespace NewDoor.Workflow.Orchestrator.Handlers;

/// <summary>
/// Handles incoming runtime telemetry events from Listener
/// Publishes to processing topic for Processor to consume
/// This is the first step in the orchestration workflow
/// </summary>
public class EventMessageHandler : IKafkaMessageHandler<RuntimeTelemetryEvent>
{
    private readonly IKafkaProducer _kafkaProducer;
    private readonly IConfiguration _configuration;
    private readonly ILogger<EventMessageHandler> _logger;

    public EventMessageHandler(
        IKafkaProducer kafkaProducer,
        IConfiguration configuration,
        ILogger<EventMessageHandler> logger)
    {
        _kafkaProducer = kafkaProducer;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task HandleAsync(string key, RuntimeTelemetryEvent message, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("=== Orchestrator: Event Received === EventId={EventId}, DeviceId={DeviceId}, CorrelationId={CorrelationId}", 
                message.EventId, message.DeviceId, message.CorrelationId);

            // Create processing request for Processor service
            var processorRequest = new ProcessorRequest
            {
                CorrelationId = message.CorrelationId,
                Event = message,
                RequestedAtUtc = DateTime.UtcNow
            };

            // Publish to processing topic for Processor to consume
            var processingTopic = _configuration["Kafka:RuntimeProcessingTopic"] ?? "newdoor.runtime.processing";
            await _kafkaProducer.PublishAsync(processingTopic, message.DeviceId, processorRequest, cancellationToken);

            _logger.LogInformation("→ Published to processing topic: {Topic}, RequestId={RequestId}, CorrelationId={CorrelationId}", 
                processingTopic, processorRequest.RequestId, message.CorrelationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling runtime event: EventId={EventId}, DeviceId={DeviceId}", 
                message.EventId, message.DeviceId);
            throw;
        }
    }
}
