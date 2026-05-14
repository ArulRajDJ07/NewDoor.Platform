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
        _kafkaConsumer = kafkaConsumer ?? throw new ArgumentNullException(nameof(kafkaConsumer));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _logger.LogInformation("ProcessingResultConsumerService created successfully");
    }
    #endregion

    #region BackgroundService
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("=== ProcessingResultConsumerService ExecuteAsync starting ===");

        try
        {
            var topic = _configuration["Kafka:RuntimeResultTopic"] ?? "newdoor.runtime.result";
            _logger.LogInformation("Starting ProcessingResultConsumer for topic: {Topic}", topic);
            _logger.LogInformation("Consumer instance: {ConsumerType}", _kafkaConsumer.GetType().Name);

            await _kafkaConsumer.StartConsumingAsync(topic, stoppingToken);
            _logger.LogInformation("Consumer started successfully, entering keep-alive loop");

            // Keep the task alive while the consumer is running
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }

            _logger.LogInformation("Consumer stopping - cancellation requested");
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("ProcessingResultConsumer service cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "ProcessingResultConsumer failed - Exception: {Message}", ex.Message);
            _logger.LogCritical("Stack trace: {StackTrace}", ex.StackTrace);
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
