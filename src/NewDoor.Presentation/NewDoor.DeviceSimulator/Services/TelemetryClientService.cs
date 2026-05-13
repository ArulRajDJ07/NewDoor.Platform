using NewDoor.DeviceSimulator.Models;
using System.Net.Http.Json;

namespace NewDoor.DeviceSimulator.Services;

public class TelemetryClientService : IAsyncDisposable
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<TelemetryClientService> _logger;
    private readonly string _gatewayUrl;

    public TelemetryClientService(IConfiguration configuration, IHttpClientFactory httpClientFactory, ILogger<TelemetryClientService> logger)
    {
        _logger = logger;
        _gatewayUrl = configuration["Gateway:ApiUrl"] ?? "https://newdoor-devicehub.azurewebsites.net";
        _httpClient = httpClientFactory.CreateClient();
        _httpClient.BaseAddress = new Uri(_gatewayUrl);
        _httpClient.Timeout = TimeSpan.FromSeconds(30);
    }

    public async Task PublishTelemetryAsync(DeviceTelemetryPayload payload)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/Telemetry/ingest", payload);
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning("Failed to publish telemetry for device {DeviceId}: {StatusCode}", 
                payload.DeviceId, ex.StatusCode);
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Failed to publish telemetry for device {DeviceId}: {Message}", 
                payload.DeviceId, ex.Message);
        }
    }

    public async Task PublishBatchAsync(List<DeviceTelemetryPayload> telemetryBatch)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/Telemetry/ingest/batch", telemetryBatch);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Failed to publish batch of {Count} telemetry messages: {Message}", 
                telemetryBatch.Count, ex.Message);
            throw;
        }
    }

    public async ValueTask DisposeAsync()
    {
        _httpClient?.Dispose();
        await Task.CompletedTask;
    }
}
