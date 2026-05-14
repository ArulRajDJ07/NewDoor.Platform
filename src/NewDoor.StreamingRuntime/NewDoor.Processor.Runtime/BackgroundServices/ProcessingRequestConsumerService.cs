using NewDoor.EventBus.Consumers;
using NewDoor.Processor.Runtime.Models;

namespace NewDoor.Processor.Runtime.BackgroundServices;

public class ProcessingRequestConsumerService : BackgroundService
{
    #region Fields
    private readonly IKafkaConsumer _kafkaConsumer;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ProcessingRequestConsumerService> _logger;
    #endregion

    #region Constructor
    public ProcessingRequestConsumerService(
        IKafkaConsumer kafkaConsumer,
        IConfiguration configuration,
        ILogger<ProcessingRequestConsumerService> logger)
    {
        _kafkaConsumer = kafkaConsumer;
        _configuration = configuration;
        _logger = logger;
    }
    #endregion

    #region BackgroundService
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            var topic = _configuration["Kafka:RuntimeProcessingTopic"] ?? "newdoor.runtime.processing";
            _logger.LogInformation("Starting consumer: {Topic}", topic);
            await _kafkaConsumer.StartConsumingAsync(topic, stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Consumer failed");
            throw;
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await _kafkaConsumer.StopConsumingAsync();
        await base.StopAsync(cancellationToken);
    }
    #endregion
}
