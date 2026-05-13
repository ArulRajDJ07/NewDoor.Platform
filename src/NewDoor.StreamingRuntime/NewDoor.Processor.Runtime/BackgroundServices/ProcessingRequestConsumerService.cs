using NewDoor.EventBus.Consumers;
using NewDoor.Processor.Runtime.Models;

namespace NewDoor.Processor.Runtime.BackgroundServices;

/// <summary>
/// Consumes processing requests from Orchestrator
/// Processes events and publishes results back to result topic
/// </summary>
public class ProcessingRequestConsumerService : BackgroundService
{
    private readonly IKafkaConsumer _kafkaConsumer;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ProcessingRequestConsumerService> _logger;

    public ProcessingRequestConsumerService(
        IKafkaConsumer kafkaConsumer,
        IConfiguration configuration,
        ILogger<ProcessingRequestConsumerService> logger)
    {
        _kafkaConsumer = kafkaConsumer;
        _configuration = configuration;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            var topic = _configuration["Kafka:RuntimeProcessingTopic"] ?? "newdoor.runtime.processing";
            _logger.LogInformation("Starting Processing Request Consumer Service for topic: {Topic}", topic);

            await _kafkaConsumer.StartConsumingAsync(topic, stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Processing Request Consumer Service failed");
            throw;
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping Processing Request Consumer Service");
        await _kafkaConsumer.StopConsumingAsync();
        await base.StopAsync(cancellationToken);
    }
}
