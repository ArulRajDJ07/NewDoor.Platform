using NewDoor.DeviceSimulator.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace NewDoor.DeviceSimulator.Services;

public class KafkaProducerService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<KafkaProducerService> _logger;
    private readonly string _gatewayEndpoint;

    public KafkaProducerService(IConfiguration configuration, ILogger<KafkaProducerService> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _gatewayEndpoint = configuration["Gateway:TelemetryEndpoint"] ?? "/api/telemetry/ingest";

        var gatewayBaseUrl = configuration["Gateway:BaseUrl"] ?? "https://newdoor-devicehub.azurewebsites.net";
        _httpClient = httpClientFactory.CreateClient();
        _httpClient.BaseAddress = new Uri(gatewayBaseUrl);

        _logger.LogInformation("Gateway HTTP client initialized: {BaseUrl}{Endpoint}", gatewayBaseUrl, _gatewayEndpoint);
    }

    public async Task PublishTelemetryAsync(DeviceTelemetryPayload payload)
    {
        try
        {
            _logger.LogDebug("Posting telemetry to Gateway: DeviceId={DeviceId}, EventType={EventType}", 
                payload.DeviceId, payload.EventType);

            var response = await _httpClient.PostAsJsonAsync(_gatewayEndpoint, payload);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError(
                    "Gateway returned error status {StatusCode} for device {DeviceId}. Response: {ErrorContent}",
                    response.StatusCode, payload.DeviceId, errorContent);
                return;
            }

            var result = await response.Content.ReadFromJsonAsync<GatewayResponse>();
            _logger.LogDebug("Gateway accepted telemetry: EventId={EventId}, Status={Status}", 
                result?.EventId, result?.Status);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error posting telemetry to Gateway for device {DeviceId}", payload.DeviceId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish telemetry for device {DeviceId}", payload.DeviceId);
        }
    }

    public async Task PublishBatchAsync(List<DeviceTelemetryPayload> telemetryBatch)
    {
        var tasks = telemetryBatch.Select(t => PublishTelemetryAsync(t));
        await Task.WhenAll(tasks);
    }
}

public class GatewayResponse
{
    public string EventId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}
