    using DoWhatta.Platform.Data.Mediator.Queries;
    using NewDoor.Platform.DTO.Features.Events.Models;

    namespace NewDoor.API.Features.Events.Query
    {
        public record FindAllEventQuery(EventFilterRequest? Filter = null) : BaseFindAllQuery<EventResponse>;
    }