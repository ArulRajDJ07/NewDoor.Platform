using NewDoor.EventBus.Consumers;
using NewDoor.API.Models;
using Microsoft.Extensions.DependencyInjection;

namespace NewDoor.API.BackgroundServices;

public class AuditHistoryConsumerService : BackgroundService
{
    #region Fields
    private readonly IKafkaConsumer _kafkaConsumer;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuditHistoryConsumerService> _logger;
    #endregion

    #region Constructor
    public AuditHistoryConsumerService(
        [FromKeyedServices("AuditHistory")] IKafkaConsumer kafkaConsumer,
        IConfiguration configuration,
        ILogger<AuditHistoryConsumerService> logger)
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
            var topic = _configuration["Kafka:AuditHistoryTopic"] ?? "newdoor.audit.history";
            _logger.LogInformation("Starting AuditHistory consumer: {Topic}", topic);
            await _kafkaConsumer.StartConsumingAsync(topic, stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "AuditHistory consumer failed");
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
