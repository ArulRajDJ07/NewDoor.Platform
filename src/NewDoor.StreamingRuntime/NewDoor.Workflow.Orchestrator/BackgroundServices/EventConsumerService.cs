using NewDoor.EventBus.Consumers;
using NewDoor.Workflow.Orchestrator.Models;
using Microsoft.Extensions.DependencyInjection;

namespace NewDoor.Workflow.Orchestrator.BackgroundServices;

public class EventConsumerService : BackgroundService
{
    #region Fields
    private readonly IKafkaConsumer _kafkaConsumer;
    private readonly IConfiguration _configuration;
    private readonly ILogger<EventConsumerService> _logger;
    #endregion

    #region Constructor
    public EventConsumerService(
        [FromKeyedServices("EventConsumer")] IKafkaConsumer kafkaConsumer,
        IConfiguration configuration,
        ILogger<EventConsumerService> logger)
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
            var topic = _configuration["Kafka:RuntimeEventTopic"] ?? "newdoor.workflow.events";
            _logger.LogInformation("Starting consumer: {Topic}", topic);

            await _kafkaConsumer.StartConsumingAsync(topic, stoppingToken);

            // Keep the task alive while the consumer is running
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Consumer service cancelled");
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
