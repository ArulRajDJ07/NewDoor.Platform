    using DoWhatta.Platform.Data.Base;
    using DoWhatta.Platform.Core.DependencyInjection;
    using NewDoor.Platform.Entities;

    namespace NewDoor.API.Repositories.Interface;

    public interface IEventsHistoryRepository : IBaseRepository<EventsHistory>, IscopedService
    {
        Task<int> AddRangeAsync(ICollection<EventsHistory> eventsHistories);
    }