    using DoWhatta.Platform.Data.Base;
    using DoWhatta.Platform.Core.DependencyInjection;
    using NewDoor.Platform.Entities;

    namespace NewDoor.API.Repositories.Interface;

    public interface IEventRepository : IBaseRepository<Event>, IscopedService
    {
        Task<int> AddRangeAsync(ICollection<Event> events);
    }