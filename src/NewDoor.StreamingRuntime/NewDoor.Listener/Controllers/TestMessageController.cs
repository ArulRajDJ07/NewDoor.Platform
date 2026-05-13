using Microsoft.AspNetCore.Mvc;
using NewDoor.EventBus.Consumers;
using NewDoor.Listener.Models;

namespace NewDoor.Listener.Controllers;

/// <summary>
/// Test controller for local development on ARM64 - bypasses Kafka and routes directly to message handler
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TestMessageController : ControllerBase
{
    private readonly IKafkaMessageHandler<EnrichedTelemetryEvent> _messageHandler;
    private readonly ILogger<TestMessageController> _logger;
    private readonly IWebHostEnvironment _environment;

    public TestMessageController(
        IKafkaMessageHandler<EnrichedTelemetryEvent> messageHandler,
        ILogger<TestMessageController> logger,
        IWebHostEnvironment environment)
    {
        _messageHandler = messageHandler;
        _logger = logger;
        _environment = environment;
    }

    /// <summary>
    /// POST telemetry event for local testing (bypasses Kafka)
    /// </summary>
    [HttpPost("telemetry")]
    public async Task<IActionResult> PostTelemetryEvent(
        [FromBody] EnrichedTelemetryEvent telemetryEvent,
        CancellationToken cancellationToken)
    {
        // Only allow in Development environment for security
        if (!_environment.IsDevelopment())
        {
            return NotFound();
        }

        try
        {
            _logger.LogInformation(
                "Test endpoint received telemetry event: DeviceId={DeviceId}, EventId={EventId}",
                telemetryEvent.DeviceId,
                telemetryEvent.EventId);

            // Route directly to the Kafka message handler (bypassing Kafka)
            var key = telemetryEvent.DeviceId;
            await _messageHandler.HandleAsync(key, telemetryEvent, cancellationToken);

            _logger.LogInformation(
                "Test telemetry event processed successfully: DeviceId={DeviceId}",
                telemetryEvent.DeviceId);

            return Ok(new
            {
                success = true,
                message = "Telemetry event processed (test mode - bypassed Kafka)",
                eventId = telemetryEvent.EventId,
                deviceId = telemetryEvent.DeviceId,
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing test telemetry event");
            return StatusCode(500, new
            {
                success = false,
                message = "Error processing telemetry event",
                error = ex.Message
            });
        }
    }

    /// <summary>
    /// Generate sample telemetry events for testing
    /// </summary>
    [HttpPost("generate-sample")]
    public async Task<IActionResult> GenerateSampleEvent(CancellationToken cancellationToken)
    {
        if (!_environment.IsDevelopment())
        {
            return NotFound();
        }

        var sampleEvent = new EnrichedTelemetryEvent
        {
            EventId = Guid.NewGuid().ToString(),
            CorrelationId = Guid.NewGuid().ToString(),
            EventName = "DeviceTelemetryReceived",
            EventType = "IoT.Telemetry",
            DeviceId = $"DEVICE-{Random.Shared.Next(1000, 9999)}",
            DeviceName = "Test Smoke Detector",
            DeviceType = "SmokeDetector",
            BuildingId = 1001,
            BuildingCode = "BLD-001",
            Floor = "Floor-3",
            Zone = "Zone-A",
            TimestampUtc = DateTime.UtcNow,
            Payload = new TelemetryPayload
            {
                Temperature = Random.Shared.Next(18, 30) + Random.Shared.NextDouble(),
                SmokeLevel = Random.Shared.Next(0, 100) + Random.Shared.NextDouble(),
                BatteryLevel = Random.Shared.Next(50, 100),
                SignalStrength = "Strong",
                Status = "Active"
            },
            Metadata = new TelemetryMetadata
            {
                Source = "NewDoor.Listener.TestController",
                GeneratedUtc = DateTime.UtcNow
            }
        };

        _logger.LogInformation(
            "Generating sample telemetry event: DeviceId={DeviceId}",
            sampleEvent.DeviceId);

        await _messageHandler.HandleAsync(sampleEvent.DeviceId, sampleEvent, cancellationToken);

        return Ok(new
        {
            success = true,
            message = "Sample event generated and processed",
            eventData = sampleEvent
        });
    }

    /// <summary>
    /// Generate high-temperature event to trigger incident detection
    /// </summary>
    [HttpPost("generate-incident")]
    public async Task<IActionResult> GenerateIncidentEvent(CancellationToken cancellationToken)
    {
        if (!_environment.IsDevelopment())
        {
            return NotFound();
        }

        var incidentEvent = new EnrichedTelemetryEvent
        {
            EventId = Guid.NewGuid().ToString(),
            CorrelationId = Guid.NewGuid().ToString(),
            EventName = "DeviceTelemetryReceived",
            EventType = "IoT.Telemetry",
            DeviceId = $"DEVICE-INCIDENT-{Random.Shared.Next(1000, 9999)}",
            DeviceName = "Test Smoke Detector (High Temp)",
            DeviceType = "SmokeDetector",
            BuildingId = 1001,
            BuildingCode = "BLD-001",
            Floor = "Floor-5",
            Zone = "Zone-B",
            TimestampUtc = DateTime.UtcNow,
            Payload = new TelemetryPayload
            {
                Temperature = 85.0 + Random.Shared.Next(0, 15), // High temp to trigger incident
                SmokeLevel = 75.0 + Random.Shared.Next(0, 20),   // High smoke level
                BatteryLevel = Random.Shared.Next(50, 100),
                SignalStrength = "Strong",
                Status = "Alert"
            },
            Metadata = new TelemetryMetadata
            {
                Source = "NewDoor.Listener.TestController",
                GeneratedUtc = DateTime.UtcNow
            }
        };

        _logger.LogWarning(
            "Generating INCIDENT telemetry event: DeviceId={DeviceId}, Temp={Temperature}°C",
            incidentEvent.DeviceId,
            incidentEvent.Payload.Temperature);

        await _messageHandler.HandleAsync(incidentEvent.DeviceId, incidentEvent, cancellationToken);

        return Ok(new
        {
            success = true,
            message = "Incident event generated and processed - check incident detection",
            eventData = incidentEvent,
            warning = "High temperature/smoke levels - should trigger incident detection"
        });
    }
}
