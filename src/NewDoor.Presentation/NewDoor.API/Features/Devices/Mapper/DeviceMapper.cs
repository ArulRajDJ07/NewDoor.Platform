    using AutoMapper;
    using NewDoor.Platform.DTO.Features.Devices.Models;
    using NewDoor.Platform.Entities;

    namespace NewDoor.API.Features.Devices.Mapper
    {
        public class DeviceMapper : Profile
        {
            public DeviceMapper()
            {
                CreateMap<AddDeviceRequest, Device>();
                CreateMap<Device, DeviceResponse>();
            }
        }
    }