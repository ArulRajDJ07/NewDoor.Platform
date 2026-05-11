    using DoWhatta.Platform.Data.Base;
    using NewDoor.Platform.Entities;
    using NewDoor.API.Repositories.Interface;
    using NewDoor.API.Data;

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
    }