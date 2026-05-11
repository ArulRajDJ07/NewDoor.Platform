    using DoWhatta.Platform.Data.Mediator.BaseCommands;
    using NewDoor.Platform.DTO.Features.Buildings.Models;

    namespace NewDoor.API.Features.Buildings.Command
    {
        public record DeleteBuildingCommand(long Id) : BaseDeleteCommand<long>(Id);
    }