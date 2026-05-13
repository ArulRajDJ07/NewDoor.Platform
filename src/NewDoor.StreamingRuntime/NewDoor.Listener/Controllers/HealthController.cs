using Microsoft.AspNetCore.Mvc;

namespace NewDoor.Listener.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly ILogger<HealthController> _logger;
    private readonly IConfiguration _configuration;

    public HealthController(ILogger<HealthController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    [HttpGet]
    public IActionResult GetHealth()
    {
        return Ok(new
        {
            status = "Healthy",
            service = "NewDoor.Listener",
            timestamp = DateTime.UtcNow,
            version = "1.0.0"
        });
    }

    [HttpGet("config")]
    public IActionResult GetConfiguration()
    {
        var kafkaConfig = new
        {
            bootstrapServers = _configuration["Kafka:BootstrapServers"],
            groupId = _configuration["Kafka:GroupId"],
            telemetryTopic = _configuration["Kafka:TelemetryTopic"],
            incidentTopic = _configuration["Kafka:IncidentTopic"],
            messageTimeoutMs = _configuration["Kafka:MessageTimeoutMs"],
            requestTimeoutMs = _configuration["Kafka:RequestTimeoutMs"]
        };

        return Ok(new
        {
            service = "NewDoor.Listener",
            kafka = kafkaConfig,
            timestamp = DateTime.UtcNow
        });
    }
}
