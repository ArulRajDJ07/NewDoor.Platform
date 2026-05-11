    using DoWhatta.Platform.Data.Base;
    using DoWhatta.Platform.Core.DependencyInjection;
    using NewDoor.Platform.Entities;

    namespace NewDoor.API.Repositories.Interface;

    public interface IEventsHistoryRepository : IBaseRepository<EventsHistory>, IscopedService
    {
        // Add custom methods here if needed
    }