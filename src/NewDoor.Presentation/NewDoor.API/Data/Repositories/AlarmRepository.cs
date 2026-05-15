    using DoWhatta.Platform.Data.Base;
    using NewDoor.Platform.Entities;
    using NewDoor.API.Repositories.Interface;
    using NewDoor.API.Data;
    using Microsoft.EntityFrameworkCore;
    using NewDoor.Platform.DTO.Features.Alarms.Models;
    using NewDoor.API.Infrastructure.Extensions;

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

        public async Task<List<Alarm>> GetAllFilteredAsync(AlarmFilterRequest filter)
        {
            IQueryable<Alarm> query = DbSet;

            if (filter.Id.HasValue)
                query = query.Where(a => a.Id == filter.Id.Value);

            if (filter.MinId.HasValue || filter.MaxId.HasValue)
                query = query.ApplyRangeFilter(nameof(Alarm.Id), filter.MinId, filter.MaxId);

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

        public async Task<Alarm?> GetByAlarmCodeAsync(string alarmCode)
        {
            return await DbSet.AsNoTracking().FirstOrDefaultAsync(a => a.AlarmCode == alarmCode);
        }
    }