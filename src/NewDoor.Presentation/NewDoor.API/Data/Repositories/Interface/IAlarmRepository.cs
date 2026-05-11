    using DoWhatta.Platform.Data.Base;
    using DoWhatta.Platform.Core.DependencyInjection;
    using NewDoor.Platform.Entities;

    namespace NewDoor.API.Repositories.Interface;

    public interface IAlarmRepository : IBaseRepository<Alarm>, IscopedService
    {
        // Add custom methods here if needed
    }