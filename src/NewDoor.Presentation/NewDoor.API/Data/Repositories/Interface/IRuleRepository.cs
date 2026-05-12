    using DoWhatta.Platform.Data.Base;
    using DoWhatta.Platform.Core.DependencyInjection;
    using NewDoor.Platform.Entities;

    namespace NewDoor.API.Repositories.Interface;

    public interface IRuleRepository : IBaseRepository<Rule>, IscopedService
    {
        Task<int> AddRangeAsync(ICollection<Rule> rules);
    }