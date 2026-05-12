    using AutoMapper;
    using NewDoor.Platform.DTO.Features.EventsHistorys.Models;
    using NewDoor.Platform.Entities;

    namespace NewDoor.API.Features.EventsHistorys.Mapper
    {
        public class EventsHistoryMapper : Profile
        {
            public EventsHistoryMapper()
            {
                CreateMap<AddEventsHistoryRequest, EventsHistory>();
                CreateMap<EventsHistory, EventsHistoryResponse>();
            }
        }
    }