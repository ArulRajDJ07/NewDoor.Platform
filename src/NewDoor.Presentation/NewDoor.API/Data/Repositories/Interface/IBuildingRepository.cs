    using DoWhatta.Platform.Data.Base;
    using DoWhatta.Platform.Core.DependencyInjection;
    using NewDoor.Platform.Entities;

    namespace NewDoor.API.Repositories.Interface;

    public interface IBuildingRepository : IBaseRepository<Building>, IscopedService
    {
        Task<int> AddRangeAsync(ICollection<Building> buildings);
    }