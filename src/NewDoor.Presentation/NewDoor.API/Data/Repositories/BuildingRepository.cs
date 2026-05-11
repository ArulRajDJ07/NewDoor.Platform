    using DoWhatta.Platform.Data.Base;
    using NewDoor.Platform.Entities;
    using NewDoor.API.Repositories.Interface;

    namespace NewDoor.API.Data.Repositories;

    public class BuildingRepository(DoWhattaDBContext context)
        : BaseRepository<Building>(context), IBuildingRepository
    {
    }