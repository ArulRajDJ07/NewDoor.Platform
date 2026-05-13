using NewDoor.EventBus.Consumers;
using NewDoor.Listener.Models;

namespace NewDoor.Listener.BackgroundServices;

public class TelemetryConsumerService : BackgroundService
{
    private readonly IKafkaConsumer _kafkaConsumer;
    private readonly IConfiguration _configuration;
    private readonly ILogger<TelemetryConsumerService> _logger;

    public TelemetryConsumerService(
        IKafkaConsumer kafkaConsumer,
        IConfiguration configuration,
        ILogger<TelemetryConsumerService> logger)
    {
        _kafkaConsumer = kafkaConsumer;
        _configuration = configuration;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            var topic = _configuration["Kafka:TelemetryTopic"] ?? "newdoor.device.telemetry";
            _logger.LogInformation("Starting Telemetry Consumer Service for topic: {Topic}", topic);

            await _kafkaConsumer.StartConsumingAsync(topic, stoppingToken);

            _logger.LogInformation("Telemetry Consumer Service started successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting Telemetry Consumer Service");
            throw;
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping Telemetry Consumer Service");
        await _kafkaConsumer.StopConsumingAsync();
        await base.StopAsync(cancellationToken);
    }
}
