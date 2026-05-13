using Microsoft.AspNetCore.SignalR;
using NewDoor.API.Models;

namespace NewDoor.API.Hubs;

public class NotificationHub : Hub
{
    private readonly ILogger<NotificationHub> _logger;

    public NotificationHub(ILogger<NotificationHub> logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("Client connected: ConnectionId={ConnectionId}", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("Client disconnected: ConnectionId={ConnectionId}", Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SubscribeToBuilding(int buildingId)
    {
        var groupName = $"Building_{buildingId}";
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        _logger.LogInformation("Client subscribed to building: ConnectionId={ConnectionId}, BuildingId={BuildingId}", 
            Context.ConnectionId, buildingId);
    }

    public async Task UnsubscribeFromBuilding(int buildingId)
    {
        var groupName = $"Building_{buildingId}";
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        _logger.LogInformation("Client unsubscribed from building: ConnectionId={ConnectionId}, BuildingId={BuildingId}", 
            Context.ConnectionId, buildingId);
    }

    public async Task SendAlertToAll(DashboardAlert alert)
    {
        await Clients.All.SendAsync("ReceiveAlert", alert);
        _logger.LogInformation("Alert broadcast to all clients: AlertId={AlertId}", alert.AlertId);
    }

    public async Task SendAlertToBuilding(int buildingId, DashboardAlert alert)
    {
        var groupName = $"Building_{buildingId}";
        await Clients.Group(groupName).SendAsync("ReceiveAlert", alert);
        _logger.LogInformation("Alert broadcast to building: BuildingId={BuildingId}, AlertId={AlertId}", 
            buildingId, alert.AlertId);
    }
}
