    using AutoMapper;
    using DoWhatta.Platform.Data.Mediator.Handlers;
    using NewDoor.Platform.DTO.Features.Events.Models;
    using NewDoor.Platform.Entities;
    using NewDoor.API.Repositories.Interface;
    using NewDoor.API.Features.Events.Query;

    namespace NewDoor.API.Features.Events.Handler
    {
        public class FindAllEventHandler(IMapper mapper, IEventRepository eventRepository)
            : FindAllHandler<FindAllEventQuery, EventResponse, Event, IEventRepository>(mapper, eventRepository);
    }