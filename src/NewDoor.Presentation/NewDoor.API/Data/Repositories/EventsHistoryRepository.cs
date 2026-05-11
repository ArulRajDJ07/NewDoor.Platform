    using DoWhatta.Platform.Data.Base;
    using NewDoor.Platform.Entities;
    using NewDoor.API.Repositories.Interface;
    using NewDoor.API.Data;
    using Microsoft.EntityFrameworkCore;
    using NewDoor.Platform.DTO.Features.EventsHistorys.Models;
    using NewDoor.API.Infrastructure.Extensions;

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

        public async Task<List<EventsHistory>> GetAllFilteredAsync(EventsHistoryFilterRequest filter)
        {
            IQueryable<EventsHistory> query = DbSet;

            if (filter.Id.HasValue)
                query = query.Where(e => e.Id == filter.Id.Value);

            if (filter.MinId.HasValue || filter.MaxId.HasValue)
                query = query.ApplyRangeFilter(nameof(EventsHistory.Id), filter.MinId, filter.MaxId);

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
    }