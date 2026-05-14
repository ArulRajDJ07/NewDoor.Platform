using NewDoor.EventBus.Consumers;
using Microsoft.AspNetCore.SignalR;
using MediatR;
using NewDoor.API.Hubs;
using NewDoor.API.Models;
using NewDoor.API.Features.Alarms.Command;
using NewDoor.API.Features.Incidents.Query;
using NewDoor.Platform.DTO.Features.Alarms.Models;
using NewDoor.Platform.DTO.Features.Incidents.Models;
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

        AlarmResponse? alarmResponse = null;
        bool canPersist = false;

        try
        {
            canPersist = await ValidateForeignKeysAsync(mediator, message, cancellationToken);
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
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to store alarm {AlarmCode}", message.AlarmCode);
            }
        }

        try
        {
            await BroadcastAlarmToUIAsync(hubContext, alarmResponse, message, cancellationToken);
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
        // Hackathon: simplified validation
        return message.DeviceId > 0 && message.BuildingId > 0 && message.RuleId > 0;
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

    private async Task BroadcastAlarmToUIAsync(IHubContext<NotificationHub> hubContext, AlarmResponse? alarm, AlarmCreatedEvent message, CancellationToken cancellationToken)
    {
        var dashboardAlert = new DashboardAlert
        {
            AlertId = alarm?.AlarmCode ?? message.AlarmCode,
            DeviceId = message.DeviceId.ToString(),
            DeviceName = "",
            BuildingCode = message.BuildingCode,
            Location = $"{message.Floor} / {message.Zone}",
            Severity = alarm?.Severity ?? message.Severity,
            Message = alarm?.AlarmMessage ?? message.AlarmMessage,
            Timestamp = alarm?.TriggeredUtc ?? message.TriggeredUtc,
            AdditionalData = new Dictionary<string, object>
            {
                { "BuildingId", message.BuildingId },
                { "RuleId", message.RuleId },
                { "AlarmStatus", message.AlarmStatus },
                { "AlarmType", message.AlarmType },
                { "Floor", message.Floor },
                { "Zone", message.Zone }
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
