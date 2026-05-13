using NewDoor.EventBus.Consumers;
using NewDoor.Workflow.Orchestrator.Models;
using Microsoft.Extensions.DependencyInjection;

namespace NewDoor.Workflow.Orchestrator.BackgroundServices;

/// <summary>
/// Consumes processing results from Processor service
/// This creates the back-and-forth communication pattern:
/// Orchestrator → Processor → Orchestrator
/// </summary>
public class ProcessingResultConsumerService : BackgroundService
{
    private readonly IKafkaConsumer _kafkaConsumer;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ProcessingResultConsumerService> _logger;

    public ProcessingResultConsumerService(
        [FromKeyedServices("ResultConsumer")] IKafkaConsumer kafkaConsumer,
        IConfiguration configuration,
        ILogger<ProcessingResultConsumerService> logger)
    {
        _kafkaConsumer = kafkaConsumer;
        _configuration = configuration;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            var topic = _configuration["Kafka:RuntimeResultTopic"] ?? "newdoor.runtime.result";
            _logger.LogInformation("Starting Processing Result Consumer Service for topic: {Topic}", topic);

            await _kafkaConsumer.StartConsumingAsync(topic, stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Processing Result Consumer Service failed");
            throw;
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping Processing Result Consumer Service");
        await _kafkaConsumer.StopConsumingAsync();
        await base.StopAsync(cancellationToken);
    }
}
