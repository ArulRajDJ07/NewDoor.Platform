using NewDoor.EventBus.Consumers;
using NewDoor.Action.Dispatcher.Models;

namespace NewDoor.Action.Dispatcher.BackgroundServices;

public class AlarmConsumerService : BackgroundService
{
    #region Fields
    private readonly IKafkaConsumer _kafkaConsumer;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AlarmConsumerService> _logger;
    #endregion

    #region Constructor
    public AlarmConsumerService(
        IKafkaConsumer kafkaConsumer,
        IConfiguration configuration,
        ILogger<AlarmConsumerService> logger)
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
            var topic = _configuration["Kafka:AlarmTriggeredTopic"] ?? "newdoor.alarm.triggered";
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
