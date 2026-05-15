using NewDoor.EventBus.Consumers;
using NewDoor.API.Models;
using NewDoor.API.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NewDoor.API.BackgroundServices;

public class AuditHistoryConsumerService : BackgroundService
{
    #region Fields
    private readonly IKafkaConsumer _kafkaConsumer;
    private readonly KafkaTopicConfiguration _topicConfig;
    private readonly ILogger<AuditHistoryConsumerService> _logger;
    #endregion

    #region Constructor
    public AuditHistoryConsumerService(
        [FromKeyedServices(KafkaConsumerKeys.AuditHistory)] IKafkaConsumer kafkaConsumer,
        [FromKeyedServices(KafkaConsumerKeys.AuditHistory)] KafkaTopicConfiguration topicConfig,
        ILogger<AuditHistoryConsumerService> logger)
    {
        _kafkaConsumer = kafkaConsumer;
        _topicConfig = topicConfig;
        _logger = logger;
    }
    #endregion

    #region BackgroundService
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("Starting consumer {ConsumerKey} on topic {Topic}", 
                _topicConfig.ConsumerKey, _topicConfig.TopicName);

            await _kafkaConsumer.StartConsumingAsync(_topicConfig.TopicName, stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Consumer {ConsumerKey} failed", _topicConfig.ConsumerKey);
            throw;
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping consumer {ConsumerKey}", _topicConfig.ConsumerKey);
        await _kafkaConsumer.StopConsumingAsync();
        await base.StopAsync(cancellationToken);
    }
    #endregion
}
