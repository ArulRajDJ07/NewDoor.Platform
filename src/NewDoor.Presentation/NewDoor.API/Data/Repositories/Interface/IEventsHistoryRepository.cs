    using DoWhatta.Platform.Data.Base;
    using DoWhatta.Platform.Core.DependencyInjection;
    using NewDoor.Platform.Entities;
    using NewDoor.Platform.DTO.Features.EventsHistorys.Models;

    namespace NewDoor.API.Repositories.Interface;

    public interface IEventsHistoryRepository : IBaseRepository<EventsHistory>, IscopedService
    {
        Task<int> AddRangeAsync(ICollection<EventsHistory> eventsHistories);
        Task<List<EventsHistory>> GetAllFilteredAsync(EventsHistoryFilterRequest filter);
    }