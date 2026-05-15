    using DoWhatta.Platform.Data.Base;
    using DoWhatta.Platform.Core.DependencyInjection;
    using NewDoor.Platform.Entities;
    using NewDoor.Platform.DTO.Features.Incidents.Models;

    namespace NewDoor.API.Repositories.Interface;

    public interface IIncidentRepository : IBaseRepository<Incident>, IscopedService
    {
        Task<int> AddRangeAsync(ICollection<Incident> incidents);
        Task<List<Incident>> GetAllFilteredAsync(IncidentFilterRequest filter);
        Task<Incident?> GetByIncidentCodeAsync(string incidentCode);
    }