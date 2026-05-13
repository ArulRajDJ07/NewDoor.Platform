using Microsoft.AspNetCore.Mvc;

namespace NewDoor.Workflow.Orchestrator.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly ILogger<HealthController> _logger;

    public HealthController(ILogger<HealthController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            Status = "Healthy",
            Service = "Workflow.Orchestrator",
            Timestamp = DateTime.UtcNow
        });
    }
}
