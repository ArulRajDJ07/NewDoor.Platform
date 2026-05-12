    using DoWhatta.Platform.Data.Base;
    using DoWhatta.Platform.Core.DependencyInjection;
    using NewDoor.Platform.Entities;
    using NewDoor.Platform.DTO.Features.Devices.Models;

    namespace NewDoor.API.Repositories.Interface;

    public interface IDeviceRepository : IBaseRepository<Device>, IscopedService
    {
        Task<int> AddRangeAsync(ICollection<Device> devices);
        Task<List<Device>> GetAllFilteredAsync(DeviceFilterRequest filter);
    }