    using AutoMapper;
    using NewDoor.Platform.DTO.Features.DeviceRuntimeStatuss.Models;
    using NewDoor.Platform.Entities;

    namespace NewDoor.API.Features.DeviceRuntimeStatuss.Mapper
    {
        public class DeviceRuntimeStatusMapper : Profile
        {
            public DeviceRuntimeStatusMapper()
            {
                CreateMap<AddDeviceRuntimeStatusRequest, DeviceRuntimeStatus>();
                CreateMap<DeviceRuntimeStatus, DeviceRuntimeStatusResponse>();
            }
        }
    }