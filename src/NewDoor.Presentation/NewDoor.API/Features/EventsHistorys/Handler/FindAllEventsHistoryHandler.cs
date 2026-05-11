    using AutoMapper;
    using DoWhatta.Platform.Data.Mediator.Handlers;
    using NewDoor.Platform.DTO.Features.EventsHistorys.Models;
    using NewDoor.Platform.Entities;
    using NewDoor.API.Repositories.Interface;
    using NewDoor.API.Features.EventsHistorys.Query;

    namespace NewDoor.API.Features.EventsHistorys.Handler
    {
        public class FindAllEventsHistoryHandler(IMapper mapper, IEventsHistoryRepository eventsHistoryRepository)
            : FindAllHandler<FindAllEventsHistoryQuery, EventsHistoryResponse, EventsHistory, IEventsHistoryRepository>(mapper, eventsHistoryRepository);
    }