using NewDoor.DeviceSimulator.Models;
using NewDoor.EventBus.Producers;
using Microsoft.Extensions.Logging;

namespace NewDoor.DeviceSimulator.Services;

public class KafkaProducerService : IAsyncDisposable
{
    private readonly IKafkaProducer _kafkaProducer;
    private readonly ILogger<KafkaProducerService> _logger;
    private readonly string _telemetryTopic;

    public KafkaProducerService(IConfiguration configuration, ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<KafkaProducerService>();
        _telemetryTopic = configuration["Kafka:TelemetryTopic"] ?? "device-telemetry";

        var kafkaConfig = new KafkaProducerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"] ?? throw new InvalidOperationException("Kafka:BootstrapServers is required"),
            Username = configuration["Kafka:Username"] ?? throw new InvalidOperationException("Kafka:Username is required"),
            Password = configuration["Kafka:Password"] ?? throw new InvalidOperationException("Kafka:Password is required"),
            MessageTimeoutMs = configuration.GetValue<int?>("Kafka:MessageTimeoutMs") ?? 300000,
            RequestTimeoutMs = configuration.GetValue<int?>("Kafka:RequestTimeoutMs") ?? 300000
        };

        var kafkaLogger = loggerFactory.CreateLogger<KafkaProducer>();
        _kafkaProducer = new KafkaProducer(kafkaConfig, kafkaLogger);

        _logger.LogInformation("Kafka producer service initialized: Topic={Topic}, BootstrapServers={BootstrapServers}", 
            _telemetryTopic, kafkaConfig.BootstrapServers);
    }

    public async Task PublishTelemetryAsync(DeviceTelemetryPayload payload)
    {
        try
        {
            _logger.LogDebug("Publishing telemetry to Kafka: DeviceId={DeviceId}, EventType={EventType}", 
                payload.DeviceId, payload.EventType);

            await _kafkaProducer.PublishAsync(_telemetryTopic, payload.DeviceId, payload);

            _logger.LogDebug("Telemetry published to Kafka successfully: DeviceId={DeviceId}", payload.DeviceId);
        }
        catch (Confluent.Kafka.ProduceException<string, string> ex)
        {
            _logger.LogError(ex, 
                "Kafka produce error for device {DeviceId}: Topic={Topic}, ErrorCode={ErrorCode}, Reason={Reason}", 
                payload.DeviceId, _telemetryTopic, ex.Error.Code, ex.Error.Reason);
            // Don't rethrow - prevents Blazor circuit crashes
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish telemetry to Kafka for device {DeviceId}", payload.DeviceId);
            // Don't rethrow - prevents Blazor circuit crashes
        }
    }

    public async Task PublishBatchAsync(List<DeviceTelemetryPayload> telemetryBatch)
    {
        try
        {
            _logger.LogDebug("Publishing batch of {Count} telemetry messages to Kafka", telemetryBatch.Count);

            var messages = telemetryBatch.Select(t => (Key: t.DeviceId, Message: t));
            await _kafkaProducer.PublishBatchAsync(_telemetryTopic, messages);

            _logger.LogDebug("Batch published to Kafka successfully: Count={Count}", telemetryBatch.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish batch to Kafka: Count={Count}", telemetryBatch.Count);
            throw;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_kafkaProducer is IAsyncDisposable disposable)
        {
            await disposable.DisposeAsync();
        }
    }
}

