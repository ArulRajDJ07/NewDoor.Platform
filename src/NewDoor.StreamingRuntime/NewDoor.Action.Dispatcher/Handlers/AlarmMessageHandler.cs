using NewDoor.EventBus.Consumers;
using NewDoor.EventBus.Producers;
using NewDoor.Action.Dispatcher.Models;

namespace NewDoor.Action.Dispatcher.Handlers;

public class AlarmMessageHandler : IKafkaMessageHandler<AlarmEvent>
{
    #region Fields
    private readonly IKafkaProducer _kafkaProducer;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AlarmMessageHandler> _logger;
    #endregion

    #region Constructor
    public AlarmMessageHandler(
        IKafkaProducer kafkaProducer,
        IConfiguration configuration,
        ILogger<AlarmMessageHandler> logger)
    {
        _kafkaProducer = kafkaProducer;
        _configuration = configuration;
        _logger = logger;
    }
    #endregion

    #region Handler
    public async Task HandleAsync(string key, AlarmEvent message, CancellationToken cancellationToken)
    {
        try
        {
            await HandleAlarmAsync(message, cancellationToken);
            await SendNotificationAsync(message, cancellationToken);
            await PublishToBroadcastAsync(message, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling alarm");
            throw;
        }
    }
    #endregion

    #region Private Methods
    private async Task HandleAlarmAsync(AlarmEvent alarm, CancellationToken cancellationToken)
    {
        await Task.Delay(10, cancellationToken);
    }

    private async Task SendNotificationAsync(AlarmEvent alarm, CancellationToken cancellationToken)
    {
        await Task.Delay(10, cancellationToken);

        if (alarm.Severity == "Critical")
        {
            _logger.LogWarning("Critical alarm: {AlarmId} at {BuildingCode}/{Floor}/{Zone}", 
                alarm.AlarmId, alarm.BuildingCode, alarm.Floor, alarm.Zone);
        }
    }

    private async Task PublishToBroadcastAsync(AlarmEvent alarm, CancellationToken cancellationToken)
    {
        var broadcastEvent = new UIBroadcastEvent
        {
            CorrelationId = alarm.CorrelationId,
            EventType = "AlarmTriggered",
            AlarmId = alarm.AlarmId,
            DeviceId = alarm.DeviceId,
            DeviceName = alarm.DeviceName,
            BuildingId = alarm.BuildingId,
            BuildingCode = alarm.BuildingCode,
            Floor = alarm.Floor,
            Zone = alarm.Zone,
            Severity = alarm.Severity,
            Message = alarm.Message,
            Timestamp = DateTime.UtcNow,
            Data = alarm.Context
        };

        var broadcastTopic = _configuration["Kafka:UIBroadcastTopic"] ?? "newdoor.ui.broadcast";
        await _kafkaProducer.PublishAsync(broadcastTopic, broadcastEvent.DeviceId, broadcastEvent, cancellationToken);
    }
    #endregion
}
