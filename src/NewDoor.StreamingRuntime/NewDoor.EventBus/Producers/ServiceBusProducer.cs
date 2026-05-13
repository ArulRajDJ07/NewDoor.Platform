using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace NewDoor.EventBus.Producers;

public class ServiceBusProducer : IKafkaProducer, IAsyncDisposable
{
    private readonly ServiceBusClient _client;
    private readonly ILogger<ServiceBusProducer> _logger;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly Dictionary<string, ServiceBusSender> _senders = new();
    private readonly object _lock = new object();

    public ServiceBusProducer(ServiceBusProducerConfig config, ILogger<ServiceBusProducer> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        try
        {
            _client = new ServiceBusClient(config.ConnectionString);
            _logger.LogInformation("Azure Service Bus client initialized successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize Service Bus client");
            throw;
        }
    }

    private ServiceBusSender GetOrCreateSender(string topic)
    {
        if (_senders.TryGetValue(topic, out var sender))
            return sender;

        lock (_lock)
        {
            if (_senders.TryGetValue(topic, out sender))
                return sender;

            sender = _client.CreateSender(topic);
            _senders[topic] = sender;
            return sender;
        }
    }

    public async Task PublishAsync<T>(string topic, string key, T message, CancellationToken cancellationToken = default)
    {
        try
        {
            var json = JsonSerializer.Serialize(message, _jsonOptions);
            var serviceBusMessage = new ServiceBusMessage(json)
            {
                MessageId = Guid.NewGuid().ToString(),
                Subject = key,
                ContentType = "application/json"
            };

            var sender = GetOrCreateSender(topic);
            await sender.SendMessageAsync(serviceBusMessage, cancellationToken);

            _logger.LogDebug("Published to Service Bus: Topic={Topic}, MessageId={MessageId}, Key={Key}",
                topic, serviceBusMessage.MessageId, key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish to Service Bus: Topic={Topic}, Key={Key}", topic, key);
            throw;
        }
    }

    public async Task PublishBatchAsync<T>(string topic, IEnumerable<(string Key, T Message)> messages, CancellationToken cancellationToken = default)
    {
        try
        {
            var sender = GetOrCreateSender(topic);
            var batch = await sender.CreateMessageBatchAsync(cancellationToken);

            foreach (var (key, message) in messages)
            {
                var json = JsonSerializer.Serialize(message, _jsonOptions);
                var serviceBusMessage = new ServiceBusMessage(json)
                {
                    MessageId = Guid.NewGuid().ToString(),
                    Subject = key,
                    ContentType = "application/json"
                };

                if (!batch.TryAddMessage(serviceBusMessage))
                {
                    await sender.SendMessagesAsync(batch, cancellationToken);
                    batch.Dispose();
                    batch = await sender.CreateMessageBatchAsync(cancellationToken);
                    
                    if (!batch.TryAddMessage(serviceBusMessage))
                    {
                        throw new InvalidOperationException($"Message is too large for batch: {key}");
                    }
                }
            }

            if (batch.Count > 0)
            {
                await sender.SendMessagesAsync(batch, cancellationToken);
            }

            batch.Dispose();
            _logger.LogDebug("Published batch to Service Bus: Topic={Topic}, Count={Count}", topic, batch.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish batch to Service Bus: Topic={Topic}", topic);
            throw;
        }
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var sender in _senders.Values)
        {
            await sender.DisposeAsync();
        }
        _senders.Clear();
        
        await _client.DisposeAsync();
        _logger.LogInformation("Service Bus client disposed");
    }
}
