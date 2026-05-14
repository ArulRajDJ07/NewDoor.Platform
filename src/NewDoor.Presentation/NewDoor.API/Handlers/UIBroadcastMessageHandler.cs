using NewDoor.EventBus.Consumers;
using Microsoft.AspNetCore.SignalR;
using NewDoor.API.Hubs;
using NewDoor.API.Models;

namespace NewDoor.API.Handlers;

public class UIBroadcastMessageHandler : IKafkaMessageHandler<UIBroadcastEvent>
{
    #region Fields
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly ILogger<UIBroadcastMessageHandler> _logger;
    #endregion

    #region Constructor
    public UIBroadcastMessageHandler(
        IHubContext<NotificationHub> hubContext,
        ILogger<UIBroadcastMessageHandler> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }
    #endregion

    #region Handler
    public async Task HandleAsync(string key, UIBroadcastEvent message, CancellationToken cancellationToken)
    {
        try
        {
            await BroadcastToClientsAsync(message, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error broadcasting to UI");
            throw;
        }
    }
    #endregion

    #region Private Methods
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
    }
    #endregion
}