using Microsoft.AspNetCore.Mvc;
using NewDoor.Processor.Runtime.Models;
using NewDoor.Processor.Runtime.Services;

namespace NewDoor.Processor.Runtime.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProcessorController : ControllerBase
{
    private readonly IEventProcessorService _processorService;
    private readonly ILogger<ProcessorController> _logger;

    public ProcessorController(IEventProcessorService processorService, ILogger<ProcessorController> logger)
    {
        _processorService = processorService;
        _logger = logger;
    }

    [HttpPost("process")]
    public async Task<ActionResult<ProcessorResponse>> Process(
        [FromBody] ProcessorRequest request, 
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Received processing request: RequestId={RequestId}, EventId={EventId}", 
                request.RequestId, request.Event.EventId);

            var response = await _processorService.ProcessAsync(request, cancellationToken);

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing request: RequestId={RequestId}", request.RequestId);
            return StatusCode(500, new { Error = "Internal server error", Message = ex.Message });
        }
    }

    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new { Status = "Healthy", Service = "Processor.Runtime", Timestamp = DateTime.UtcNow });
    }
}
