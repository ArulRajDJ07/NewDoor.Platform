using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace NewDoor.EventBus.Producers;

public class KafkaProducer : IKafkaProducer, IAsyncDisposable
{
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<KafkaProducer> _logger;
    private readonly JsonSerializerOptions _jsonOptions;
    private const int MaxRetries = 3;
    private static readonly TimeSpan[] RetryDelays = 
    {
        TimeSpan.FromSeconds(1),
        TimeSpan.FromSeconds(2),
        TimeSpan.FromSeconds(4)
    };

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
            MessageTimeoutMs = config.MessageTimeoutMs,
            RequestTimeoutMs = config.RequestTimeoutMs,
            SocketTimeoutMs = config.MessageTimeoutMs,
            MessageSendMaxRetries = 3,
            RetryBackoffMs = 1000,
            SocketKeepaliveEnable = true
        };

        _producer = new ProducerBuilder<string, string>(producerConfig)
            .SetErrorHandler((_, e) => _logger.LogError("Kafka error: Code={Code}, Reason={Reason}, IsBrokerError={IsBrokerError}", 
                e.Code, e.Reason, e.IsBrokerError))
            .Build();

        _logger.LogInformation("Kafka producer initialized: {BootstrapServers}", config.BootstrapServers);
    }

    public async Task PublishAsync<T>(string topic, string key, T message, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(message, _jsonOptions);
        var kafkaMessage = new Message<string, string>
        {
            Key = key,
            Value = json,
            Timestamp = new Timestamp(DateTime.UtcNow)
        };

        for (int attempt = 0; attempt <= MaxRetries; attempt++)
        {
            try
            {
                var result = await _producer.ProduceAsync(topic, kafkaMessage, cancellationToken);

                _logger.LogDebug("Published to Kafka: Topic={Topic}, Partition={Partition}, Offset={Offset}, Key={Key}", 
                    topic, result.Partition.Value, result.Offset.Value, key);
                return;
            }
            catch (ProduceException<string, string> ex) when (attempt < MaxRetries && IsRetriableError(ex.Error.Code))
            {
                var delay = RetryDelays[attempt];
                _logger.LogWarning(ex, "Kafka publish failed (attempt {Attempt}/{MaxAttempts}): Topic={Topic}, Key={Key}, ErrorCode={ErrorCode}, Reason={Reason}. Retrying in {Delay}s...", 
                    attempt + 1, MaxRetries + 1, topic, key, ex.Error.Code, ex.Error.Reason, delay.TotalSeconds);

                await Task.Delay(delay, cancellationToken);
            }
            catch (ProduceException<string, string> ex)
            {
                LogProduceException(ex, topic, key, attempt);
                throw;
            }
            catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
            {
                _logger.LogWarning(ex, "Publish operation canceled: Topic={Topic}, Key={Key}", topic, key);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish to Kafka: Topic={Topic}, Key={Key}", topic, key);
                throw;
            }
        }
    }

    private static bool IsRetriableError(ErrorCode errorCode)
    {
        return errorCode == ErrorCode.Local_MsgTimedOut ||
               errorCode == ErrorCode.RequestTimedOut ||
               errorCode == ErrorCode.Local_QueueFull ||
               errorCode == ErrorCode.Local_TimedOut;
    }

    private void LogProduceException(ProduceException<string, string> ex, string topic, string key, int attempt)
    {
        var attemptInfo = attempt > 0 ? $" after {attempt + 1} attempts" : "";

        if (ex.Error.Code == ErrorCode.Local_MsgTimedOut)
        {
            _logger.LogError(ex, "Kafka message timeout{AttemptInfo}: Topic={Topic}, Key={Key}, Reason={Reason}. Message exceeded configured timeout. Check broker connectivity and network latency.", 
                attemptInfo, topic, key, ex.Error.Reason);
        }
        else if (ex.Error.Code == ErrorCode.RequestTimedOut)
        {
            _logger.LogError(ex, "Kafka request timeout{AttemptInfo}: Topic={Topic}, Key={Key}, Reason={Reason}. Broker may be slow or unreachable.", 
                attemptInfo, topic, key, ex.Error.Reason);
        }
        else if (ex.Error.Code == ErrorCode.Local_QueueFull)
        {
            _logger.LogError(ex, "Kafka queue full{AttemptInfo}: Topic={Topic}, Key={Key}, Reason={Reason}. Producer cannot keep up with send rate. Consider increasing queue size or reducing send rate.", 
                attemptInfo, topic, key, ex.Error.Reason);
        }
        else
        {
            _logger.LogError(ex, "Kafka produce error{AttemptInfo}: Topic={Topic}, Key={Key}, ErrorCode={ErrorCode}, Reason={Reason}", 
                attemptInfo, topic, key, ex.Error.Code, ex.Error.Reason);
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
