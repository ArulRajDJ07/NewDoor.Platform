    using DoWhatta.Platform.Data.Base;
    using NewDoor.Platform.Entities;
    using NewDoor.API.Repositories.Interface;
    using NewDoor.API.Data;

    namespace NewDoor.API.Data.Repositories;

    public class AlarmRepository(DoWhattaProductDBContext context)
        : BaseRepository<Alarm>(context), IAlarmRepository
    {
    }