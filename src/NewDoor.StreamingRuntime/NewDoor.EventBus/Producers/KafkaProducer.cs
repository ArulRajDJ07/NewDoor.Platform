using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace NewDoor.EventBus.Producers;

public class KafkaProducer : IKafkaProducer, IAsyncDisposable
{
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<KafkaProducer> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public KafkaProducer(KafkaProducerConfig config, ILogger<KafkaProducer> logger)
    {
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var producerConfig = new ProducerConfig
        {
            BootstrapServers = config.BootstrapServers,
            SecurityProtocol = SecurityProtocol.SaslSsl,
            SaslMechanism = SaslMechanism.Plain,
            SaslUsername = config.Username,
            SaslPassword = config.Password,
            Acks = Acks.Leader,
            EnableIdempotence = false,
            CompressionType = CompressionType.Snappy,
            LingerMs = 10,
            BatchSize = 32768,
            QueueBufferingMaxMessages = 100000,
            QueueBufferingMaxKbytes = 1048576,
            MessageTimeoutMs = 30000
        };

        _producer = new ProducerBuilder<string, string>(producerConfig).Build();
        _logger.LogInformation("Kafka producer initialized: {BootstrapServers}", config.BootstrapServers);
    }

    public async Task PublishAsync<T>(string topic, string key, T message, CancellationToken cancellationToken = default)
    {
        try
        {
            var json = JsonSerializer.Serialize(message, _jsonOptions);
            var kafkaMessage = new Message<string, string>
            {
                Key = key,
                Value = json,
                Timestamp = new Timestamp(DateTime.UtcNow)
            };

            var result = await _producer.ProduceAsync(topic, kafkaMessage, cancellationToken);
            
            _logger.LogDebug("Published to Kafka: Topic={Topic}, Partition={Partition}, Offset={Offset}, Key={Key}", 
                topic, result.Partition.Value, result.Offset.Value, key);
        }
        catch (ProduceException<string, string> ex)
        {
            _logger.LogError(ex, "Kafka produce error: Topic={Topic}, Key={Key}, Reason={Reason}", 
                topic, key, ex.Error.Reason);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish to Kafka: Topic={Topic}, Key={Key}", topic, key);
            throw;
        }
    }

    public async Task PublishBatchAsync<T>(string topic, IEnumerable<(string Key, T Message)> messages, CancellationToken cancellationToken = default)
    {
        var tasks = messages.Select(m => PublishAsync(topic, m.Key, m.Message, cancellationToken));
        await Task.WhenAll(tasks);
    }

    public async ValueTask DisposeAsync()
    {
        _producer?.Flush(TimeSpan.FromSeconds(10));
        _producer?.Dispose();
        await Task.CompletedTask;
    }
}
