    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using NewDoor.Platform.DTO.Features.Buildings.Models;
    using NewDoor.API.Features.Buildings.Command;
    using NewDoor.API.Features.Buildings.Query;

[Route("api/[controller]")]
    [ApiController]
    public class BuildingController(IMediator mediator) : ControllerBase
    {
        [HttpGet("GetAll")]
        public async Task<List<BuildingResponse>> GetAll() =>
            await mediator.Send(new FindAllBuildingQuery());

        [HttpGet("GetAllWithDevices")]
        public async Task<List<BuildingWithDevicesResponse>> GetAllWithDevices([FromQuery] BuildingFilterRequest? filter) =>
            await mediator.Send(new FindAllBuildingsWithDevicesQuery(filter));

        [HttpPost]
        public async Task<BuildingResponse> Create([FromBody] AddBuildingRequest request) =>
            await mediator.Send(new AddBuildingCommand(request));

        [HttpDelete("{id}")]
        public async Task<long> Delete(long id) =>
            await mediator.Send(new DeleteBuildingCommand(id));

        [HttpPost("Building/bulk")]
        public async Task<ActionResult<BulkAddBuildingRequest >> CreateBuildings([FromBody] BulkAddBuildingRequest Buildingrequests)
        {
            var result = await mediator.Send(new BulkAddBuildingCommand(Buildingrequests));
            return Ok(result);
        }
    }