    using DoWhatta.Platform.Data.Base;
    using NewDoor.Platform.Entities;
    using NewDoor.API.Repositories.Interface;

    namespace NewDoor.API.Data.Repositories;

    public class BuildingRepository(DoWhattaProductDBContext context)
        : BaseRepository<Building>(context), IBuildingRepository
    {
        private readonly DoWhattaProductDBContext _context = context;

        public async Task<int> AddRangeAsync(ICollection<Building> buildings)
        {
            await DbSet.AddRangeAsync(buildings);
            return await _context.SaveChangesAsync();
        }
    }