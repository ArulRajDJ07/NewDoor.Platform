using NewDoor.EventBus.Consumers;
using NewDoor.Listener.Models;

namespace NewDoor.Listener.BackgroundServices;

public class TelemetryConsumerService : BackgroundService
{
    #region Fields
    private readonly IKafkaConsumer _kafkaConsumer;
    private readonly IConfiguration _configuration;
    private readonly ILogger<TelemetryConsumerService> _logger;
    #endregion

    #region Constructor
    public TelemetryConsumerService(
        IKafkaConsumer kafkaConsumer,
        IConfiguration configuration,
        ILogger<TelemetryConsumerService> logger)
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
            var topic = _configuration["Kafka:TelemetryTopic"] ?? "newdoor.device.telemetry";
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
