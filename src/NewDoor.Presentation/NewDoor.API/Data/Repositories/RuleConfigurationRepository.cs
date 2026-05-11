    using DoWhatta.Platform.Data.Base;
    using NewDoor.Platform.Entities;
    using NewDoor.API.Repositories.Interface;
    using NewDoor.API.Data;

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
    }