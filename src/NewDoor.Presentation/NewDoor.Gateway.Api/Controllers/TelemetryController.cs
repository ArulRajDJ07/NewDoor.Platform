using Microsoft.AspNetCore.Mvc;
using NewDoor.EventBus.Producers;
using NewDoor.Gateway.Api.Models;
using NewDoor.Gateway.Api.Services;

namespace NewDoor.Gateway.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TelemetryController : ControllerBase
{
    private readonly IKafkaProducer _kafkaProducer;
    private readonly IDeviceEnrichmentService _enrichmentService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<TelemetryController> _logger;

    public TelemetryController(
        IKafkaProducer kafkaProducer,
        IDeviceEnrichmentService enrichmentService,
        IConfiguration configuration,
        ILogger<TelemetryController> logger)
    {
        _kafkaProducer = kafkaProducer;
        _enrichmentService = enrichmentService;
        _configuration = configuration;
        _logger = logger;
    }

    [HttpPost("ingest")]
    public async Task<IActionResult> IngestTelemetry([FromBody] DeviceTelemetryRequest request)
    {
        try
        {
            _logger.LogInformation("Received telemetry: DeviceId={DeviceId}, EventType={EventType}", 
                request.DeviceId, request.EventType);

            var enrichedEvent = await _enrichmentService.EnrichTelemetryAsync(request);

            var topic = _configuration["Kafka:TelemetryTopic"] ?? "newdoor.device.telemetry";
            await _kafkaProducer.PublishAsync(topic, request.DeviceId, enrichedEvent);

            _logger.LogInformation("Published enriched telemetry to Kafka: EventId={EventId}, Topic={Topic}", 
                enrichedEvent.EventId, topic);

            return Accepted(new { eventId = enrichedEvent.EventId, status = "queued" });
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("platform") || ex.Message.Contains("librdkafka"))
        {
            _logger.LogError(ex, "Platform compatibility error for DeviceId={DeviceId}. Kafka unavailable on this platform.", request.DeviceId);
            return StatusCode(503, new 
            { 
                error = "Kafka service unavailable on this platform",
                detail = "The application is running on an unsupported platform. Please rebuild with x64 target or deploy to a supported platform.",
                deviceId = request.DeviceId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process telemetry for DeviceId={DeviceId}", request.DeviceId);
            return StatusCode(500, new { error = "Failed to process telemetry", deviceId = request.DeviceId });
        }
    }

    [HttpPost("ingest/batch")]
    public async Task<IActionResult> IngestBatchTelemetry([FromBody] List<DeviceTelemetryRequest> requests)
    {
        try
        {
            _logger.LogInformation("Received batch telemetry: Count={Count}", requests.Count);

            var enrichedEvents = new List<(string Key, EnrichedTelemetryEvent Message)>();

            foreach (var request in requests)
            {
                var enrichedEvent = await _enrichmentService.EnrichTelemetryAsync(request);
                enrichedEvents.Add((request.DeviceId, enrichedEvent));
            }

            var topic = _configuration["Kafka:TelemetryTopic"] ?? "newdoor.device.telemetry";
            await _kafkaProducer.PublishBatchAsync(topic, enrichedEvents);

            _logger.LogInformation("Published batch telemetry to Kafka: Count={Count}, Topic={Topic}", 
                enrichedEvents.Count, topic);

            return Accepted(new { count = enrichedEvents.Count, status = "queued" });
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("platform") || ex.Message.Contains("librdkafka"))
        {
            _logger.LogError(ex, "Platform compatibility error for batch telemetry. Kafka unavailable on this platform.");
            return StatusCode(503, new 
            { 
                error = "Kafka service unavailable on this platform",
                detail = "The application is running on an unsupported platform. Please rebuild with x64 target or deploy to a supported platform.",
                count = requests.Count
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process batch telemetry");
            return StatusCode(500, new { error = "Failed to process batch telemetry" });
        }
    }
}
