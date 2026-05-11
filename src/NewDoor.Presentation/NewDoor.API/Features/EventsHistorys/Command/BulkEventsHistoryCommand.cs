    using DoWhatta.Platform.Data.Mediator.BaseCommands;
    using NewDoor.Platform.DTO.Features.EventsHistorys.Models;

    namespace NewDoor.API.Features.EventsHistorys.Command
    {
        public record BulkAddEventsHistoryCommand(BulkAddEventsHistoryRequest eventsHistoryRequest)
            : BaseAddCommand<BulkAddEventsHistoryRequest, int>(eventsHistoryRequest);
    }