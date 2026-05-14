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

        [HttpGet("GetById")]
        public async Task<ActionResult<DeviceResponse>> GetById([FromQuery] string id)
        {
            var filter = new DeviceFilterRequest { DeviceId = id };
            var devices = await mediator.Send(new FindAllDeviceQuery(filter));
            var device = devices.FirstOrDefault();

            if (device == null)
                return NotFound(new { message = $"Device with ID '{id}' not found" });

            return Ok(device);
        }

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