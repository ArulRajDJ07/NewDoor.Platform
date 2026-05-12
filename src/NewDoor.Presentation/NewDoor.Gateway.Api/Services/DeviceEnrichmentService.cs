using NewDoor.Gateway.Api.Models;

namespace NewDoor.Gateway.Api.Services;

public interface IDeviceEnrichmentService
{
    Task<EnrichedTelemetryEvent> EnrichTelemetryAsync(DeviceTelemetryRequest request);
}

public class DeviceEnrichmentService : IDeviceEnrichmentService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<DeviceEnrichmentService> _logger;
    private readonly Dictionary<string, DeviceMetadata> _deviceCache = new();

    public DeviceEnrichmentService(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<DeviceEnrichmentService> logger)
    {
        _logger = logger;
        var apiBaseUrl = configuration["InternalServices:NewDoorApi"] ?? "https://newdoor-api.azurewebsites.net";
        _httpClient = httpClientFactory.CreateClient();
        _httpClient.BaseAddress = new Uri(apiBaseUrl);
    }

    public async Task<EnrichedTelemetryEvent> EnrichTelemetryAsync(DeviceTelemetryRequest request)
    {
        var deviceMetadata = await GetDeviceMetadataAsync(request.DeviceId);

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

        return enrichedEvent;
    }

    private async Task<DeviceMetadata> GetDeviceMetadataAsync(string deviceId)
    {
        if (_deviceCache.TryGetValue(deviceId, out var cached))
        {
            return cached;
        }

        try
        {
            var devices = await _httpClient.GetFromJsonAsync<List<DeviceDto>>("/api/device/getall");
            var device = devices?.FirstOrDefault(d => d.DeviceId == deviceId);

            if (device != null)
            {
                var metadata = new DeviceMetadata
                {
                    DeviceName = device.DeviceName,
                    DeviceType = device.DeviceType,
                    BuildingId = device.BuildingId,
                    BuildingCode = $"BLD-{device.BuildingId:D4}",
                    Floor = device.Floor,
                    Zone = device.Zone
                };

                _deviceCache[deviceId] = metadata;
                return metadata;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch device metadata for {DeviceId}", deviceId);
        }

        return new DeviceMetadata
        {
            DeviceName = $"Device-{deviceId}",
            DeviceType = "Unknown",
            BuildingId = 0,
            BuildingCode = "UNKNOWN",
            Floor = "Unknown",
            Zone = "Unknown"
        };
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

public class DeviceDto
{
    public int Id { get; set; }
    public string DeviceId { get; set; } = string.Empty;
    public string DeviceName { get; set; } = string.Empty;
    public string DeviceType { get; set; } = string.Empty;
    public int BuildingId { get; set; }
    public string Floor { get; set; } = string.Empty;
    public string Zone { get; set; } = string.Empty;
}
