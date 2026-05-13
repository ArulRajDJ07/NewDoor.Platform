using Microsoft.AspNetCore.Mvc;

namespace NewDoor.Action.Dispatcher.Controllers;

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
            Service = "Action.Dispatcher",
            Timestamp = DateTime.UtcNow
        });
    }
}
