    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using NewDoor.Platform.DTO.Features.EventsHistorys.Models;
    using NewDoor.API.Features.EventsHistorys.Command;
    using NewDoor.API.Features.EventsHistorys.Query;

    [Route("api/[controller]")]
    [ApiController]
    public class EventsHistoryController(IMediator mediator) : ControllerBase
    {
        [HttpGet("GetAll")]
        public async Task<List<EventsHistoryResponse>> GetAll([FromQuery] EventsHistoryFilterRequest? filter) =>
            await mediator.Send(new FindAllEventsHistoryQuery(filter));

        [HttpPost]
        public async Task<EventsHistoryResponse> Create([FromBody] AddEventsHistoryRequest request) =>
            await mediator.Send(new AddEventsHistoryCommand(request));

        [HttpDelete("{id}")]
        public async Task<long> Delete(long id) =>
            await mediator.Send(new DeleteEventsHistoryCommand(id));

        [HttpPost("EventsHistory/bulk")]
        public async Task<ActionResult<BulkAddEventsHistoryRequest >> CreateEventsHistorys([FromBody] BulkAddEventsHistoryRequest EventsHistoryrequests)
        {
            var result = await mediator.Send(new BulkAddEventsHistoryCommand(EventsHistoryrequests));
            return Ok(result);
        }
    }