    using AutoMapper;
    using DoWhatta.Platform.Data.Mediator.Handlers;
    using NewDoor.Platform.DTO.Features.Devices.Models;
    using NewDoor.Platform.Entities;
    using NewDoor.API.Repositories.Interface;
    using NewDoor.API.Features.Devices.Command;

    namespace NewDoor.API.Features.Devices.Handler
    {
        public class AddDeviceHandler(IMapper mapper, IDeviceRepository deviceRepository)
            : BaseAddHandler<AddDeviceCommand, AddDeviceRequest, Device, IDeviceRepository, DeviceResponse>(mapper, deviceRepository);
    }