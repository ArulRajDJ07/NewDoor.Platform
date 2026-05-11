    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using NewDoor.Platform.DTO.Features.Incidents.Models;
    using NewDoor.API.Features.Incidents.Command;
    using NewDoor.API.Features.Incidents.Query;

    [Route("api/[controller]")]
    [ApiController]
    public class IncidentController(IMediator mediator) : ControllerBase
    {
        [HttpGet("GetAll")]
        public async Task<List<IncidentResponse>> GetAll() =>
            await mediator.Send(new FindAllIncidentQuery());

        [HttpPost]
        public async Task<IncidentResponse> Create([FromBody] AddIncidentRequest request) =>
            await mediator.Send(new AddIncidentCommand(request));

        [HttpDelete("{id}")]
        public async Task<long> Delete(long id) =>
            await mediator.Send(new DeleteIncidentCommand(id));

        [HttpPost("Incident/bulk")]
        public async Task<ActionResult<BulkAddIncidentRequest >> CreateIncidents([FromBody] BulkAddIncidentRequest Incidentrequests)
        {
            var result = await mediator.Send(new BulkAddIncidentCommand(Incidentrequests));
            return Ok(result);
        }
    }