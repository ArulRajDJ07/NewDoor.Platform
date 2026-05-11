    using DoWhatta.Platform.Data.Base;
    using NewDoor.Platform.Entities;
    using NewDoor.API.Repositories.Interface;
    using NewDoor.API.Data;
    using Microsoft.EntityFrameworkCore;
    using NewDoor.Platform.DTO.Features.RuleConfigurations.Models;
    using NewDoor.API.Infrastructure.Extensions;

    namespace NewDoor.API.Data.Repositories;

    public class RuleConfigurationRepository(DoWhattaProductDBContext context)
        : BaseRepository<RuleConfiguration>(context), IRuleConfigurationRepository
    {
        private readonly DoWhattaProductDBContext _context = context;

        public async Task<int> AddRangeAsync(ICollection<RuleConfiguration> ruleConfigurations)
        {
            await DbSet.AddRangeAsync(ruleConfigurations);
            return await _context.SaveChangesAsync();
        }

        public async Task<List<RuleConfiguration>> GetAllFilteredAsync(RuleConfigurationFilterRequest filter)
        {
            IQueryable<RuleConfiguration> query = DbSet;

            if (filter.Id.HasValue)
                query = query.Where(r => r.Id == filter.Id.Value);

            if (filter.MinId.HasValue || filter.MaxId.HasValue)
                query = query.ApplyRangeFilter(nameof(RuleConfiguration.Id), filter.MinId, filter.MaxId);

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