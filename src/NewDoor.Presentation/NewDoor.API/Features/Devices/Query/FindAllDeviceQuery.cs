    using DoWhatta.Platform.Data.Mediator.Queries;
    using NewDoor.Platform.DTO.Features.Devices.Models;

    namespace NewDoor.API.Features.Devices.Query
    {
        public record FindAllDeviceQuery : BaseFindAllQuery<DeviceResponse>;
    }