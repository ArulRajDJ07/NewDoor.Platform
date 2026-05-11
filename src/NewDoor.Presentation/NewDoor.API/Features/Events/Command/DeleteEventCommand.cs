    using DoWhatta.Platform.Data.Mediator.BaseCommands;
    using NewDoor.Platform.DTO.Features.Events.Models;

    namespace NewDoor.API.Features.Events.Command
    {
        public record DeleteEventCommand(long Id) : BaseDeleteCommand<long>(Id);
    }