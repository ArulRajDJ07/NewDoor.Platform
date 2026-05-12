    using DoWhatta.Platform.Data.Base;
    using DoWhatta.Platform.Core.DependencyInjection;
    using NewDoor.Platform.Entities;
    using NewDoor.Platform.DTO.Features.DeviceRuntimeStatuss.Models;

    namespace NewDoor.API.Repositories.Interface;

    public interface IDeviceRuntimeStatusRepository : IBaseRepository<DeviceRuntimeStatus>, IscopedService
    {
        Task<int> AddRangeAsync(ICollection<DeviceRuntimeStatus> deviceRuntimeStatuses);
        Task<List<DeviceRuntimeStatus>> GetAllFilteredAsync(DeviceRuntimeStatusFilterRequest filter);
    }