using NewDoor.EventBus.Consumers;
using NewDoor.Action.Dispatcher.Models;
using Microsoft.Extensions.DependencyInjection;

namespace NewDoor.Action.Dispatcher.BackgroundServices;

public class IncidentConsumerService : BackgroundService
{
    #region Fields
    private readonly IKafkaConsumer _kafkaConsumer;
    private readonly IConfiguration _configuration;
    private readonly ILogger<IncidentConsumerService> _logger;
    #endregion

    #region Constructor
    public IncidentConsumerService(
        [FromKeyedServices("IncidentConsumer")] IKafkaConsumer kafkaConsumer,
        IConfiguration configuration,
        ILogger<IncidentConsumerService> logger)
    {
        _kafkaConsumer = kafkaConsumer ?? throw new ArgumentNullException(nameof(kafkaConsumer));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    #endregion

    #region BackgroundService
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            var topic = _configuration["Kafka:IncidentDetectedTopic"] ?? "newdoor.incident.detected";
            _logger.LogInformation("Incident consumer started: {Topic}", topic);

            await _kafkaConsumer.StartConsumingAsync(topic, stoppingToken);

            // Keep the task alive while the consumer is running
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            // Silent cancellation
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Incident consumer failed");
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
