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
            _logger.LogInformation("Processing alarm {AlarmId}", message.AlarmId);

            // 1. Publish to database persistence topic
            await PublishAlarmCreatedAsync(message, cancellationToken);

            // 2. Publish to audit history
            await PublishAuditHistoryAsync(message, cancellationToken);

            // 3. Trigger notification for critical alarms
            await SendNotificationAsync(message, cancellationToken);

            // 4. Broadcast to UI via SignalR
            await PublishToBroadcastAsync(message, cancellationToken);

            _logger.LogInformation("Alarm {AlarmId} processed", message.AlarmId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling alarm {AlarmId}", message.AlarmId);
            throw;
        }
    }
    #endregion

    #region Private Methods
    private async Task PublishAlarmCreatedAsync(AlarmEvent alarm, CancellationToken cancellationToken)
    {
        var alarmCreatedEvent = new AlarmCreatedEvent
        {
            AlarmCode = alarm.AlarmId,
            CorrelationId = alarm.CorrelationId,
            DeviceId = int.TryParse(alarm.DeviceId, out var deviceId) ? deviceId : 0,
            BuildingId = alarm.BuildingId,
            BuildingCode = alarm.BuildingCode,
            RuleId = alarm.RuleId,
            IncidentCode = alarm.IncidentId,
            Severity = alarm.Severity,
            AlarmMessage = alarm.Message,
            AlarmStatus = "Active",
            TriggeredUtc = alarm.TriggeredAtUtc,
            TriggeredBy = "System",
            AlarmType = alarm.AlarmType,
            Floor = alarm.Floor,
            Zone = alarm.Zone,
            Context = alarm.Context
        };

        var alarmCreatedTopic = _configuration["Kafka:AlarmCreatedTopic"] ?? "newdoor.alarm.created";
        await _kafkaProducer.PublishAsync(alarmCreatedTopic, alarm.AlarmId, alarmCreatedEvent, cancellationToken);

        _logger.LogInformation("Published to {Topic}", alarmCreatedTopic);
    }

    private async Task PublishAuditHistoryAsync(AlarmEvent alarm, CancellationToken cancellationToken)
    {
        var auditEvent = new AuditHistoryEvent
        {
            CorrelationId = alarm.CorrelationId,
            EventId = 0, // Will be populated by API consumer
            DeviceId = int.TryParse(alarm.DeviceId, out var deviceId) ? deviceId : 0,
            EventType = alarm.AlarmType,
            Severity = alarm.Severity,
            ProcessingResult = "Alarm Triggered",
            ProcessorName = "Action.Dispatcher.AlarmHandler",
            Remarks = $"Alarm {alarm.AlarmId} triggered at {alarm.BuildingCode}/{alarm.Floor}/{alarm.Zone} - {alarm.Message}",
            ProcessedUtc = DateTime.UtcNow,
            Metadata = new Dictionary<string, object>
            {
                ["AlarmId"] = alarm.AlarmId,
                ["AlarmType"] = alarm.AlarmType,
                ["BuildingCode"] = alarm.BuildingCode,
                ["Floor"] = alarm.Floor,
                ["Zone"] = alarm.Zone,
                ["DeviceId"] = alarm.DeviceId,
                ["DeviceName"] = alarm.DeviceName,
                ["IncidentId"] = alarm.IncidentId,
                ["TriggeredAtUtc"] = alarm.TriggeredAtUtc,
                ["Context"] = alarm.Context
            }
        };

        var auditTopic = _configuration["Kafka:AuditHistoryTopic"] ?? "newdoor.audit.history";
        await _kafkaProducer.PublishAsync(auditTopic, alarm.CorrelationId, auditEvent, cancellationToken);

        _logger.LogInformation("Published to {Topic}", auditTopic);
    }

    private async Task SendNotificationAsync(AlarmEvent alarm, CancellationToken cancellationToken)
    {
        // Log critical/high-severity alarms for notification
        if (alarm.Severity == "Critical" || alarm.Severity == "High")
        {
            _logger.LogWarning(
                "{Severity} alarm: {AlarmId} at {BuildingCode}/{Floor}/{Zone}",
                alarm.Severity, alarm.AlarmId, alarm.BuildingCode, alarm.Floor, alarm.Zone);

            // TODO: Implement notification delivery
            // - Send email for critical alarms
            // - Send SMS for high-severity alarms
            // - Push notifications to mobile apps
            // - Alert on-call personnel
            // - Integration with notification service/queue
        }

        await Task.CompletedTask;
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

        _logger.LogInformation("Published to {Topic}", broadcastTopic);
    }
    #endregion
}
