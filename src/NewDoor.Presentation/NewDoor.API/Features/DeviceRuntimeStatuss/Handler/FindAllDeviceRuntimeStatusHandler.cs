    using AutoMapper;
    using DoWhatta.Platform.Data.Mediator.Handlers;
    using NewDoor.Platform.DTO.Features.DeviceRuntimeStatuss.Models;
    using NewDoor.Platform.Entities;
    using NewDoor.API.Repositories.Interface;
    using NewDoor.API.Features.DeviceRuntimeStatuss.Query;

    namespace NewDoor.API.Features.DeviceRuntimeStatuss.Handler
    {
        public class FindAllDeviceRuntimeStatusHandler(IMapper mapper, IDeviceRuntimeStatusRepository deviceRuntimeStatusRepository)
            : FindAllHandler<FindAllDeviceRuntimeStatusQuery, DeviceRuntimeStatusResponse, DeviceRuntimeStatus, IDeviceRuntimeStatusRepository>(mapper, deviceRuntimeStatusRepository);
    }