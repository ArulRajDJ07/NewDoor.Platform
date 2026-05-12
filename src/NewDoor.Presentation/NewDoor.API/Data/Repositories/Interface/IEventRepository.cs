    using DoWhatta.Platform.Data.Base;
    using DoWhatta.Platform.Core.DependencyInjection;
    using NewDoor.Platform.Entities;
    using NewDoor.Platform.DTO.Features.Events.Models;

    namespace NewDoor.API.Repositories.Interface;

    public interface IEventRepository : IBaseRepository<Event>, IscopedService
    {
        Task<int> AddRangeAsync(ICollection<Event> events);
        Task<List<Event>> GetAllFilteredAsync(EventFilterRequest filter);
    }