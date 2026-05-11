    using DoWhatta.Platform.Data.Mediator.Queries;
    using NewDoor.Platform.DTO.Features.DeviceRuntimeStatuss.Models;

    namespace NewDoor.API.Features.DeviceRuntimeStatuss.Query
    {
        public record FindAllDeviceRuntimeStatusQuery(DeviceRuntimeStatusFilterRequest? Filter = null) : BaseFindAllQuery<DeviceRuntimeStatusResponse>;
    }