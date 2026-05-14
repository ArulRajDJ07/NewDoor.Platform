using NewDoor.EventBus.Consumers;
using Microsoft.AspNetCore.SignalR;
using MediatR;
using NewDoor.API.Hubs;
using NewDoor.API.Models;
using NewDoor.API.Features.Incidents.Command;
using NewDoor.Platform.DTO.Features.Incidents.Models;

namespace NewDoor.API.Handlers;

public class IncidentCreatedMessageHandler : IKafkaMessageHandler<IncidentCreatedEvent>
{
    #region Fields
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<IncidentCreatedMessageHandler> _logger;
    #endregion

    #region Constructor
    public IncidentCreatedMessageHandler(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<IncidentCreatedMessageHandler> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }
    #endregion

    #region Handler
    public async Task HandleAsync(string key, IncidentCreatedEvent message, CancellationToken cancellationToken)
    {
        // Create a new scope for this message to resolve scoped dependencies
        using var scope = _serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<NotificationHub>>();

        try
        {
            _logger.LogInformation("Received incident.created event: {IncidentCode}", message.IncidentCode);

            // 1. Store in database using CQRS command
            var incidentResponse = await StoreIncidentAsync(mediator, message, cancellationToken);

            // 2. Broadcast to UI via SignalR
            await BroadcastIncidentToUIAsync(hubContext, incidentResponse, message, cancellationToken);

            _logger.LogInformation("Incident {IncidentCode} stored and broadcasted", message.IncidentCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling incident.created for {IncidentCode}", message.IncidentCode);
            throw;
        }
    }
    #endregion

    #region Private Methods
    private async Task<IncidentResponse> StoreIncidentAsync(IMediator mediator, IncidentCreatedEvent message, CancellationToken cancellationToken)
    {
        var addIncidentRequest = new AddIncidentRequest
        {
            IncidentCode = message.IncidentCode,
            BuildingId = message.BuildingId,
            IncidentType = message.IncidentType,
            Severity = message.Severity,
            Status = message.Status,
            StartedUtc = message.StartedUtc,
            Summary = message.Summary,
            RootCause = message.RootCause,
            TriggeredByRule = message.TriggeredByRule,
            EventCount = message.EventCount
        };

        var command = new AddIncidentCommand(addIncidentRequest);
        var result = await mediator.Send(command, cancellationToken);

        _logger.LogInformation("Incident stored in database: Id={Id}, Code={Code}", result.Id, result.IncidentCode);

        return result;
    }

    private async Task BroadcastIncidentToUIAsync(IHubContext<NotificationHub> hubContext, IncidentResponse incident, IncidentCreatedEvent message, CancellationToken cancellationToken)
    {
        var floor = message.TelemetryData.TryGetValue("Floor", out var floorValue) 
            ? floorValue?.ToString() ?? "" 
            : "";
        var zone = message.TelemetryData.TryGetValue("Zone", out var zoneValue) 
            ? zoneValue?.ToString() ?? "" 
            : "";

        var dashboardAlert = new DashboardAlert
        {
            AlertId = incident.IncidentCode,
            DeviceId = message.DeviceId,
            DeviceName = message.DeviceName,
            BuildingCode = message.BuildingCode,
            Location = $"{floor} / {zone}",
            Severity = incident.Severity,
            Message = incident.Summary,
            Timestamp = incident.StartedUtc,
            AdditionalData = new Dictionary<string, object>
            {
                { "IncidentId", incident.Id },
                { "IncidentType", incident.IncidentType },
                { "Status", incident.Status },
                { "RootCause", incident.RootCause },
                { "ConfidenceScore", message.ConfidenceScore },
                { "TelemetryData", message.TelemetryData }
            }
        };

        // Broadcast to all clients
        await hubContext.Clients.All.SendAsync("ReceiveIncident", dashboardAlert, cancellationToken);

        // Broadcast to building-specific group
        if (incident.BuildingId > 0)
        {
            var groupName = $"Building_{incident.BuildingId}";
            await hubContext.Clients.Group(groupName).SendAsync("ReceiveIncident", dashboardAlert, cancellationToken);
        }

        _logger.LogInformation("Incident broadcasted to UI: {IncidentCode}", incident.IncidentCode);
    }
    #endregion
}
