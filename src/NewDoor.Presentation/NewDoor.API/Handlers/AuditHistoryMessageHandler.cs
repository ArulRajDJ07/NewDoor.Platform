using NewDoor.EventBus.Consumers;
using Microsoft.AspNetCore.SignalR;
using MediatR;
using NewDoor.API.Hubs;
using NewDoor.API.Models;
using NewDoor.API.Features.EventsHistorys.Command;
using NewDoor.Platform.DTO.Features.EventsHistorys.Models;

namespace NewDoor.API.Handlers;

public class AuditHistoryMessageHandler : IKafkaMessageHandler<AuditHistoryEvent>
{
    #region Fields
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<AuditHistoryMessageHandler> _logger;
    #endregion

    #region Constructor
    public AuditHistoryMessageHandler(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<AuditHistoryMessageHandler> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }
    #endregion

    #region Handler
    public async Task HandleAsync(string key, AuditHistoryEvent message, CancellationToken cancellationToken)
    {
        // Create a new scope for this message to resolve scoped dependencies
        using var scope = _serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<NotificationHub>>();

        try
        {
            _logger.LogInformation("Received audit.history event for EventId: {EventId}", message.EventId);

            // 1. Store in database using CQRS command
            var eventsHistoryResponse = await StoreEventsHistoryAsync(mediator, message, cancellationToken);

            // 2. Broadcast to UI via SignalR (optional for audit trail visibility)
            await BroadcastAuditToUIAsync(hubContext, eventsHistoryResponse, message, cancellationToken);

            _logger.LogInformation("EventsHistory stored and broadcasted: Id={Id}", eventsHistoryResponse.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling audit.history for EventId: {EventId}", message.EventId);
            throw;
        }
    }
    #endregion

    #region Private Methods
    private async Task<EventsHistoryResponse> StoreEventsHistoryAsync(IMediator mediator, AuditHistoryEvent message, CancellationToken cancellationToken)
    {
        var addEventsHistoryRequest = new AddEventsHistoryRequest
        {
            EventId = message.EventId,
            DeviceId = message.DeviceId,
            EventType = message.EventType,
            Severity = message.Severity,
            ProcessingResult = message.ProcessingResult,
            ProcessorName = message.ProcessorName,
            Remarks = message.Remarks,
            ProcessedUtc = message.ProcessedUtc
        };

        var command = new AddEventsHistoryCommand(addEventsHistoryRequest);
        var result = await mediator.Send(command, cancellationToken);

        _logger.LogInformation("EventsHistory stored in database: Id={Id}, EventId={EventId}", 
            result.Id, result.EventId);

        return result;
    }

    private async Task BroadcastAuditToUIAsync(IHubContext<NotificationHub> hubContext, EventsHistoryResponse eventsHistory, AuditHistoryEvent message, CancellationToken cancellationToken)
    {
        var auditData = new
        {
            AuditId = eventsHistory.Id,
            CorrelationId = message.CorrelationId,
            EventId = eventsHistory.EventId,
            DeviceId = eventsHistory.DeviceId,
            EventType = eventsHistory.EventType,
            Severity = eventsHistory.Severity,
            ProcessingResult = eventsHistory.ProcessingResult,
            ProcessorName = eventsHistory.ProcessorName,
            Remarks = eventsHistory.Remarks,
            ProcessedUtc = eventsHistory.ProcessedUtc
        };

        // Broadcast audit trail to all admin/monitoring clients
        await hubContext.Clients.All.SendAsync("ReceiveAuditHistory", auditData, cancellationToken);

        _logger.LogInformation("Audit history broadcasted to UI: Id={Id}", eventsHistory.Id);
    }
    #endregion
}
