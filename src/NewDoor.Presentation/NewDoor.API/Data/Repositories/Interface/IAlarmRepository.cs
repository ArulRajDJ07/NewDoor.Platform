    using DoWhatta.Platform.Data.Base;
    using DoWhatta.Platform.Core.DependencyInjection;
    using NewDoor.Platform.Entities;
    using NewDoor.Platform.DTO.Features.Alarms.Models;

    namespace NewDoor.API.Repositories.Interface;

    public interface IAlarmRepository : IBaseRepository<Alarm>, IscopedService
    {
        Task<int> AddRangeAsync(ICollection<Alarm> alarms);
        Task<List<Alarm>> GetAllFilteredAsync(AlarmFilterRequest filter);
        Task<Alarm?> GetByAlarmCodeAsync(string alarmCode);
    }