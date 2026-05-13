using NewDoor.EventBus.Consumers;
using NewDoor.EventBus.Producers;
using NewDoor.Processor.Runtime.Models;
using NewDoor.Processor.Runtime.Services;

namespace NewDoor.Processor.Runtime.Handlers;

/// <summary>
/// Handles processing requests from Orchestrator
/// Analyzes events and publishes results back to result topic
/// </summary>
public class ProcessingRequestMessageHandler : IKafkaMessageHandler<ProcessorRequest>
{
    private readonly IEventProcessorService _eventProcessorService;
    private readonly IKafkaProducer _kafkaProducer;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ProcessingRequestMessageHandler> _logger;

    public ProcessingRequestMessageHandler(
        IEventProcessorService eventProcessorService,
        IKafkaProducer kafkaProducer,
        IConfiguration configuration,
        ILogger<ProcessingRequestMessageHandler> logger)
    {
        _eventProcessorService = eventProcessorService;
        _kafkaProducer = kafkaProducer;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task HandleAsync(string key, ProcessorRequest request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("=== Processing Request Received === RequestId={RequestId}, CorrelationId={CorrelationId}, DeviceId={DeviceId}",
                request.RequestId, request.CorrelationId, request.Event.DeviceId);

            // Process the event using the event processor service
            var response = await _eventProcessorService.ProcessAsync(request, cancellationToken);

            // Add original event data to response for context
            response.AdditionalData["DeviceId"] = request.Event.DeviceId;
            response.AdditionalData["DeviceName"] = request.Event.DeviceName;
            response.AdditionalData["BuildingId"] = request.Event.BuildingId;
            response.AdditionalData["BuildingCode"] = request.Event.BuildingCode;
            response.AdditionalData["Floor"] = request.Event.Floor;
            response.AdditionalData["Zone"] = request.Event.Zone;
            response.AdditionalData["Temperature"] = request.Event.Temperature;
            response.AdditionalData["SmokeLevel"] = request.Event.SmokeLevel;
            response.AdditionalData["BatteryLevel"] = request.Event.BatteryLevel;

            // Publish result back to result topic for Orchestrator to consume
            var resultTopic = _configuration["Kafka:RuntimeResultTopic"] ?? "newdoor.runtime.result";
            await _kafkaProducer.PublishAsync(resultTopic, request.Event.DeviceId, response, cancellationToken);

            _logger.LogInformation("← Published processing result: Topic={Topic}, ResponseId={ResponseId}, IsIncident={IsIncident}, IsAlarm={IsAlarm}",
                resultTopic, response.ResponseId, response.IsIncident, response.IsAlarm);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing request: RequestId={RequestId}, CorrelationId={CorrelationId}",
                request.RequestId, request.CorrelationId);
            throw;
        }
    }
}
