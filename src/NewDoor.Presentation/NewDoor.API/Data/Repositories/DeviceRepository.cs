    using DoWhatta.Platform.Data.Base;
    using NewDoor.Platform.Entities;
    using NewDoor.API.Repositories.Interface;

    namespace NewDoor.API.Data.Repositories;

    public class DeviceRepository(DoWhattaProductDBContext context)
        : BaseRepository<Device>(context), IDeviceRepository
    {
        private readonly DoWhattaProductDBContext _context = context;

        public async Task<int> AddRangeAsync(ICollection<Device> devices)
        {
            await DbSet.AddRangeAsync(devices);
            return await _context.SaveChangesAsync();
        }
    }