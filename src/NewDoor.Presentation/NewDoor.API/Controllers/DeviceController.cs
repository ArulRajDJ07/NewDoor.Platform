    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using NewDoor.Platform.DTO.Features.Devices.Models;
    using NewDoor.API.Features.Devices.Command;
    using NewDoor.API.Features.Devices.Query;

    [Route("api/[controller]")]
    [ApiController]
    public class DeviceController(IMediator mediator) : ControllerBase
    {
        [HttpGet("GetAll")]
        public async Task<List<DeviceResponse>> GetAll([FromQuery] DeviceFilterRequest? filter) =>
            await mediator.Send(new FindAllDeviceQuery(filter));

        [HttpPost]
        public async Task<DeviceResponse> Create([FromBody] AddDeviceRequest request) =>
            await mediator.Send(new AddDeviceCommand(request));

        [HttpDelete("{id}")]
        public async Task<long> Delete(long id) =>
            await mediator.Send(new DeleteDeviceCommand(id));

        [HttpPost("Device/bulk")]
        public async Task<ActionResult<BulkAddDeviceRequest >> CreateDevices([FromBody] BulkAddDeviceRequest Devicerequests)
        {
            var result = await mediator.Send(new BulkAddDeviceCommand(Devicerequests));
            return Ok(result);
        }
    }