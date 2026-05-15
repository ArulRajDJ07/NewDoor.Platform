using NewDoor.EventBus.Consumers;
using NewDoor.API.Models;
using Microsoft.Extensions.DependencyInjection;

namespace NewDoor.API.BackgroundServices;

public class UIBroadcastConsumerService : BackgroundService
{
    #region Fields
    private readonly IKafkaConsumer _kafkaConsumer;
    private readonly IConfiguration _configuration;
    private readonly ILogger<UIBroadcastConsumerService> _logger;
    #endregion

    #region Constructor
    public UIBroadcastConsumerService(
        [FromKeyedServices("UIBroadcast")] IKafkaConsumer kafkaConsumer,
        IConfiguration configuration,
        ILogger<UIBroadcastConsumerService> logger)
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
            var topic = _configuration["Kafka:UIBroadcastTopic"] ?? "newdoor.ui.broadcast";
            await _kafkaConsumer.StartConsumingAsync(topic, stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "UIBroadcast consumer failed");
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
