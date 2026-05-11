    using DoWhatta.Platform.Data.Base;
    using NewDoor.Platform.Entities;
    using NewDoor.API.Repositories.Interface;

    namespace NewDoor.API.Data.Repositories;

    public class DeviceRepository(DoWhattaDBContext context)
        : BaseRepository<Device>(context), IDeviceRepository
    {
    }