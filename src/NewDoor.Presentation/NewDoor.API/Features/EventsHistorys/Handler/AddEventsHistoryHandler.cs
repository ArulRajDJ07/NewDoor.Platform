    using AutoMapper;
    using DoWhatta.Platform.Data.Mediator.Handlers;
    using NewDoor.Platform.DTO.Features.EventsHistorys.Models;
    using NewDoor.Platform.Entities;
    using NewDoor.API.Repositories.Interface;
    using NewDoor.API.Features.EventsHistorys.Command;

    namespace NewDoor.API.Features.EventsHistorys.Handler
    {
        public class AddEventsHistoryHandler(IMapper mapper, IEventsHistoryRepository eventsHistoryRepository)
            : BaseAddHandler<AddEventsHistoryCommand, AddEventsHistoryRequest, EventsHistory, IEventsHistoryRepository, EventsHistoryResponse>(mapper, eventsHistoryRepository);
    }