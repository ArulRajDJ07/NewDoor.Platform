using NewDoor.Gateway.Api.Models;

namespace NewDoor.Gateway.Api.Services;

public interface IDeviceEnrichmentService
{
    Task<EnrichedTelemetryEvent> EnrichTelemetryAsync(DeviceTelemetryRequest request);
}

public class DeviceEnrichmentService : IDeviceEnrichmentService
{
    private readonly ILogger<DeviceEnrichmentService> _logger;
    private readonly Dictionary<string, DeviceMetadata> _deviceCache = new();

    public DeviceEnrichmentService(ILogger<DeviceEnrichmentService> logger)
    {
        _logger = logger;
    }

    public Task<EnrichedTelemetryEvent> EnrichTelemetryAsync(DeviceTelemetryRequest request)
    {
        // Simple enrichment - no external API calls, no database
        var deviceMetadata = GetDeviceMetadataFromCache(request.DeviceId);

        var enrichedEvent = new EnrichedTelemetryEvent
        {
            EventId = Guid.NewGuid().ToString(),
            CorrelationId = Guid.NewGuid().ToString(),
            EventName = "DeviceTelemetryReceived",
            EventType = request.EventType,
            DeviceId = request.DeviceId,
            DeviceName = deviceMetadata.DeviceName,
            DeviceType = deviceMetadata.DeviceType,
            BuildingId = deviceMetadata.BuildingId,
            BuildingCode = deviceMetadata.BuildingCode,
            Floor = deviceMetadata.Floor,
            Zone = deviceMetadata.Zone,
            TimestampUtc = request.TimestampUtc,
            Payload = new TelemetryPayload
            {
                Temperature = request.Temperature ?? 0,
                SmokeLevel = request.SmokeLevel ?? 0,
                BatteryLevel = request.BatteryLevel,
                SignalStrength = request.SignalStrength ?? "Unknown",
                Status = request.Status
            },
            Metadata = new TelemetryMetadata
            {
                Source = "NewDoor.Gateway.Api",
                GeneratedUtc = DateTime.UtcNow
            }
        };

        return Task.FromResult(enrichedEvent);
    }

    private DeviceMetadata GetDeviceMetadataFromCache(string deviceId)
    {
        // Return cached or create default metadata (no external API call)
        if (_deviceCache.TryGetValue(deviceId, out var cached))
        {
            return cached;
        }

        // Create default metadata and cache it
        var metadata = new DeviceMetadata
        {
            DeviceName = $"Device-{deviceId}",
            DeviceType = "SmokeDetector",
            BuildingId = 1,
            BuildingCode = "BLD-0001",
            Floor = "1",
            Zone = "Default"
        };

        _deviceCache[deviceId] = metadata;
        _logger.LogDebug("Created default metadata for device {DeviceId}", deviceId);

        return metadata;
    }
}

public class DeviceMetadata
{
    public string DeviceName { get; set; } = string.Empty;
    public string DeviceType { get; set; } = string.Empty;
    public int BuildingId { get; set; }
    public string BuildingCode { get; set; } = string.Empty;
    public string Floor { get; set; } = string.Empty;
    public string Zone { get; set; } = string.Empty;
}
