    using DoWhatta.Platform.Data.Mediator.BaseCommands;
    using NewDoor.Platform.DTO.Features.Buildings.Models;

    namespace NewDoor.API.Features.Buildings.Command
    {
        public record BulkAddBuildingCommand(BulkAddBuildingRequest buildingRequest)
            : BaseAddCommand<BulkAddBuildingRequest, int>(buildingRequest);
    }