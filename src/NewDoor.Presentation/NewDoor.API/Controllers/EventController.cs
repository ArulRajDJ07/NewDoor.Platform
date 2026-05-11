    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using NewDoor.Platform.DTO.Features.Events.Models;
    using NewDoor.API.Features.Events.Command;
    using NewDoor.API.Features.Events.Query;

    [Route("api/[controller]")]
    [ApiController]
    public class EventController(IMediator mediator) : ControllerBase
    {
        [HttpGet("GetAll")]
        public async Task<List<EventResponse>> GetAll() =>
            await mediator.Send(new FindAllEventQuery());

        [HttpPost]
        public async Task<EventResponse> Create([FromBody] AddEventRequest request) =>
            await mediator.Send(new AddEventCommand(request));

        [HttpDelete("{id}")]
        public async Task<long> Delete(long id) =>
            await mediator.Send(new DeleteEventCommand(id));

        [HttpPost("Event/bulk")]
        public async Task<ActionResult<BulkAddEventRequest >> CreateEvents([FromBody] BulkAddEventRequest Eventrequests)
        {
            var result = await mediator.Send(new BulkAddEventCommand(Eventrequests));
            return Ok(result);
        }
    }