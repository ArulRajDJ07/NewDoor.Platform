using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace NewDoor.EventBus.Consumers;

public class KafkaConsumer<T> : IKafkaConsumer, IAsyncDisposable
{
    private readonly IConsumer<string, string> _consumer;
    private readonly IKafkaMessageHandler<T> _messageHandler;
    private readonly ILogger<KafkaConsumer<T>> _logger;
    private readonly JsonSerializerOptions _jsonOptions;
    private Task? _consumeTask;
    private CancellationTokenSource? _cts;

    public KafkaConsumer(
        KafkaConsumerConfig config, 
        IKafkaMessageHandler<T> messageHandler,
        ILogger<KafkaConsumer<T>> logger)
    {
        _messageHandler = messageHandler;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = config.BootstrapServers,
            SecurityProtocol = SecurityProtocol.SaslSsl,
            SaslMechanism = SaslMechanism.Plain,
            SaslUsername = config.Username,
            SaslPassword = config.Password,
            GroupId = config.GroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false,
            SessionTimeoutMs = 30000,
            MaxPollIntervalMs = 300000
        };

        _consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
        _logger.LogInformation("Kafka consumer initialized: GroupId={GroupId}", config.GroupId);
    }

    public Task StartConsumingAsync(string topic, CancellationToken cancellationToken)
    {
        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _consumer.Subscribe(topic);
        _logger.LogInformation("Subscribed to Kafka topic: {Topic}", topic);

        _consumeTask = Task.Run(async () => await ConsumeLoopAsync(topic, _cts.Token), _cts.Token);
        return Task.CompletedTask;
    }

    private async Task ConsumeLoopAsync(string topic, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Starting consume loop for topic: {Topic}", topic);

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = _consumer.Consume(cancellationToken);

                    if (consumeResult?.Message == null)
                        continue;

                    _logger.LogInformation("Consumed message from Kafka: Topic={Topic}, Partition={Partition}, Offset={Offset}, Key={Key}", 
                        topic, consumeResult.Partition.Value, consumeResult.Offset.Value, consumeResult.Message.Key);

                    var message = JsonSerializer.Deserialize<T>(consumeResult.Message.Value, _jsonOptions);

                    if (message != null)
                    {
                        _logger.LogInformation("Message deserialized successfully, invoking handler for type {MessageType}", typeof(T).Name);
                        await _messageHandler.HandleAsync(consumeResult.Message.Key, message, cancellationToken);
                        _consumer.Commit(consumeResult);
                        _logger.LogInformation("Message committed: Offset={Offset}", consumeResult.Offset.Value);
                    }
                    else
                    {
                        _logger.LogWarning("Message deserialized to null, skipping. Topic={Topic}, Offset={Offset}", topic, consumeResult.Offset.Value);
                    }
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "Kafka consume error: {Reason}", ex.Error.Reason);
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex, "JSON deserialization error for message from topic {Topic}", topic);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing Kafka message from topic {Topic}", topic);
                }
            }

            _logger.LogInformation("Consume loop ending for topic: {Topic}", topic);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Kafka consumer cancelled for topic: {Topic}", topic);
        }
        finally
        {
            _logger.LogInformation("Closing consumer for topic: {Topic}", topic);
            _consumer.Close();
        }
    }

    public async Task StopConsumingAsync()
    {
        _cts?.Cancel();
        if (_consumeTask != null)
        {
            await _consumeTask;
        }
    }

    public async ValueTask DisposeAsync()
    {
        await StopConsumingAsync();
        _consumer?.Dispose();
        _cts?.Dispose();
    }
}
