    using DoWhatta.Platform.Data.Base;
    using DoWhatta.Platform.Core.DependencyInjection;
    using NewDoor.Platform.Entities;

    namespace NewDoor.API.Repositories.Interface;

    public interface IIncidentRepository : IBaseRepository<Incident>, IscopedService
    {
        Task<int> AddRangeAsync(ICollection<Incident> incidents);
    }