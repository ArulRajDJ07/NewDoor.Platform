using NewDoor.EventBus.Consumers;
using NewDoor.Workflow.Orchestrator.Models;
using Microsoft.Extensions.DependencyInjection;

namespace NewDoor.Workflow.Orchestrator.BackgroundServices;

public class EventConsumerService : BackgroundService
{
    private readonly IKafkaConsumer _kafkaConsumer;
    private readonly IConfiguration _configuration;
    private readonly ILogger<EventConsumerService> _logger;

    public EventConsumerService(
        [FromKeyedServices("EventConsumer")] IKafkaConsumer kafkaConsumer,
        IConfiguration configuration,
        ILogger<EventConsumerService> logger)
    {
        _kafkaConsumer = kafkaConsumer;
        _configuration = configuration;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            var topic = _configuration["Kafka:RuntimeEventTopic"] ?? "newdoor.workflow.events";
            _logger.LogInformation("Starting Runtime Event Consumer Service for topic: {Topic}", topic);

            await _kafkaConsumer.StartConsumingAsync(topic, stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Runtime Event Consumer Service failed");
            throw;
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping Runtime Event Consumer Service");
        await _kafkaConsumer.StopConsumingAsync();
        await base.StopAsync(cancellationToken);
    }
}
