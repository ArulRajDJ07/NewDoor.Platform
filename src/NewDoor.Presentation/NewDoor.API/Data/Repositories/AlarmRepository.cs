    using DoWhatta.Platform.Data.Base;
    using NewDoor.Platform.Entities;
    using NewDoor.API.Repositories.Interface;
    using NewDoor.API.Data;

    namespace NewDoor.API.Data.Repositories;

    public class AlarmRepository(DoWhattaProductDBContext context)
        : BaseRepository<Alarm>(context), IAlarmRepository
    {
        private readonly DoWhattaProductDBContext _context = context;

        public async Task<int> AddRangeAsync(ICollection<Alarm> alarms)
        {
            await DbSet.AddRangeAsync(alarms);
            return await _context.SaveChangesAsync();
        }
    }