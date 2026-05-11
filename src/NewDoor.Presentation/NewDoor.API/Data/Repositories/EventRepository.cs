    using DoWhatta.Platform.Data.Base;
    using NewDoor.Platform.Entities;
    using NewDoor.API.Repositories.Interface;
    using NewDoor.API.Data;

    namespace NewDoor.API.Data.Repositories;

    public class EventRepository(DoWhattaProductDBContext context)
        : BaseRepository<Event>(context), IEventRepository
    {
        private readonly DoWhattaProductDBContext _context = context;

        public async Task<int> AddRangeAsync(ICollection<Event> events)
        {
            await DbSet.AddRangeAsync(events);
            return await _context.SaveChangesAsync();
        }
    }