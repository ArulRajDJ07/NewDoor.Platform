    using AutoMapper;
    using NewDoor.Platform.DTO.Features.Events.Models;
    using NewDoor.Platform.Entities;

    namespace NewDoor.API.Features.Events.Mapper
    {
        public class EventMapper : Profile
        {
            public EventMapper()
            {
                CreateMap<AddEventRequest, Event>();
                CreateMap<Event, EventResponse>();
            }
        }
    }