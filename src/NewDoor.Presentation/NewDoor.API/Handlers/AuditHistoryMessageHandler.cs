using NewDoor.EventBus.Consumers;
using Microsoft.AspNetCore.SignalR;
using MediatR;
using NewDoor.API.Hubs;
using NewDoor.API.Models;
using NewDoor.API.Features.EventsHistorys.Command;
using NewDoor.API.Features.Devices.Query;
using NewDoor.API.Features.Events.Command;
using NewDoor.Platform.DTO.Features.EventsHistorys.Models;
using NewDoor.Platform.DTO.Features.Devices.Models;
using NewDoor.Platform.DTO.Features.Events.Models;

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
            // 1. Create Event record first
            var eventResponse = await StoreEventAsync(mediator, message, cancellationToken);

            // 2. Create EventsHistory with the Event.Id
            var eventsHistoryResponse = await StoreEventsHistoryAsync(mediator, message, eventResponse.Id, cancellationToken);

            // 3. Broadcast to UI via SignalR
            await BroadcastAuditToUIAsync(hubContext, eventsHistoryResponse, message, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling audit.history for CorrelationId: {CorrelationId}", message.CorrelationId);
            throw;
        }
    }
    #endregion

    #region Private Methods
    private async Task<EventResponse> StoreEventAsync(IMediator mediator, AuditHistoryEvent message, CancellationToken cancellationToken)
    {
        var addEventRequest = new AddEventRequest
        {
            EventId = message.EventIdGuid, // Use the GUID from audit event
            DeviceId = message.DeviceId, // Now storing string DeviceId directly from telemetry
            BuildingId = message.BuildingId,
            EventType = message.EventType,
            Temperature = message.Temperature,
            SmokeLevel = message.SmokeLevel,
            BatteryLevel = message.BatteryLevel,
            SignalStrength = message.SignalStrength,
            Payload = System.Text.Json.JsonSerializer.Serialize(message.Metadata),
            Severity = message.Severity,
            EventUtc = message.EventUtc,
            CorrelationId = message.CorrelationId
        };

        var command = new AddEventCommand(addEventRequest);
        var result = await mediator.Send(command, cancellationToken);

        return result;
    }

    private async Task<EventsHistoryResponse> StoreEventsHistoryAsync(IMediator mediator, AuditHistoryEvent message, int eventId, CancellationToken cancellationToken)
    {
        // Lookup Device by DeviceId string to get integer PK
        var devicePkId = await GetDeviceIdByDeviceIdentifierAsync(mediator, message.DeviceId, cancellationToken);

        var addEventsHistoryRequest = new AddEventsHistoryRequest
        {
            EventId = eventId, // Use the Event.Id we just created
            DeviceId = devicePkId,
            EventType = message.EventType,
            Severity = message.Severity,
            ProcessingResult = message.ProcessingResult,
            ProcessorName = message.ProcessorName,
            Remarks = message.Remarks,
            ProcessedUtc = message.ProcessedUtc
        };

        var command = new AddEventsHistoryCommand(addEventsHistoryRequest);
        var result = await mediator.Send(command, cancellationToken);

        return result;
    }

    private async Task<int> GetDeviceIdByDeviceIdentifierAsync(IMediator mediator, string deviceId, CancellationToken cancellationToken)
    {
        var filter = new DeviceFilterRequest
        {
            DeviceId = deviceId
        };

        var query = new FindAllDeviceQuery(filter);
        var devices = await mediator.Send(query, cancellationToken);

        if (devices == null || devices.Count == 0)
        {
            _logger.LogWarning("Device not found with DeviceId: {DeviceId}", deviceId);
            throw new InvalidOperationException($"Device not found with DeviceId: {deviceId}");
        }

        return devices.First().Id;
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
    }
    #endregion
}
