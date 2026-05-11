    using DoWhatta.Platform.Data.Base;
    using DoWhatta.Platform.Core.DependencyInjection;
    using NewDoor.Platform.Entities;

    namespace NewDoor.API.Repositories.Interface;

    public interface IDeviceRepository : IBaseRepository<Device>, IscopedService
    {
        // Add custom methods here if needed
    }