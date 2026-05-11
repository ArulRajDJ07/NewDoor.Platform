    using AutoMapper;
    using NewDoor.Platform.DTO.Features.Alarms.Models;
    using NewDoor.Platform.Entities;

    namespace NewDoor.API.Features.Alarms.Mapper
    {
        public class AlarmMapper : Profile
        {
            public AlarmMapper()
            {
                CreateMap<AddAlarmRequest, Alarm>();
                CreateMap<Alarm, AlarmResponse>();
            }
        }
    }