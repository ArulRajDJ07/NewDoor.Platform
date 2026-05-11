    using DoWhatta.Platform.Data.Mediator.BaseCommands;
    using NewDoor.Platform.DTO.Features.Events.Models;

    namespace NewDoor.API.Features.Events.Command
    {
        public record BulkAddEventCommand(BulkAddEventRequest eventRequest)
            : BaseAddCommand<BulkAddEventRequest, int>(eventRequest);
    }