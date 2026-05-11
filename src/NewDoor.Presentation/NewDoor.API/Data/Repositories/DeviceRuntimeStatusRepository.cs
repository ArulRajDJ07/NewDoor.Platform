    using DoWhatta.Platform.Data.Base;
    using NewDoor.Platform.Entities;
    using NewDoor.API.Repositories.Interface;
    using NewDoor.API.Data;

    namespace NewDoor.API.Data.Repositories;

    public class DeviceRuntimeStatusRepository(DoWhattaProductDBContext context)
        : BaseRepository<DeviceRuntimeStatus>(context), IDeviceRuntimeStatusRepository
    {
        private readonly DoWhattaProductDBContext _context = context;

        public async Task<int> AddRangeAsync(ICollection<DeviceRuntimeStatus> deviceRuntimeStatuses)
        {
            await DbSet.AddRangeAsync(deviceRuntimeStatuses);
            return await _context.SaveChangesAsync();
        }
    }