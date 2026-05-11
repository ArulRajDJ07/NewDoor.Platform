    using DoWhatta.Platform.Data.Mediator.Queries;
    using NewDoor.Platform.DTO.Features.Alarms.Models;

    namespace NewDoor.API.Features.Alarms.Query
    {
        public record FindAllAlarmQuery : BaseFindAllQuery<AlarmResponse>;
    }