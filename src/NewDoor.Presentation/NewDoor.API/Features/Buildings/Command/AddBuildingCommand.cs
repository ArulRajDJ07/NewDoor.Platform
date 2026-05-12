    using DoWhatta.Platform.Data.Mediator.BaseCommands;
using NewDoor.Platform.DTO.Features.Buildings.Models;
using NewDoor.Platform.DTO.Features.Buildings.Models;

    namespace NewDoor.API.Features.Buildings.Command
    {
        public record AddBuildingCommand(AddBuildingRequest buildingRequest)
            : BaseAddCommand<AddBuildingRequest, BuildingResponse>(buildingRequest);
    }