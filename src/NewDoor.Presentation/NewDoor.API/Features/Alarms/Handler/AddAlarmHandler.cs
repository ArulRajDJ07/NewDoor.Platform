    using AutoMapper;
    using DoWhatta.Platform.Data.Mediator.Handlers;
    using NewDoor.Platform.DTO.Features.Alarms.Models;
    using NewDoor.Platform.Entities;
    using NewDoor.API.Repositories.Interface;
    using NewDoor.API.Features.Alarms.Command;

    namespace NewDoor.API.Features.Alarms.Handler
    {
        public class AddAlarmHandler(IMapper mapper, IAlarmRepository alarmRepository)
            : BaseAddHandler<AddAlarmCommand, AddAlarmRequest, Alarm, IAlarmRepository, AlarmResponse>(mapper, alarmRepository);
    }