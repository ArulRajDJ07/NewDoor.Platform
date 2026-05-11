    using DoWhatta.Platform.Data.Base;
    using DoWhatta.Platform.Core.DependencyInjection;
    using NewDoor.Platform.Entities;

    namespace NewDoor.API.Repositories.Interface;

    public interface IDeviceRuntimeStatusRepository : IBaseRepository<DeviceRuntimeStatus>, IscopedService
    {
        Task<int> AddRangeAsync(ICollection<DeviceRuntimeStatus> deviceRuntimeStatuses);
    }