using NewDoor.Gateway.Api.Models;
using System.Collections.Concurrent;

namespace NewDoor.Gateway.Api.Services;

public interface IDeviceEnrichmentService
{
    Task<EnrichedTelemetryEvent> EnrichTelemetryAsync(DeviceTelemetryRequest request);
}

public class DeviceEnrichmentService : IDeviceEnrichmentService
{
    private readonly ILogger<DeviceEnrichmentService> _logger;
    private readonly HttpClient _httpClient;
    private readonly ConcurrentDictionary<string, DeviceMetadata> _deviceCache = new();
    private readonly IConfiguration _configuration;

    public DeviceEnrichmentService(
        ILogger<DeviceEnrichmentService> logger,
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _logger = logger;
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<EnrichedTelemetryEvent> EnrichTelemetryAsync(DeviceTelemetryRequest request)
    {
        // Fetch device metadata from API or cache
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
        // Check cache first
        if (_deviceCache.TryGetValue(deviceId, out var cached))
        {
            return cached;
        }

        try
        {
            // Fetch from NewDoor API using GetById endpoint
            var apiBaseUrl = _configuration["ApiSettings:NewDoorApiBaseUrl"] ?? "https://localhost:7192/";
            var response = await _httpClient.GetAsync($"{apiBaseUrl}api/Device/GetById?id={deviceId}");

            if (response.IsSuccessStatusCode)
            {
                var deviceResponse = await response.Content.ReadFromJsonAsync<DeviceApiResponse>();

                if (deviceResponse != null)
                {
                    var metadata = new DeviceMetadata
                    {
                        DeviceName = deviceResponse.DeviceName ?? $"Device-{deviceId}",
                        DeviceType = deviceResponse.DeviceType ?? "SmokeSensor",
                        BuildingId = deviceResponse.BuildingId,
                        BuildingCode = $"BLD-{deviceResponse.BuildingId:D4}", // Generate from BuildingId
                        Floor = deviceResponse.Floor ?? "1",
                        Zone = deviceResponse.Zone ?? "Default"
                    };

                    // Cache for future requests
                    _deviceCache[deviceId] = metadata;
                    _logger.LogInformation("Fetched and cached metadata for device {DeviceId}: Type={DeviceType}, Building={BuildingId}, Floor={Floor}", 
                        deviceId, metadata.DeviceType, metadata.BuildingId, metadata.Floor);

                    return metadata;
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Device {DeviceId} not found in API. Using defaults.", deviceId);
            }
            else
            {
                _logger.LogWarning("Failed to fetch device metadata for {DeviceId} from API. Status: {StatusCode}. Using defaults.", 
                    deviceId, response.StatusCode);
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error fetching device metadata for {DeviceId}. API may be unavailable. Using defaults.", deviceId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching device metadata for {DeviceId} from API. Using defaults.", deviceId);
        }

        // Fallback: Create and cache default metadata
        var defaultMetadata = new DeviceMetadata
        {
            DeviceName = $"Device-{deviceId}",
            DeviceType = "SmokeSensor",
            BuildingId = 1,
            BuildingCode = "BLD-0001",
            Floor = "1",
            Zone = "Default"
        };

        _deviceCache[deviceId] = defaultMetadata;
        _logger.LogWarning("Created default metadata for device {DeviceId} (API fetch failed or device not found)", deviceId);

        return defaultMetadata;
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

public class DeviceApiResponse
{
    public string DeviceId { get; set; } = string.Empty;
    public string DeviceName { get; set; } = string.Empty;
    public string DeviceType { get; set; } = string.Empty;
    public int BuildingId { get; set; }
    public string Floor { get; set; } = string.Empty;
    public string Zone { get; set; } = string.Empty;
}
