using NewDoor.EventBus.Consumers;
using Microsoft.AspNetCore.SignalR;
using MediatR;
using NewDoor.API.Hubs;
using NewDoor.API.Models;
using NewDoor.API.Features.Alarms.Command;
using NewDoor.API.Features.Incidents.Query;
using NewDoor.API.Features.Devices.Query;
using NewDoor.Platform.DTO.Features.Alarms.Models;
using NewDoor.Platform.DTO.Features.Incidents.Models;
using NewDoor.Platform.DTO.Features.Devices.Models;
using NewDoor.Platform.DTO.Common;
using NewDoor.API.Repositories.Interface;

namespace NewDoor.API.Handlers;

public class AlarmCreatedMessageHandler : IKafkaMessageHandler<AlarmCreatedEvent>
{
    #region Fields
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<AlarmCreatedMessageHandler> _logger;
    #endregion

    #region Constructor
    public AlarmCreatedMessageHandler(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<AlarmCreatedMessageHandler> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }
    #endregion

    #region Handler
    public async Task HandleAsync(string key, AlarmCreatedEvent message, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing alarm {AlarmCode}", message.AlarmCode);

        using var scope = _serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<NotificationHub>>();

        try
        {
            var alarmResponse = await StoreAlarmAsync(mediator, message, cancellationToken);
            await BroadcastAlarmToUIAsync(hubContext, alarmResponse, message, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Alarm handler failed for {Code}", message.AlarmCode);
            throw;
        }
    }
    #endregion

    #region Private Methods
    private async Task<bool> ValidateForeignKeysAsync(IMediator mediator, AlarmCreatedEvent message, CancellationToken cancellationToken)
    {
        // Just validate building and rule exist
        return message.BuildingId > 0 && message.RuleId > 0;
    }

    private async Task<AlarmResponse> StoreAlarmAsync(IMediator mediator, AlarmCreatedEvent message, CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var alarmRepository = scope.ServiceProvider.GetRequiredService<IAlarmRepository>();

            var existingAlarm = await alarmRepository.GetByAlarmCodeAsync(message.AlarmCode);

            if (existingAlarm != null)
            {
                return new AlarmResponse
                {
                    Id = existingAlarm.Id,
                    AlarmCode = existingAlarm.AlarmCode,
                    DeviceId = existingAlarm.DeviceId,
                    BuildingId = existingAlarm.BuildingId,
                    RuleId = existingAlarm.RuleId ?? 0,
                    IncidentId = existingAlarm.IncidentId,
                    Severity = existingAlarm.Severity,
                    AlarmMessage = existingAlarm.AlarmMessage,
                    AlarmStatus = existingAlarm.AlarmStatus,
                    TriggeredUtc = existingAlarm.TriggeredUtc,
                    TriggeredBy = existingAlarm.TriggeredBy
                };
            }

            int? incidentId = null;
            if (!string.IsNullOrEmpty(message.IncidentCode))
            {
                try
                {
                    var incidentFilter = new IncidentFilterRequest
                    {
                        Filters = new List<FilterRequest>
                        {
                            new FilterRequest
                            {
                                FieldName = "IncidentCode",
                                Operator = "eq",
                                Value = message.IncidentCode
                            }
                        },
                        PageSize = 1
                    };

                    var incidentQuery = new FindAllIncidentQuery(incidentFilter);
                    var incidents = await mediator.Send(incidentQuery, cancellationToken);
                    incidentId = incidents.FirstOrDefault()?.Id;
                }
                catch { }
            }

            var addAlarmRequest = new AddAlarmRequest
            {
                AlarmCode = message.AlarmCode,
                DeviceId = message.DeviceId,
                BuildingId = message.BuildingId,
                RuleId = message.RuleId,
                IncidentId = incidentId,
                Severity = message.Severity,
                AlarmMessage = message.AlarmMessage,
                AlarmStatus = message.AlarmStatus,
                TriggeredUtc = message.TriggeredUtc,
                TriggeredBy = message.TriggeredBy
            };

            var command = new AddAlarmCommand(addAlarmRequest);
            return await mediator.Send(command, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DB error for alarm {Code} - using fallback", message.AlarmCode);

            return new AlarmResponse
            {
                AlarmCode = message.AlarmCode,
                DeviceId = message.DeviceId,
                BuildingId = message.BuildingId,
                RuleId = message.RuleId,
                Severity = message.Severity,
                AlarmMessage = message.AlarmMessage,
                AlarmStatus = message.AlarmStatus,
                TriggeredUtc = message.TriggeredUtc,
                TriggeredBy = message.TriggeredBy
            };
        }
    }

    private async Task BroadcastAlarmToUIAsync(IHubContext<NotificationHub> hubContext, AlarmResponse? alarm, AlarmCreatedEvent message, CancellationToken cancellationToken)
    {
        // Create a meaningful message if AlarmMessage is empty
        var alarmMessage = alarm?.AlarmMessage ?? message.AlarmMessage;
        var displayMessage = !string.IsNullOrWhiteSpace(alarmMessage) 
            ? alarmMessage 
            : $"{message.AlarmType} alarm triggered - {message.Severity} severity";

        var dashboardAlert = new DashboardAlert
        {
            AlertId = alarm?.AlarmCode ?? message.AlarmCode,
            DeviceId = message.DeviceId?.ToString() ?? "Unknown",
            DeviceName = "Sensor Device",
            BuildingCode = message.BuildingCode ?? "Unknown",
            Location = $"{message.Floor ?? "Unknown"} / {message.Zone ?? "Unknown"}",
            Severity = alarm?.Severity ?? message.Severity,
            Message = displayMessage,
            Timestamp = alarm?.TriggeredUtc ?? message.TriggeredUtc,
            AdditionalData = new Dictionary<string, object>
            {
                { "BuildingId", message.BuildingId },
                { "RuleId", message.RuleId },
                { "AlarmStatus", message.AlarmStatus },
                { "AlarmType", message.AlarmType ?? "General" },
                { "Floor", message.Floor ?? "Unknown" },
                { "Zone", message.Zone ?? "Unknown" }
            }
        };

        // Broadcast to all clients
        await hubContext.Clients.All.SendAsync("ReceiveAlarm", dashboardAlert, cancellationToken);

        // Also broadcast to building-specific group
        await hubContext.Clients.Group($"Building_{message.BuildingId}")
            .SendAsync("ReceiveAlarm", dashboardAlert, cancellationToken);
    }
    #endregion
}
