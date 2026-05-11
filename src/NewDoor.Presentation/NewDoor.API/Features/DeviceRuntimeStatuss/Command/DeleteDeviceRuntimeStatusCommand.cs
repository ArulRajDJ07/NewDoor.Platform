    using DoWhatta.Platform.Data.Mediator.BaseCommands;
    using NewDoor.Platform.DTO.Features.DeviceRuntimeStatuss.Models;

    namespace NewDoor.API.Features.DeviceRuntimeStatuss.Command
    {
        public record DeleteDeviceRuntimeStatusCommand(long Id) : BaseDeleteCommand<long>(Id);
    }