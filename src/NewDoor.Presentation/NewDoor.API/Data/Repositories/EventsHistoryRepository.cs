    using DoWhatta.Platform.Data.Base;
    using NewDoor.Platform.Entities;
    using NewDoor.API.Repositories.Interface;
    using NewDoor.API.Data;

    namespace NewDoor.API.Data.Repositories;

    public class EventsHistoryRepository(DoWhattaProductDBContext context)
        : BaseRepository<EventsHistory>(context), IEventsHistoryRepository
    {
        private readonly DoWhattaProductDBContext _context = context;

        public async Task<int> AddRangeAsync(ICollection<EventsHistory> eventsHistories)
        {
            await DbSet.AddRangeAsync(eventsHistories);
            return await _context.SaveChangesAsync();
        }
    }