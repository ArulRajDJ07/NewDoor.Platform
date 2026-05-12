    using DoWhatta.Platform.Data.Mediator.BaseCommands;
    using NewDoor.Platform.DTO.Features.Alarms.Models;

    namespace NewDoor.API.Features.Alarms.Command
    {
        public record BulkAddAlarmCommand(BulkAddAlarmRequest alarmRequest)
            : BaseAddCommand<BulkAddAlarmRequest, int>(alarmRequest);
    }