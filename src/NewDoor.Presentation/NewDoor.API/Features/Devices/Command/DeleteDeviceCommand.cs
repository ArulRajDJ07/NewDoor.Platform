    using DoWhatta.Platform.Data.Mediator.BaseCommands;
    using NewDoor.Platform.DTO.Features.Devices.Models;

    namespace NewDoor.API.Features.Devices.Command
    {
        public record DeleteDeviceCommand(long Id) : BaseDeleteCommand<long>(Id);
    }