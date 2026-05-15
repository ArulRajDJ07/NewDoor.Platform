    using DoWhatta.Platform.Data.Base;
    using NewDoor.Platform.Entities;
    using NewDoor.API.Repositories.Interface;
    using NewDoor.API.Data;
    using Microsoft.EntityFrameworkCore;
    using NewDoor.Platform.DTO.Features.Events.Models;
    using NewDoor.API.Infrastructure.Extensions;

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

        public async Task<List<Event>> GetAllFilteredAsync(EventFilterRequest filter)
        {
            IQueryable<Event> query = DbSet;

            if (filter.Id.HasValue)
                query = query.Where(e => e.Id == filter.Id.Value);

            if (filter.MinId.HasValue || filter.MaxId.HasValue)
                query = query.ApplyRangeFilter(nameof(Event.Id), filter.MinId, filter.MaxId);

            if (filter.Filters?.Any() == true)
            {
                foreach (var dynamicFilter in filter.Filters)
                {
                    query = query.ApplyFilter(dynamicFilter.FieldName!, dynamicFilter.Operator!, dynamicFilter.Value!);
                }
            }

            query = query.ApplySort(filter.SortBy, filter.SortDirection);
            query = query.ApplyPaging(filter.PageNumber, filter.PageSize);

            return await query.ToListAsync();
        }

        public async Task<Event?> GetByEventIdAsync(string eventId)
        {
            return await DbSet.AsNoTracking().FirstOrDefaultAsync(e => e.EventId == eventId);
        }
    }