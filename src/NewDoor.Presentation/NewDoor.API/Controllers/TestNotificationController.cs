using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using NewDoor.API.Hubs;
using NewDoor.API.Models;

namespace NewDoor.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestNotificationController : ControllerBase
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly ILogger<TestNotificationController> _logger;

    public TestNotificationController(
        IHubContext<NotificationHub> hubContext,
        ILogger<TestNotificationController> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    /// <summary>
    /// Sends a test alarm notification to all connected clients
    /// </summary>
    [HttpPost("send-test-alarm")]
    public async Task<IActionResult> SendTestAlarm()
    {
        try
        {
            var testAlarm = new DashboardAlert
            {
                AlertId = Guid.NewGuid().ToString(),
                DeviceId = $"TEST-{Random.Shared.Next(100, 999)}",
                DeviceName = $"Test Device {Random.Shared.Next(1, 10)}",
                BuildingCode = $"Building {(char)('A' + Random.Shared.Next(0, 3))}",
                Location = $"Floor {Random.Shared.Next(1, 5)} / Zone {Random.Shared.Next(1, 10)}",
                Severity = GetRandomSeverity(),
                Message = GetRandomMessage(),
                Timestamp = DateTime.UtcNow,
                AdditionalData = new Dictionary<string, object>
                {
                    ["Temperature"] = Random.Shared.Next(20, 35),
                    ["SmokeLevel"] = Random.Shared.Next(0, 100),
                    ["IsTest"] = true
                }
            };

            await _hubContext.Clients.All.SendAsync("ReceiveAlert", testAlarm);

            _logger.LogInformation("Test alarm sent: {AlertId}", testAlarm.AlertId);

            return Ok(new { success = true, alert = testAlarm });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending test alarm");
            return StatusCode(500, new { success = false, error = ex.Message });
        }
    }

    /// <summary>
    /// Sends multiple test alarms
    /// </summary>
    [HttpPost("send-multiple-alarms/{count}")]
    public async Task<IActionResult> SendMultipleAlarms(int count = 5)
    {
        try
        {
            var alarms = new List<DashboardAlert>();

            for (int i = 0; i < count; i++)
            {
                var testAlarm = new DashboardAlert
                {
                    AlertId = Guid.NewGuid().ToString(),
                    DeviceId = $"TEST-{Random.Shared.Next(100, 999)}",
                    DeviceName = $"Test Device {Random.Shared.Next(1, 10)}",
                    BuildingCode = $"Building {(char)('A' + Random.Shared.Next(0, 3))}",
                    Location = $"Floor {Random.Shared.Next(1, 5)} / Zone {Random.Shared.Next(1, 10)}",
                    Severity = GetRandomSeverity(),
                    Message = GetRandomMessage(),
                    Timestamp = DateTime.UtcNow,
                    AdditionalData = new Dictionary<string, object>
                    {
                        ["Temperature"] = Random.Shared.Next(20, 35),
                        ["SmokeLevel"] = Random.Shared.Next(0, 100),
                        ["IsTest"] = true
                    }
                };

                await _hubContext.Clients.All.SendAsync("ReceiveAlert", testAlarm);
                alarms.Add(testAlarm);

                // Small delay between messages
                await Task.Delay(100);
            }

            _logger.LogInformation("Sent {Count} test alarms", count);

            return Ok(new { success = true, count = alarms.Count, alarms });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending multiple test alarms");
            return StatusCode(500, new { success = false, error = ex.Message });
        }
    }

    private string GetRandomSeverity()
    {
        var severities = new[] { "Low", "Medium", "High", "Critical" };
        return severities[Random.Shared.Next(severities.Length)];
    }

    private string GetRandomMessage()
    {
        var messages = new[]
        {
            "Temperature threshold exceeded",
            "Motion detected in restricted area",
            "Door left open",
            "Low battery warning",
            "Connection timeout",
            "Sensor malfunction detected",
            "Fire alarm triggered",
            "Smoke detected",
            "Unauthorized access attempt",
            "Power failure detected"
        };
        return messages[Random.Shared.Next(messages.Length)];
    }
}
