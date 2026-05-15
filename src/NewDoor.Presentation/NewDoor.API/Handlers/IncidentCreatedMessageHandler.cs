using NewDoor.EventBus.Consumers;
using Microsoft.AspNetCore.SignalR;
using MediatR;
using NewDoor.API.Hubs;
using NewDoor.API.Models;
using NewDoor.API.Features.Incidents.Command;
using NewDoor.Platform.DTO.Features.Incidents.Models;
using NewDoor.API.Repositories.Interface;

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
            // 1. Store in database using CQRS command (with fallback inside)
            var incidentResponse = await StoreIncidentAsync(mediator, message, cancellationToken);

            // 2. Broadcast to UI via SignalR
            await BroadcastIncidentToUIAsync(hubContext, incidentResponse, message, cancellationToken);
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
        try
        {
            // Check if incident already exists
            using var scope = _serviceScopeFactory.CreateScope();
            var incidentRepository = scope.ServiceProvider.GetRequiredService<IIncidentRepository>();

            var existingIncident = await incidentRepository.GetByIncidentCodeAsync(message.IncidentCode);

            if (existingIncident != null)
            {
                return new IncidentResponse
                {
                    Id = existingIncident.Id,
                    IncidentCode = existingIncident.IncidentCode,
                    BuildingId = existingIncident.BuildingId,
                    DeviceId = existingIncident.DeviceId,
                    IncidentType = existingIncident.IncidentType,
                    Severity = existingIncident.Severity,
                    Status = existingIncident.Status,
                    StartedUtc = existingIncident.StartedUtc,
                    Summary = existingIncident.Summary,
                    RootCause = existingIncident.RootCause
                };
            }

            var addIncidentRequest = new AddIncidentRequest
            {
                IncidentCode = message.IncidentCode,
                BuildingId = message.BuildingId,
                DeviceId = message.DeviceId,
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
            return await mediator.Send(command, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DB error for incident {Code} - using fallback", message.IncidentCode);

            return new IncidentResponse
            {
                IncidentCode = message.IncidentCode,
                BuildingId = message.BuildingId,
                DeviceId = message.DeviceId,
                IncidentType = message.IncidentType,
                Severity = message.Severity,
                Status = message.Status,
                StartedUtc = message.StartedUtc,
                Summary = message.Summary,
                RootCause = message.RootCause
            };
        }
    }

    private async Task BroadcastIncidentToUIAsync(IHubContext<NotificationHub> hubContext, IncidentResponse incident, IncidentCreatedEvent message, CancellationToken cancellationToken)
    {
        var floor = message.TelemetryData.TryGetValue("Floor", out var floorValue) 
            ? floorValue?.ToString() ?? "Unknown" 
            : "Unknown";
        var zone = message.TelemetryData.TryGetValue("Zone", out var zoneValue) 
            ? zoneValue?.ToString() ?? "Unknown" 
            : "Unknown";
        var ruleName = message.TelemetryData.TryGetValue("RuleName", out var ruleValue)
            ? ruleValue?.ToString() ?? "N/A"
            : "N/A";

        // Create a meaningful message if Summary is empty
        var displayMessage = !string.IsNullOrWhiteSpace(incident.Summary) 
            ? incident.Summary 
            : $"{incident.IncidentType} detected - {incident.Severity} severity";

        var dashboardAlert = new DashboardAlert
        {
            AlertId = incident.IncidentCode,
            DeviceId = message.DeviceId ?? "Unknown",
            DeviceName = message.DeviceName ?? "Unknown Device",
            BuildingCode = message.BuildingCode ?? "Unknown",
            Location = $"{floor} / {zone}",
            Severity = incident.Severity,
            Message = displayMessage,
            Timestamp = incident.StartedUtc,
            AdditionalData = new Dictionary<string, object>
            {
                { "IncidentId", incident.Id },
                { "IncidentType", incident.IncidentType },
                { "Status", incident.Status },
                { "RootCause", incident.RootCause ?? "Under investigation" },
                { "TriggeredByRule", message.TriggeredByRule },
                { "RuleName", ruleName },
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
    }
    #endregion
}
