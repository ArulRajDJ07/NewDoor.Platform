    using AutoMapper;
    using DoWhatta.Platform.Data.Mediator.Handlers;
    using NewDoor.Platform.DTO.Features.Alarms.Models;
    using NewDoor.Platform.Entities;
    using NewDoor.API.Repositories.Interface;
    using NewDoor.API.Features.Alarms.Query;

    namespace NewDoor.API.Features.Alarms.Handler
    {
        public class FindAllAlarmHandler(IMapper mapper, IAlarmRepository alarmRepository)
            : FindAllHandler<FindAllAlarmQuery, AlarmResponse, Alarm, IAlarmRepository>(mapper, alarmRepository);
    }