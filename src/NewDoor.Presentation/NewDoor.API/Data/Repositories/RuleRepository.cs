    using DoWhatta.Platform.Data.Base;
    using NewDoor.Platform.Entities;
    using NewDoor.API.Repositories.Interface;
    using NewDoor.API.Data;

    namespace NewDoor.API.Data.Repositories;

    public class RuleRepository(DoWhattaProductDBContext context)
        : BaseRepository<Rule>(context), IRuleRepository
    {
        private readonly DoWhattaProductDBContext _context = context;

        public async Task<int> AddRangeAsync(ICollection<Rule> rules)
        {
            await DbSet.AddRangeAsync(rules);
            return await _context.SaveChangesAsync();
        }
    }