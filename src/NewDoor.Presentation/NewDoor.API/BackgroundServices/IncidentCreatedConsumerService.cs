using NewDoor.EventBus.Consumers;
using NewDoor.API.Models;
using Microsoft.Extensions.DependencyInjection;

namespace NewDoor.API.BackgroundServices;

public class IncidentCreatedConsumerService : BackgroundService
{
    #region Fields
    private readonly IKafkaConsumer _kafkaConsumer;
    private readonly IConfiguration _configuration;
    private readonly ILogger<IncidentCreatedConsumerService> _logger;
    #endregion

    #region Constructor
    public IncidentCreatedConsumerService(
        [FromKeyedServices("IncidentCreated")] IKafkaConsumer kafkaConsumer,
        IConfiguration configuration,
        ILogger<IncidentCreatedConsumerService> logger)
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
            var topic = _configuration["Kafka:IncidentCreatedTopic"] ?? "newdoor.incident.created";
            _logger.LogInformation("Starting IncidentCreated consumer: {Topic}", topic);
            await _kafkaConsumer.StartConsumingAsync(topic, stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "IncidentCreated consumer failed");
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
