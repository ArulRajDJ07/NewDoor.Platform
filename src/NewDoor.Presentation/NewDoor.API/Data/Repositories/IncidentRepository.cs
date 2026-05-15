    using DoWhatta.Platform.Data.Base;
    using NewDoor.Platform.Entities;
    using NewDoor.API.Repositories.Interface;
    using NewDoor.API.Data;
    using Microsoft.EntityFrameworkCore;
    using NewDoor.Platform.DTO.Features.Incidents.Models;
    using NewDoor.API.Infrastructure.Extensions;

    namespace NewDoor.API.Data.Repositories;

    public class IncidentRepository(DoWhattaProductDBContext context)
        : BaseRepository<Incident>(context), IIncidentRepository
    {
        private readonly DoWhattaProductDBContext _context = context;

        public async Task<int> AddRangeAsync(ICollection<Incident> incidents)
        {
            await DbSet.AddRangeAsync(incidents);
            return await _context.SaveChangesAsync();
        }

        public async Task<List<Incident>> GetAllFilteredAsync(IncidentFilterRequest filter)
        {
            IQueryable<Incident> query = DbSet;

            if (filter.Id.HasValue)
                query = query.Where(i => i.Id == filter.Id.Value);

            if (filter.MinId.HasValue || filter.MaxId.HasValue)
                query = query.ApplyRangeFilter(nameof(Incident.Id), filter.MinId, filter.MaxId);

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

        public async Task<Incident?> GetByIncidentCodeAsync(string incidentCode)
        {
            return await DbSet.AsNoTracking().FirstOrDefaultAsync(i => i.IncidentCode == incidentCode);
        }
    }