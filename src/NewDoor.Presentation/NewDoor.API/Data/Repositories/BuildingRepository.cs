    using DoWhatta.Platform.Data.Base;
    using NewDoor.Platform.Entities;
    using NewDoor.API.Repositories.Interface;
    using Microsoft.EntityFrameworkCore;

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

        public async Task<List<(Building Building, List<Device> Devices)>> GetAllBuildingsWithDevicesAsync()
        {
            var buildings = await DbSet.ToListAsync();
            var devices = await _context.Set<Device>().ToListAsync();

            return buildings.Select(building => (
                Building: building,
                Devices: devices.Where(d => d.BuildingId == building.Id).ToList()
            )).ToList();
        }
    }