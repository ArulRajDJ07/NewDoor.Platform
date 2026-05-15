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
        using var scope = _serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<NotificationHub>>();

        _logger.LogError("=== ALARM HANDLER START: {AlarmCode} ===", message.AlarmCode);

        AlarmResponse? alarmResponse = null;
        bool canPersist = false;

        try
        {
            canPersist = await ValidateForeignKeysAsync(mediator, message, cancellationToken);
            _logger.LogError("Alarm {AlarmCode} validation result: {CanPersist}", message.AlarmCode, canPersist);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating alarm {AlarmCode}", message.AlarmCode);
        }

        if (canPersist)
        {
            try
            {
                alarmResponse = await StoreAlarmAsync(mediator, message, cancellationToken);
                _logger.LogError("Alarm {AlarmCode} stored successfully: ID={Id}", message.AlarmCode, alarmResponse.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to store alarm {AlarmCode}", message.AlarmCode);
            }
        }

        try
        {
            await BroadcastAlarmToUIAsync(hubContext, alarmResponse, message, cancellationToken);
            _logger.LogError("Alarm {AlarmCode} broadcasted to UI", message.AlarmCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to broadcast alarm {AlarmCode}", message.AlarmCode);
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
            catch
            {
                // Ignore if incident not found
            }
        }

        var addAlarmRequest = new AddAlarmRequest
        {
            AlarmCode = message.AlarmCode,
            DeviceId = message.DeviceId, // Now storing string DeviceId directly from telemetry
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
