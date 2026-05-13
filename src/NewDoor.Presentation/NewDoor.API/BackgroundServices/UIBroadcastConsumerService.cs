using NewDoor.EventBus.Consumers;
using NewDoor.API.Models;

namespace NewDoor.API.BackgroundServices;

public class UIBroadcastConsumerService : BackgroundService
{
    private readonly IKafkaConsumer _kafkaConsumer;
    private readonly IConfiguration _configuration;
    private readonly ILogger<UIBroadcastConsumerService> _logger;

    public UIBroadcastConsumerService(
        IKafkaConsumer kafkaConsumer,
        IConfiguration configuration,
        ILogger<UIBroadcastConsumerService> logger)
    {
        _kafkaConsumer = kafkaConsumer;
        _configuration = configuration;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            var topic = _configuration["Kafka:UIBroadcastTopic"] ?? "newdoor.ui.broadcast";
            _logger.LogInformation("Starting UI Broadcast Consumer Service for topic: {Topic}", topic);

            await _kafkaConsumer.StartConsumingAsync(topic, stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "UI Broadcast Consumer Service failed");
            throw;
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping UI Broadcast Consumer Service");
        await _kafkaConsumer.StopConsumingAsync();
        await base.StopAsync(cancellationToken);
    }
}
