    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using NewDoor.Platform.DTO.Features.Alarms.Models;
    using NewDoor.API.Features.Alarms.Command;
    using NewDoor.API.Features.Alarms.Query;

    [Route("api/[controller]")]
    [ApiController]
    public class AlarmController(IMediator mediator) : ControllerBase
    {
        [HttpGet("GetAll")]
        public async Task<List<AlarmResponse>> GetAll([FromQuery] AlarmFilterRequest? filter) =>
            await mediator.Send(new FindAllAlarmQuery(filter));

        [HttpPost]
        public async Task<AlarmResponse> Create([FromBody] AddAlarmRequest request) =>
            await mediator.Send(new AddAlarmCommand(request));

        [HttpDelete("{id}")]
        public async Task<long> Delete(long id) =>
            await mediator.Send(new DeleteAlarmCommand(id));

        [HttpPost("Alarm/bulk")]
        public async Task<ActionResult<BulkAddAlarmRequest >> CreateAlarms([FromBody] BulkAddAlarmRequest Alarmrequests)
        {
            var result = await mediator.Send(new BulkAddAlarmCommand(Alarmrequests));
            return Ok(result);
        }
    }