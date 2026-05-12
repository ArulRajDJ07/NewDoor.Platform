    using AutoMapper;
    using DoWhatta.Platform.Data.Mediator.Handlers;
    using NewDoor.Platform.DTO.Features.DeviceRuntimeStatuss.Models;
    using NewDoor.Platform.Entities;
    using NewDoor.API.Repositories.Interface;
    using NewDoor.API.Features.DeviceRuntimeStatuss.Command;

    namespace NewDoor.API.Features.DeviceRuntimeStatuss.Handler
    {
        public class AddDeviceRuntimeStatusHandler(IMapper mapper, IDeviceRuntimeStatusRepository deviceRuntimeStatusRepository)
            : BaseAddHandler<AddDeviceRuntimeStatusCommand, AddDeviceRuntimeStatusRequest, DeviceRuntimeStatus, IDeviceRuntimeStatusRepository, DeviceRuntimeStatusResponse>(mapper, deviceRuntimeStatusRepository);
    }