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
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SubscribeToBuilding(int buildingId)
    {
        var groupName = $"Building_{buildingId}";
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
    }

    public async Task UnsubscribeFromBuilding(int buildingId)
    {
        var groupName = $"Building_{buildingId}";
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
    }

    public async Task SendAlertToAll(DashboardAlert alert)
    {
        await Clients.All.SendAsync("ReceiveAlert", alert);
    }

    public async Task SendAlertToBuilding(int buildingId, DashboardAlert alert)
    {
        var groupName = $"Building_{buildingId}";
        await Clients.Group(groupName).SendAsync("ReceiveAlert", alert);
    }
}
