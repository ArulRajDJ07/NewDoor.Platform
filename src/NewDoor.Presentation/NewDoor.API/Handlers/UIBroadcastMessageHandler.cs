using NewDoor.EventBus.Consumers;
using Microsoft.AspNetCore.SignalR;
using NewDoor.API.Hubs;
using NewDoor.API.Models;

namespace NewDoor.API.Handlers;

public class UIBroadcastMessageHandler : IKafkaMessageHandler<UIBroadcastEvent>
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly ILogger<UIBroadcastMessageHandler> _logger;

    public UIBroadcastMessageHandler(
        IHubContext<NotificationHub> hubContext,
        ILogger<UIBroadcastMessageHandler> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task HandleAsync(string key, UIBroadcastEvent message, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Handling UI broadcast: BroadcastId={BroadcastId}, EventType={EventType}, DeviceId={DeviceId}", 
                message.BroadcastId, message.EventType, message.DeviceId);

            LogAlarmData(message);
            LogIncidentData(message);
            await BroadcastToClientsAsync(message, cancellationToken);

            _logger.LogInformation("UI broadcast handled successfully: BroadcastId={BroadcastId}", message.BroadcastId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling UI broadcast: BroadcastId={BroadcastId}", message.BroadcastId);
            throw;
        }
    }

    private void LogAlarmData(UIBroadcastEvent broadcastEvent)
    {
        _logger.LogInformation(
            "ALARM DATA - DeviceId={DeviceId}, Type={EventType}, Severity={Severity}, Message={Message}, Timestamp={Timestamp}", 
            broadcastEvent.DeviceId, 
            broadcastEvent.EventType, 
            broadcastEvent.Severity, 
            broadcastEvent.Message, 
            broadcastEvent.Timestamp);
    }

    private void LogIncidentData(UIBroadcastEvent broadcastEvent)
    {
        _logger.LogInformation(
            "INCIDENT DATA - DeviceId={DeviceId}, Type={EventType}, Severity={Severity}, BuildingId={BuildingId}, Location={Location}", 
            broadcastEvent.DeviceId, 
            broadcastEvent.EventType, 
            broadcastEvent.Severity, 
            broadcastEvent.BuildingId,
            $"{broadcastEvent.BuildingCode}/{broadcastEvent.Floor}/{broadcastEvent.Zone}");
    }

    private async Task BroadcastToClientsAsync(UIBroadcastEvent broadcastEvent, CancellationToken cancellationToken)
    {
        var dashboardAlert = new DashboardAlert
        {
            AlertId = broadcastEvent.BroadcastId,
            DeviceId = broadcastEvent.DeviceId,
            DeviceName = broadcastEvent.DeviceName,
            BuildingCode = broadcastEvent.BuildingCode,
            Location = $"{broadcastEvent.Floor} / {broadcastEvent.Zone}",
            Severity = broadcastEvent.Severity,
            Message = broadcastEvent.Message,
            Timestamp = broadcastEvent.Timestamp,
            AdditionalData = broadcastEvent.Data
        };

        await _hubContext.Clients.All.SendAsync("ReceiveAlert", dashboardAlert, cancellationToken);

        if (broadcastEvent.BuildingId > 0)
        {
            var groupName = $"Building_{broadcastEvent.BuildingId}";
            await _hubContext.Clients.Group(groupName).SendAsync("ReceiveAlert", dashboardAlert, cancellationToken);
        }

        _logger.LogInformation("Alert broadcast via SignalR: AlertId={AlertId}, Severity={Severity}", 
            dashboardAlert.AlertId, dashboardAlert.Severity);
    }
}

