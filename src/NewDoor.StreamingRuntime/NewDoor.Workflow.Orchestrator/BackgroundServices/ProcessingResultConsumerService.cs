using NewDoor.EventBus.Consumers;
using NewDoor.Workflow.Orchestrator.Models;
using Microsoft.Extensions.DependencyInjection;

namespace NewDoor.Workflow.Orchestrator.BackgroundServices;

public class ProcessingResultConsumerService : BackgroundService
{
    #region Fields
    private readonly IKafkaConsumer _kafkaConsumer;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ProcessingResultConsumerService> _logger;
    #endregion

    #region Constructor
    public ProcessingResultConsumerService(
        [FromKeyedServices("ResultConsumer")] IKafkaConsumer kafkaConsumer,
        IConfiguration configuration,
        ILogger<ProcessingResultConsumerService> logger)
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
            var topic = _configuration["Kafka:RuntimeResultTopic"] ?? "newdoor.runtime.result";
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
