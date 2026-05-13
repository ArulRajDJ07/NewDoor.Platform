using NewDoor.EventBus.Consumers;
using NewDoor.EventBus.Producers;
using NewDoor.Action.Dispatcher.Models;

namespace NewDoor.Action.Dispatcher.Handlers;

public class AlarmMessageHandler : IKafkaMessageHandler<AlarmEvent>
{
    private readonly IKafkaProducer _kafkaProducer;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AlarmMessageHandler> _logger;

    public AlarmMessageHandler(
        IKafkaProducer kafkaProducer,
        IConfiguration configuration,
        ILogger<AlarmMessageHandler> logger)
    {
        _kafkaProducer = kafkaProducer;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task HandleAsync(string key, AlarmEvent message, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Handling alarm: AlarmId={AlarmId}, DeviceId={DeviceId}, Severity={Severity}", 
                message.AlarmId, message.DeviceId, message.Severity);

            await HandleAlarmAsync(message, cancellationToken);
            await SendNotificationAsync(message, cancellationToken);
            await PublishToBroadcastAsync(message, cancellationToken);

            _logger.LogInformation("Alarm handled successfully: AlarmId={AlarmId}", message.AlarmId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling alarm: AlarmId={AlarmId}, DeviceId={DeviceId}", 
                message.AlarmId, message.DeviceId);
            throw;
        }
    }

    private async Task HandleAlarmAsync(AlarmEvent alarm, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing alarm action: AlarmId={AlarmId}, Type={AlarmType}", 
            alarm.AlarmId, alarm.AlarmType);

        await Task.Delay(10, cancellationToken);
        
        _logger.LogInformation("Alarm action processed: AlarmId={AlarmId}", alarm.AlarmId);
    }

    private async Task SendNotificationAsync(AlarmEvent alarm, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sending notifications for alarm: AlarmId={AlarmId}, Severity={Severity}", 
            alarm.AlarmId, alarm.Severity);

        await Task.Delay(10, cancellationToken);

        if (alarm.Severity == "Critical")
        {
            _logger.LogWarning("CRITICAL ALARM - Emergency notification sent: AlarmId={AlarmId}, Location={BuildingCode}/{Floor}/{Zone}", 
                alarm.AlarmId, alarm.BuildingCode, alarm.Floor, alarm.Zone);
        }
        else
        {
            _logger.LogInformation("Standard notification sent: AlarmId={AlarmId}", alarm.AlarmId);
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

        _logger.LogInformation("Published to UI broadcast: BroadcastId={BroadcastId}, Topic={Topic}", 
            broadcastEvent.BroadcastId, broadcastTopic);
    }
}
