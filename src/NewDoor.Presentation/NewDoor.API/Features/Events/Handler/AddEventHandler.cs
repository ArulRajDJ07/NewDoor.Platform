    using AutoMapper;
    using DoWhatta.Platform.Data.Mediator.Handlers;
    using NewDoor.Platform.DTO.Features.Events.Models;
    using NewDoor.Platform.Entities;
    using NewDoor.API.Repositories.Interface;
    using NewDoor.API.Features.Events.Command;

    namespace NewDoor.API.Features.Events.Handler
    {
        public class AddEventHandler(IMapper mapper, IEventRepository eventRepository)
            : BaseAddHandler<AddEventCommand, AddEventRequest, Event, IEventRepository, EventResponse>(mapper, eventRepository);
    }