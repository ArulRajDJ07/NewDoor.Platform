    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using NewDoor.Platform.DTO.Features.DeviceRuntimeStatuss.Models;
    using NewDoor.API.Features.DeviceRuntimeStatuss.Command;
    using NewDoor.API.Features.DeviceRuntimeStatuss.Query;

    [Route("api/[controller]")]
    [ApiController]
    public class DeviceRuntimeStatusController(IMediator mediator) : ControllerBase
    {
        [HttpGet("GetAll")]
        public async Task<List<DeviceRuntimeStatusResponse>> GetAll([FromQuery] DeviceRuntimeStatusFilterRequest? filter) =>
            await mediator.Send(new FindAllDeviceRuntimeStatusQuery(filter));

        [HttpPost]
        public async Task<DeviceRuntimeStatusResponse> Create([FromBody] AddDeviceRuntimeStatusRequest request) =>
            await mediator.Send(new AddDeviceRuntimeStatusCommand(request));

        [HttpDelete("{id}")]
        public async Task<long> Delete(long id) =>
            await mediator.Send(new DeleteDeviceRuntimeStatusCommand(id));

        [HttpPost("DeviceRuntimeStatus/bulk")]
        public async Task<ActionResult<BulkAddDeviceRuntimeStatusRequest >> CreateDeviceRuntimeStatuss([FromBody] BulkAddDeviceRuntimeStatusRequest DeviceRuntimeStatusrequests)
        {
            var result = await mediator.Send(new BulkAddDeviceRuntimeStatusCommand(DeviceRuntimeStatusrequests));
            return Ok(result);
        }
    }