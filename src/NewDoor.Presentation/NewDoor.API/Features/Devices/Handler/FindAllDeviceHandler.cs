    using AutoMapper;
    using DoWhatta.Platform.Data.Mediator.Handlers;
    using NewDoor.Platform.DTO.Features.Devices.Models;
    using NewDoor.Platform.Entities;
    using NewDoor.API.Repositories.Interface;
    using NewDoor.API.Features.Devices.Query;

    namespace NewDoor.API.Features.Devices.Handler
    {
        public class FindAllDeviceHandler(IMapper mapper, IDeviceRepository deviceRepository)
            : FindAllHandler<FindAllDeviceQuery, DeviceResponse, Device, IDeviceRepository>(mapper, deviceRepository);
    }