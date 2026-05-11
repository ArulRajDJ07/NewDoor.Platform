    using DoWhatta.Platform.Data.Mediator.Queries;
    using NewDoor.Platform.DTO.Features.EventsHistorys.Models;

    namespace NewDoor.API.Features.EventsHistorys.Query
    {
        public record FindAllEventsHistoryQuery : BaseFindAllQuery<EventsHistoryResponse>;
    }