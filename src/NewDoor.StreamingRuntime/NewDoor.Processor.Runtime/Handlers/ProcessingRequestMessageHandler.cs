using NewDoor.EventBus.Consumers;
using NewDoor.EventBus.Producers;
using NewDoor.Processor.Runtime.Models;
using NewDoor.Processor.Runtime.Services;

namespace NewDoor.Processor.Runtime.Handlers;

public class ProcessingRequestMessageHandler : IKafkaMessageHandler<ProcessorRequest>
{
    #region Fields
    private readonly IEventProcessorService _eventProcessorService;
    private readonly IKafkaProducer _kafkaProducer;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ProcessingRequestMessageHandler> _logger;
    #endregion

    #region Constructor
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
    #endregion

    #region Handler
    public async Task HandleAsync(string key, ProcessorRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _eventProcessorService.ProcessAsync(request, cancellationToken);
            response.AdditionalData["DeviceId"] = request.Event.DeviceId;
            response.AdditionalData["DeviceName"] = request.Event.DeviceName;
            response.AdditionalData["BuildingId"] = request.Event.BuildingId;
            response.AdditionalData["BuildingCode"] = request.Event.BuildingCode;
            response.AdditionalData["Floor"] = request.Event.Floor;
            response.AdditionalData["Zone"] = request.Event.Zone;
            response.AdditionalData["Temperature"] = request.Event.Temperature;
            response.AdditionalData["SmokeLevel"] = request.Event.SmokeLevel;
            response.AdditionalData["BatteryLevel"] = request.Event.BatteryLevel;

            var resultTopic = _configuration["Kafka:RuntimeResultTopic"] ?? "newdoor.runtime.result";
            await _kafkaProducer.PublishAsync(resultTopic, request.Event.DeviceId, response, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing request");
            throw;
        }
    }
    #endregion
}
