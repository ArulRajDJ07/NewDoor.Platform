using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace NewDoor.EventBus.Consumers;

public class ServiceBusConsumer<T> : IKafkaConsumer, IAsyncDisposable
{
    private readonly ServiceBusClient _client;
    private readonly ServiceBusProcessor _processor;
    private readonly IKafkaMessageHandler<T> _messageHandler;
    private readonly ILogger<ServiceBusConsumer<T>> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public ServiceBusConsumer(
        ServiceBusConsumerConfig config,
        IKafkaMessageHandler<T> messageHandler,
        ILogger<ServiceBusConsumer<T>> logger)
    {
        _messageHandler = messageHandler ?? throw new ArgumentNullException(nameof(messageHandler));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        try
        {
            _client = new ServiceBusClient(config.ConnectionString);
            
            var options = new ServiceBusProcessorOptions
            {
                AutoCompleteMessages = false,
                MaxConcurrentCalls = config.MaxConcurrentCalls,
                PrefetchCount = config.PrefetchCount
            };

            _processor = _client.CreateProcessor(config.TopicName, config.SubscriptionName, options);
            
            _processor.ProcessMessageAsync += ProcessMessageAsync;
            _processor.ProcessErrorAsync += ProcessErrorAsync;

            _logger.LogInformation(
                "Azure Service Bus consumer initialized: Topic={Topic}, Subscription={Subscription}",
                config.TopicName,
                config.SubscriptionName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize Service Bus consumer");
            throw;
        }
    }

    public async Task StartConsumingAsync(string topic, CancellationToken cancellationToken)
    {
        try
        {
            await _processor.StartProcessingAsync(cancellationToken);
            _logger.LogInformation("Service Bus consumer started processing messages");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start Service Bus consumer");
            throw;
        }
    }

    private async Task ProcessMessageAsync(ProcessMessageEventArgs args)
    {
        try
        {
            var body = args.Message.Body.ToString();
            var key = args.Message.Subject ?? args.Message.MessageId;

            _logger.LogDebug(
                "Received Service Bus message: MessageId={MessageId}, Subject={Subject}",
                args.Message.MessageId,
                args.Message.Subject);

            var message = JsonSerializer.Deserialize<T>(body, _jsonOptions);
            
            if (message != null)
            {
                await _messageHandler.HandleAsync(key, message, args.CancellationToken);
                await args.CompleteMessageAsync(args.Message, args.CancellationToken);
                
                _logger.LogDebug(
                    "Successfully processed Service Bus message: MessageId={MessageId}",
                    args.Message.MessageId);
            }
            else
            {
                _logger.LogWarning(
                    "Failed to deserialize message: MessageId={MessageId}",
                    args.Message.MessageId);
                await args.DeadLetterMessageAsync(args.Message, cancellationToken: args.CancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error processing Service Bus message: MessageId={MessageId}",
                args.Message.MessageId);

            // Let Service Bus retry or move to dead-letter queue based on retry policy
            await args.AbandonMessageAsync(args.Message, cancellationToken: args.CancellationToken);
        }
    }

    private Task ProcessErrorAsync(ProcessErrorEventArgs args)
    {
        _logger.LogError(args.Exception,
            "Service Bus error: EntityPath={EntityPath}, ErrorSource={ErrorSource}",
            args.EntityPath,
            args.ErrorSource);
        
        return Task.CompletedTask;
    }

    public async Task StopConsumingAsync()
    {
        try
        {
            await _processor.StopProcessingAsync();
            _logger.LogInformation("Service Bus consumer stopped processing");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error stopping Service Bus consumer");
        }
    }

    public async ValueTask DisposeAsync()
    {
        await StopConsumingAsync();
        
        if (_processor != null)
        {
            await _processor.DisposeAsync();
        }
        
        if (_client != null)
        {
            await _client.DisposeAsync();
        }
    }
}
